using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

using System.Linq;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Interface.Masters;

namespace InventoryTrackingAutomation.EventHandlers.Movements;

/// <summary>
/// Workflow motorundan gelen "iş akışı tamamlandı/reddedildi" event'lerini dinleyen ve
/// MovementRequest tablosunu asenkron/izole olarak güncelleyen Event Handler.
/// </summary>
public class MovementRequestWorkflowEventHandler : ILocalEventHandler<WorkflowCompletedEto>, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly InventoryTrackingAutomation.Interface.Movements.IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly InventoryTrackingAutomation.Interface.Stock.IProductStockRepository _productStockRepository;
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IShipmentLineRepository _shipmentLineRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly Volo.Abp.Guids.IGuidGenerator _guidGenerator;

    public MovementRequestWorkflowEventHandler(
        IMovementRequestRepository movementRequestRepository,
        InventoryTrackingAutomation.Interface.Movements.IMovementRequestLineRepository movementRequestLineRepository,
        InventoryTrackingAutomation.Interface.Stock.IProductStockRepository productStockRepository,
        IShipmentRepository shipmentRepository,
        IShipmentLineRepository shipmentLineRepository,
        IVehicleRepository vehicleRepository,
        IWorkerRepository workerRepository,
        Volo.Abp.Guids.IGuidGenerator guidGenerator)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _productStockRepository = productStockRepository;
        _shipmentRepository = shipmentRepository;
        _shipmentLineRepository = shipmentLineRepository;
        _vehicleRepository = vehicleRepository;
        _workerRepository = workerRepository;
        _guidGenerator = guidGenerator;
    }

    [Volo.Abp.Uow.UnitOfWork]
    public virtual async Task HandleEventAsync(WorkflowCompletedEto eventData)
    {
        // Event sadece "MovementRequest" nesnesi için fırlatılmışsa ilgileniyoruz.
        if (eventData.EntityType != "MovementRequest")
        {
            return;
        }

        var request = await _movementRequestRepository.FindAsync(eventData.EntityId);
        if (request == null)
        {
            return;
        }

        // Workflow sonucuna göre MovementRequest'in asıl durumunu (Status) güncelliyoruz.
        if (eventData.FinalState == WorkflowState.Completed)
        {
            // Stok Kontrolü ve Düşümü
            var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == request.Id);
            foreach (var line in lines)
            {
                var stock = await _productStockRepository.FindAsync(x => x.ProductId == line.ProductId && x.SiteId == request.SourceSiteId);
                
                if (stock == null || stock.TotalQuantity < line.Quantity)
                {
                    // UOW rollback yapması için hata fırlatıyoruz!
                    // Böylece Workflow'un onaylanması (Approved durumu) da veritabanına Commit edilmeyecek!
                    throw new Volo.Abp.UserFriendlyException($"Stok yetersiz! İstenen: {line.Quantity}, Mevcut: {stock?.TotalQuantity ?? 0}");
                }

                stock.TotalQuantity -= line.Quantity;
                await _productStockRepository.UpdateAsync(stock, autoSave: true);
            }

            // Faz 1 - Arabaya Yükleme (Sevkiyat Oluşturma)
            // Araç talep sırasında seçilir; final onayda yeniden uygunluk kontrolü yapılır.
            if (!request.RequestedVehicleId.HasValue)
            {
                throw new Volo.Abp.UserFriendlyException("Sevkiyat için talep edilen araç bulunamadı.");
            }

            var vehicle = await _vehicleRepository.FindAsync(request.RequestedVehicleId.Value);
            if (vehicle == null || !vehicle.IsActive)
            {
                throw new Volo.Abp.UserFriendlyException("Talep edilen araç bulunamadı veya aktif değil.");
            }

            var activeShipments = await _shipmentRepository.GetListAsync(x =>
                x.VehicleId == request.RequestedVehicleId.Value &&
                (x.Status == ShipmentStatusEnum.Preparing || x.Status == ShipmentStatusEnum.InTransit));
            var activeShipment = activeShipments.FirstOrDefault();
            if (activeShipment != null)
            {
                throw new Volo.Abp.UserFriendlyException("Talep edilen araç aktif bir sevkiyatta olduğu için atanamaz.");
            }

            var driver = (await _workerRepository.GetListAsync(x => x.WorkerType == WorkerTypeEnum.BlueCollar)).FirstOrDefault();
            
            if (vehicle != null && driver != null)
            {
                var shipment = new Shipment(_guidGenerator.Create())
                {
                    ShipmentNumber = $"SHP-{request.RequestNumber}",
                    VehicleId = vehicle.Id,
                    DriverWorkerId = driver.Id,
                    Status = ShipmentStatusEnum.Preparing,
                    PlannedDepartureTime = System.DateTime.UtcNow.AddHours(2)
                };
                
                await _shipmentRepository.InsertAsync(shipment, autoSave: true);
                
                foreach (var line in lines)
                {
                    var shipmentLine = new ShipmentLine(_guidGenerator.Create())
                    {
                        ShipmentId = shipment.Id,
                        MovementRequestLineId = line.Id,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity
                    };
                    await _shipmentLineRepository.InsertAsync(shipmentLine, autoSave: true);
                }

                // Talebi sevkiyata bağla
                request.ShipmentId = shipment.Id;
            }

            request.Status = MovementStatusEnum.Approved;
        }
        else if (eventData.FinalState == WorkflowState.Rejected)
        {
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }
}
