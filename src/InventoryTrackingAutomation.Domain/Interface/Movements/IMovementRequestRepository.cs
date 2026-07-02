using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Models.Movements;

namespace InventoryTrackingAutomation.Interface.Movements;

/// <summary>
/// MovementRequest entity'si için repository arayüzü.
/// </summary>
public interface IMovementRequestRepository : IBaseRepository<MovementRequest>
{
    /// <summary>
    /// Hareket talebinin task ve vehicle-task baglamini tek repository sorgusunda getirir.
    /// </summary>
    Task<MovementRequestOperationalContextModel?> GetOperationalContextAsync(Guid movementRequestId);
    
    /// <summary>
    /// Birden fazla hareket talebinin task ve vehicle-task bağlamını toplu olarak (batch) getirir.
    /// N+1 sorgu problemini engellemek için kullanılır.
    /// </summary>
    Task<Dictionary<Guid, MovementRequestOperationalContextModel>> GetOperationalContextsAsync(System.Collections.Generic.IEnumerable<Guid> movementRequestIds);
    /// <summary>
    /// Iade talebi icin ayni vehicle-task'a ait son ana movement'i bulur.
    /// </summary>
    Task<Guid?> FindLatestMainMovementIdAsync(Guid vehicleTaskId);
}

