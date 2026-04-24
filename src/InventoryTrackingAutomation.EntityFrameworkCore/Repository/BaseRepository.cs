using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace InventoryTrackingAutomation.Repository;

/// <summary>
/// Tüm repository'lerin türeyeceği ortak base sınıf — temel CRUD + bulk + soft delete.
/// </summary>
public class BaseRepository<T> : EfCoreRepository<InventoryTrackingAutomationDbContext, T, Guid>, IBaseRepository<T>
    where T : class, IEntity<Guid>
{
    public BaseRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Birden fazla entity'yi tek işlemde insert eder ve insert edilmiş listeyi döner.
    /// </summary>
    public async Task<List<T>> InsertManyAndGetListAsync(IEnumerable<T> entities)
    {
        var list = entities.ToList();
        await InsertManyAsync(list, autoSave: true);
        return list;
    }

    /// <summary>
    /// Id'ye göre soft delete yapar — entity ISoftDelete implement etmiyorsa exception fırlatır.
    /// </summary>
    public async Task SoftDeleteAsync(Guid id)
    {
        if (!typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
        {
            throw new BusinessException("Entity:SoftDeleteNotSupported")
                .WithData("EntityType", typeof(T).Name);
        }

        await DeleteAsync(id, autoSave: true);
    }

    /// <summary>
    /// Toplu soft delete — verilen Id listesindeki tüm entity'leri siler.
    /// </summary>
    public async Task SoftDeleteManyAsync(IEnumerable<Guid> ids)
    {
        if (!typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
        {
            throw new BusinessException("Entity:SoftDeleteNotSupported")
                .WithData("EntityType", typeof(T).Name);
        }

        await DeleteManyAsync(ids, autoSave: true);
    }
}
