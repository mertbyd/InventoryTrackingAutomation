using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers;

// Tüm domain manager'larının türeyeceği generic base — varlık/unique/mapping/enum doğrulama helper'larını merkezileştirir.
// Türetilmiş manager'lar tekrarlı kontrolleri buradan kullanır, sadece kendine özgü iş kurallarını ekler.
//işlevi: Base etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public abstract class BaseManager<TEntity> : DomainService
    where TEntity : class, IEntity<Guid>
{
    // Yönetilen entity tipinin temel repository'si (alt sınıflara açık).
    protected readonly IBaseRepository<TEntity> Repository;

    // Yönetilen entity'nin repository'sini DI ile alır.
    protected BaseManager(IBaseRepository<TEntity> repository)
    {
        Repository = repository;
    }

    // ════════════════════════════════════════════════════════
    //  VARLIK KONTROL HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// Entity'nin varlığını doğrulamak ve getirmek için kullanılır.
    public async Task<TEntity> EnsureExistsAsync(Guid id)
    {
        var entity = await Repository.FindAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    /// Belirli bir repository'de entity varlığını doğrulamak için kullanılır.
    public async Task EnsureExistsInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        Guid id)
        where TOther : class, IEntity<Guid>
    {
        var entity = await otherRepository.FindAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TOther), id);
        }
    }

    /// Opsiyonel entity varlığını doğrulamak için kullanılır.
    public async Task EnsureExistsInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        Guid? id)
        where TOther : class, IEntity<Guid>
    {
        if (id.HasValue)
        {
            await EnsureExistsInAsync(otherRepository, id.Value);
        }
    }

    /// Verilen tüm ID'lerin varlığını topluca doğrulamak için kullanılır.
    public async Task EnsureAllExistInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        IEnumerable<Guid> ids)
        where TOther : class, IEntity<Guid>
    {
        // Distinct id listesi.
        var idList = ids.Distinct().ToList();
        // Mevcut id'leri tek query ile çek.
        var queryable = await otherRepository.GetQueryableAsync();
        var foundIds = queryable
            .Where(x => idList.Contains(x.Id))
            .Select(x => x.Id)
            .ToHashSet();

        // Listede olup DB'de olmayan ilk id'yi bul ve onu raporla.
        var missingId = idList.FirstOrDefault(id => !foundIds.Contains(id));
        if (missingId != Guid.Empty || foundIds.Count != idList.Count)
        {
            throw new EntityNotFoundException(typeof(TOther), missingId);
        }
    }

    // ════════════════════════════════════════════════════════
    //  UNIQUE KONTROL HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// Yeni kayıt için benzersizlik kontrolü yapmak için kullanılır.
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate)
    {
        var queryable = await Repository.GetQueryableAsync();
        if (queryable.Any(predicate))
        {
            throw new BusinessException(BuildAlreadyExistsErrorCode());
        }
    }

    /// Güncelleme için benzersizlik kontrolü yapmak için kullanılır.
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate,
        Guid excludeId)
    {
        var queryable = await Repository.GetQueryableAsync();
        var exists = queryable
            .Where(predicate)
            .Any(e => !e.Id.Equals(excludeId));
        if (exists)
        {
            throw new BusinessException(BuildAlreadyExistsErrorCode());
        }
    }

    /// 'Zaten mevcut' hata kodunu oluşturmak için kullanılır.
    private static string BuildAlreadyExistsErrorCode()
        => $"InventoryTrackingAutomation:{typeof(TEntity).Name}.AlreadyExists";

    // ════════════════════════════════════════════════════════
    //  ENUM VALIDASYON HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// Enum değerinin geçerliliğini doğrulamak için kullanılır.
    protected async Task EnsureValidEnumAsync<TEnum>(TEnum value, string settingName) where TEnum : struct, Enum
    {
        var enumValidationManager = LazyServiceProvider.LazyGetRequiredService<InventoryTrackingAutomation.Managers.Shared.EnumValidationManager>();
        await enumValidationManager.ValidateAllowedEnumAsync(value, settingName);
    }
}
