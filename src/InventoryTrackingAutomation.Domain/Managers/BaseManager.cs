using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.ExceptionCodes;
using InventoryTrackingAutomation.Interface;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Managers;

// islevi: Domain manager'lar icin ortak varlik, benzersizlik ve enum dogrulama yardimcilarini toplar.
// sistemdeki gorevi: Tekil ve toplu validasyonlarda ayni kurallarin tekrar yazilmasini engeller.
public abstract class BaseManager<TEntity> : InventoryTrackingAutomationDomainService
    where TEntity : class, IEntity<Guid>
{
    protected readonly IBaseRepository<TEntity> Repository;

    // Tureyen manager kendi modulune ait exception code'u ezmelidir.
    protected virtual string AlreadyExistsErrorCode => GeneralExceptionCodes.InvalidOperation;

    protected BaseManager(IBaseRepository<TEntity> repository)
    {
        Repository = repository;
    }

    protected BaseManager(
        IBaseRepository<TEntity> repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
        Repository = repository;
    }

    /// Entity'nin varligini dogrulamak ve getirmek icin kullanilir.
    public async Task<TEntity> EnsureExistsAsync(Guid id)
    {
        var entity = await Repository.FindAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TEntity), id);
        }

        return entity;
    }

    /// Belirli bir repository'de entity varligini dogrulamak icin kullanilir.
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

    /// Belirli bir ABP repository'de entity varligini dogrulamak icin kullanilir.
    public async Task EnsureExistsInAsync<TOther>(
        IRepository<TOther, Guid> otherRepository,
        Guid id)
        where TOther : class, IEntity<Guid>
    {
        var entity = await otherRepository.FindAsync(id);
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(TOther), id);
        }
    }

    /// Opsiyonel entity varligini dogrulamak icin kullanilir.
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

    /// Opsiyonel ABP repository entity varligini dogrulamak icin kullanilir.
    public async Task EnsureExistsInAsync<TOther>(
        IRepository<TOther, Guid> otherRepository,
        Guid? id)
        where TOther : class, IEntity<Guid>
    {
        if (id.HasValue)
        {
            await EnsureExistsInAsync(otherRepository, id.Value);
        }
    }

    /// Verilen tum ID'lerin varligini tek async repository sorgusu ile dogrular.
    public async Task EnsureAllExistInAsync<TOther>(
        IBaseRepository<TOther> otherRepository,
        IEnumerable<Guid> ids)
        where TOther : class, IEntity<Guid>
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return;
        }

        if (idList.Contains(Guid.Empty))
        {
            throw new EntityNotFoundException(typeof(TOther), Guid.Empty);
        }

        var foundEntities = await otherRepository.GetListAsync(x => idList.Contains(x.Id));
        EnsureAllIdsFound<TOther>(idList, foundEntities.Select(x => x.Id));
    }

    /// Verilen tum ID'lerin varligini tek async ABP repository sorgusu ile dogrular.
    public async Task EnsureAllExistInAsync<TOther>(
        IRepository<TOther, Guid> otherRepository,
        IEnumerable<Guid> ids)
        where TOther : class, IEntity<Guid>
    {
        var idList = ids.Distinct().ToList();
        if (idList.Count == 0)
        {
            return;
        }

        if (idList.Contains(Guid.Empty))
        {
            throw new EntityNotFoundException(typeof(TOther), Guid.Empty);
        }

        var foundEntities = await otherRepository.GetListAsync(x => idList.Contains(x.Id));
        EnsureAllIdsFound<TOther>(idList, foundEntities.Select(x => x.Id));
    }

    /// Yeni kayit icin benzersizlik kontrolu yapar.
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate)
    {
        var exists = (await Repository.GetListAsync(predicate)).Any();
        if (exists)
        {
            throw new BusinessException(AlreadyExistsErrorCode);
        }
    }

    /// Yeni kayitlar icin input ici ve DB'deki benzersizlik kurallarini toplu kontrol eder.
    public async Task EnsureUniqueBulkAsync<TValue>(
        IEnumerable<TValue> values,
        Expression<Func<TEntity, TValue>> propertySelector)
    {
        var rawValues = values.Where(v => v != null).ToList();
        if (rawValues.Count == 0)
        {
            return;
        }

        var duplicateValue = rawValues
            .GroupBy(v => v)
            .FirstOrDefault(g => g.Count() > 1);

        if (duplicateValue != null)
        {
            throw new BusinessException(AlreadyExistsErrorCode)
                .WithData("Value", duplicateValue.Key!);
        }

        var valueList = rawValues.Distinct().ToList();
        var parameter = propertySelector.Parameters[0];
        var containsMethod = typeof(List<TValue>).GetMethod(nameof(List<TValue>.Contains), new[] { typeof(TValue) });
        var containsExpression = Expression.Call(Expression.Constant(valueList), containsMethod!, propertySelector.Body);
        var lambda = Expression.Lambda<Func<TEntity, bool>>(containsExpression, parameter);

        var exists = (await Repository.GetListAsync(lambda)).Any();
        if (exists)
        {
            throw new BusinessException(AlreadyExistsErrorCode);
        }
    }

    /// Guncelleme icin benzersizlik kontrolu yapar.
    public async Task EnsureUniqueAsync(
        Expression<Func<TEntity, bool>> predicate,
        Guid excludeId)
    {
        var exists = (await Repository.GetListAsync(predicate))
            .Any(e => !e.Id.Equals(excludeId));

        if (exists)
        {
            throw new BusinessException(AlreadyExistsErrorCode);
        }
    }

    /// Enum degerinin gecerliligini dogrulamak icin kullanilir.
    protected async Task EnsureValidEnumAsync<TEnum>(TEnum value, string settingName) where TEnum : struct, Enum
    {
        var enumValidationManager = LazyGetRequiredService<InventoryTrackingAutomation.Managers.Shared.EnumValidationManager>();
        await enumValidationManager.ValidateAllowedEnumAsync(value, settingName);
    }

    private static void EnsureAllIdsFound<TOther>(IReadOnlyCollection<Guid> expectedIds, IEnumerable<Guid> foundIds)
        where TOther : class, IEntity<Guid>
    {
        var foundIdSet = foundIds.ToHashSet();
        var missingId = expectedIds.FirstOrDefault(id => !foundIdSet.Contains(id));
        if (missingId != Guid.Empty || foundIdSet.Count != expectedIds.Count)
        {
            throw new EntityNotFoundException(typeof(TOther), missingId);
        }
    }
}
