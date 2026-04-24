using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Services;
using Volo.Abp.ObjectMapping;

namespace InventoryTrackingAutomation.Managers;

/// <summary>
/// Tüm domain manager'larının türeyeceği generic base — tekrarlı validasyon ve mapping işlerini merkezileştirir.
/// </summary>
/// <typeparam name="TEntity">Yönetilen entity tipi (IEntity&lt;Guid&gt; olmalı).</typeparam>
public abstract class BaseManager<TEntity> : DomainService
    where TEntity : class, IEntity<Guid>
{
    protected readonly IBaseRepository<TEntity> Repository;

    // DomainService'de ObjectMapper property'si yoktur; LazyServiceProvider üzerinden çözülür.
    private IObjectMapper? _objectMapper;
    protected IObjectMapper ObjectMapper => _objectMapper ??= LazyServiceProvider.LazyGetRequiredService<IObjectMapper>();

    protected BaseManager(IBaseRepository<TEntity> repository)
    {
        Repository = repository;
    }

    // ════════════════════════════════════════════════════════
    //  VARLIK KONTROL HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Id'ye ait entity DB'de varsa döner, yoksa verilen errorCode ile BusinessException fırlatır.
    /// </summary>
    public async Task<TEntity> EnsureExistsAsync(Guid id, string errorCode)
    {
        var entity = await Repository.FindAsync(id);
        if (entity == null)
        {
            throw new BusinessException(errorCode);
        }

        return entity;
    }

    /// <summary>
    /// Başka tipteki bir repository'de id'nin varlığını kontrol eder (zorunlu FK validasyonu için).
    /// </summary>
    public async Task EnsureExistsInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        Guid id,
        string errorCode)
        where TOther : class, IEntity<Guid>
    {
        var entity = await otherRepository.FindAsync(id);
        if (entity == null)
        {
            throw new BusinessException(errorCode);
        }
    }

    /// <summary>
    /// Opsiyonel FK için — null ise kontrolü atlar.
    /// </summary>
    public async Task EnsureExistsInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        Guid? id,
        string errorCode)
        where TOther : class, IEntity<Guid>
    {
        if (id.HasValue)
        {
            await EnsureExistsInAsync(otherRepository, id.Value, errorCode);
        }
    }

    /// <summary>
    /// Id listesindeki tüm entity'lerin başka bir repository'de var olduğunu doğrular — biri eksikse exception.
    /// </summary>
    public async Task EnsureAllExistInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        IEnumerable<Guid> ids,
        string errorCode)
        where TOther : class, IEntity<Guid>
    {
        var idList = ids.Distinct().ToList();
        var queryable = await otherRepository.GetQueryableAsync();
        var foundCount = queryable.Count(x => idList.Contains(x.Id));

        if (foundCount != idList.Count)
        {
            throw new BusinessException(errorCode);
        }
    }

    // ════════════════════════════════════════════════════════
    //  UNIQUE KONTROL HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Create için unique kontrol — predicate'e uyan kayıt varsa BusinessException fırlatır.
    /// </summary>
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate,
        string errorCode)
    {
        var queryable = await Repository.GetQueryableAsync();
        if (queryable.Any(predicate))
        {
            throw new BusinessException(errorCode);
        }
    }

    /// <summary>
    /// Update için unique kontrol — kendisi (excludeId) hariç tutularak kontrol yapılır.
    /// </summary>
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate,
        Guid excludeId,
        string errorCode)
    {
        var queryable = await Repository.GetQueryableAsync();
        var exists = queryable
            .Where(predicate)
            .Any(e => !e.Id.Equals(excludeId));
        if (exists)
        {
            throw new BusinessException(errorCode);
        }
    }

    // ════════════════════════════════════════════════════════
    //  MAPPING HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Model'den yeni entity oluşturur — GuidGenerator ile Id atar, ObjectMapper ile property'leri doldurur.
    /// </summary>
    public TEntity MapAndAssignId<TModel>(TModel model)
    {
        var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), GuidGenerator.Create())!;
        ObjectMapper.Map<TModel, TEntity>(model, entity);
        return entity;
    }

    /// <summary>
    /// Update model'ini mevcut entity üzerine map eder — audit alanlarını bozmaz.
    /// </summary>
    public TEntity MapForUpdate<TModel>(TModel model, TEntity existingEntity)
    {
        ObjectMapper.Map<TModel, TEntity>(model, existingEntity);
        return existingEntity;
    }

    // ════════════════════════════════════════════════════════
    //  ENUM VALIDASYON HELPER'LARI
    // ════════════════════════════════════════════════════════

    /// <summary>
    /// Verilen enum değerinin, SettingProvider'da tanımlı izin verilen değerler
    /// listesinde olup olmadığını kontrol eder. Base sınıfların constructor'ını
    /// bozmamak için LazyServiceProvider üzerinden resolve edilir.
    /// </summary>
    protected async Task EnsureValidEnumAsync<TEnum>(TEnum value, string settingName) where TEnum : struct, Enum
    {
        var enumValidationManager = LazyServiceProvider.LazyGetRequiredService<InventoryTrackingAutomation.Managers.Shared.EnumValidationManager>();
        await enumValidationManager.ValidateAllowedEnumAsync(value, settingName);
    }
}
