using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Models.Shipments;

namespace InventoryTrackingAutomation.Managers.Shipments;

/// <summary>
/// Sevkiyat satırı domain manager'ı — ShipmentLine entity'si için iş kuralları ve validasyonları.
/// </summary>
public class ShipmentLineManager : BaseManager<ShipmentLine>
{
    private readonly IShipmentRepository _shipmentRepository;                        // ShipmentId FK validasyonu için
    private readonly IMovementRequestLineRepository _movementRequestLineRepository;  // MovementRequestLineId FK validasyonu için
    private readonly IProductRepository _productRepository;                           // ProductId FK validasyonu için

    /// <summary>
    /// ShipmentLineManager constructor'ı.
    /// </summary>
    public ShipmentLineManager(
        IShipmentLineRepository repository,
        IShipmentRepository shipmentRepository,
        IMovementRequestLineRepository movementRequestLineRepository,
        IProductRepository productRepository)
        : base(repository)
    {
        _shipmentRepository = shipmentRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Yeni sevkiyat satırı oluşturur — ShipmentId, MovementRequestLineId ve ProductId varlık kontrolü yapar.
    /// </summary>
    public async Task<ShipmentLine> CreateAsync(CreateShipmentLineModel model)
    {
        await EnsureExistsInAsync(
            _shipmentRepository,
            model.ShipmentId,
            InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);

        await EnsureExistsInAsync(
            _movementRequestLineRepository,
            model.MovementRequestLineId,
            InventoryTrackingAutomationDomainErrorCodes.MovementRequestLines.NotFound);

        await EnsureExistsInAsync(
            _productRepository,
            model.ProductId,
            InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);

        return MapAndAssignId(model);
    }

    /// <summary>
    /// Sevkiyat satırını günceller — ShipmentId, MovementRequestLineId ve ProductId varlık kontrolleri yapar.
    /// </summary>
    public async Task<ShipmentLine> UpdateAsync(ShipmentLine existing, UpdateShipmentLineModel model)
    {
        if (existing.ShipmentId != model.ShipmentId)
        {
            await EnsureExistsInAsync(
                _shipmentRepository,
                model.ShipmentId,
                InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);
        }

        if (existing.MovementRequestLineId != model.MovementRequestLineId)
        {
            await EnsureExistsInAsync(
                _movementRequestLineRepository,
                model.MovementRequestLineId,
                InventoryTrackingAutomationDomainErrorCodes.MovementRequestLines.NotFound);
        }

        if (existing.ProductId != model.ProductId)
        {
            await EnsureExistsInAsync(
                _productRepository,
                model.ProductId,
                InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);
        }

        return MapForUpdate(model, existing);
    }
}
