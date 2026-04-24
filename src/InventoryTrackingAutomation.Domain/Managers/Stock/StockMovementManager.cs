using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Models.Stock;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Stok hareketi domain manager'ı — StockMovement entity'si için iş kuralları ve validasyonları.
/// </summary>
public class StockMovementManager : BaseManager<StockMovement>
{
    private readonly IProductRepository _productRepository;  // ProductId FK validasyonu için
    private readonly ISiteRepository _siteRepository;        // SiteId FK validasyonu için

    /// <summary>
    /// StockMovementManager constructor'ı.
    /// </summary>
    public StockMovementManager(
        IStockMovementRepository repository,
        IProductRepository productRepository,
        ISiteRepository siteRepository)
        : base(repository)
    {
        _productRepository = productRepository;
        _siteRepository = siteRepository;
    }

    /// <summary>
    /// Yeni stok hareketi oluşturur — ProductId ve SiteId varlık kontrolü yapar.
    /// </summary>
    public async Task<StockMovement> CreateAsync(CreateStockMovementModel model)
    {
        await EnsureExistsInAsync(
            _productRepository,
            model.ProductId,
            InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);

        await EnsureExistsInAsync(
            _siteRepository,
            model.SiteId,
            InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);

        await EnsureValidEnumAsync(model.MovementType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Stock.AllowedStockMovementTypes);

        return MapAndAssignId(model);
    }

    /// <summary>
    /// Stok hareketini günceller — ProductId ve SiteId varlık kontrolleri yapar.
    /// </summary>
    public async Task<StockMovement> UpdateAsync(StockMovement existing, UpdateStockMovementModel model)
    {
        if (existing.ProductId != model.ProductId)
        {
            await EnsureExistsInAsync(
                _productRepository,
                model.ProductId,
                InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);
        }

        if (existing.SiteId != model.SiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.SiteId,
                InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        }

        await EnsureValidEnumAsync(model.MovementType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Stock.AllowedStockMovementTypes);

        return MapForUpdate(model, existing);
    }
}
