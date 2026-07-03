using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Mappers.Notifications;
using InventoryTrackingAutomation.Dtos.Notifications;
using InventoryTrackingAutomation.Events.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Notifications;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.EventHandlers.Notifications;

/// <summary>
/// Depodan urun cikisi gerceklestiginde deponun sorumlusuna bildirim gonderir.
/// </summary>
// islevi: Dispatch basina tek yayinlanan WarehouseStockDispatchedEto'yu dinler; kaynak deponun sorumlusunu cozer.
// sistemdeki gorevi: Sevkiyat kac urun satiri icerirse icersin depo sorumlusuna dispatch basina tek bildirim uretir.
public class WarehouseStockDispatchedNotificationHandler :
    NotificationEventHandler<WarehouseStockDispatchedEto>,
    ITransientDependency
{
    private static readonly InventoryNotificationMapper _mapper = new();

    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWorkerRepository _workerRepository;

    public WarehouseStockDispatchedNotificationHandler(
        IEnumerable<IInventoryNotificationSender> notificationSenders,
        IWarehouseRepository warehouseRepository,
        IWorkerRepository workerRepository)
        : base(notificationSenders)
    {
        _warehouseRepository = warehouseRepository;
        _workerRepository = workerRepository;
    }

    // Deponun sorumlusu tanimli degilse veya sorumlunun kullanici hesabi yoksa bildirim uretilmez.
    protected override async Task<Guid?> ResolveTargetUserIdAsync(WarehouseStockDispatchedEto eventData)
    {
        var warehouse = await _warehouseRepository.FindAsync(eventData.SourceWarehouseId);
        if (warehouse?.ManagerWorkerId == null)
        {
            return null;
        }

        var managerWorker = await _workerRepository.FindAsync(warehouse.ManagerWorkerId.Value);
        return managerWorker?.UserId;
    }

    protected override InventoryNotificationPayload CreatePayload(WarehouseStockDispatchedEto eventData)
    {
        return _mapper.MapToPayload(eventData);
    }
}
