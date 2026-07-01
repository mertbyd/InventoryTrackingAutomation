using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Interface;

/// <summary>
/// Tüm repository interface'lerinin türediği generic base repository arayüzü.
/// </summary>
public interface IBaseRepository<T> : IRepository<T, Guid> where T : class, IEntity<Guid>
{
    /// <summary>
    /// Verilen koşula uyan kayıt olup olmadığını liste çekmeden kontrol eder.
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

    /// <summary>
    /// Toplu insert yapar ve insert edilmiş listeyi döner.
    /// </summary>
    Task<List<T>> InsertManyAndGetListAsync(IEnumerable<T> entities);

    /// <summary>
    /// Id'ye göre soft delete yapar — ISoftDelete implement etmiyorsa exception fırlatır.
    /// </summary>
    Task SoftDeleteAsync(Guid id);

    /// <summary>
    /// Toplu soft delete — verilen Id listesindeki tüm entity'leri siler.
    /// </summary>
    Task SoftDeleteManyAsync(IEnumerable<Guid> ids);
}

