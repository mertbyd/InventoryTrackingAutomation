using System.Threading.Tasks;
using InventoryTrackingAutomation.Managers;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// Hedef lokasyona girmeyen hasar, kayip veya tuketim stok duzeltmelerini yonetir.
/// </summary>
public class StockAdjustmentManager : InventoryTrackingAutomationDomainService
{
    public StockAdjustmentManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private StockLocationManager _stockLocationManager => LazyGetRequiredService<StockLocationManager>();
    private InventoryTransactionManager _transactionManager => LazyGetRequiredService<InventoryTransactionManager>();



    /// Stok miktarını azaltmak (düzeltme) için kullanılır.
    public async Task<InventoryTransaction> DecreaseAsync(StockAdjustmentModel model)
    {
        if (model.Quantity <= 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.QuantityMustBePositive);
        }

        await _stockLocationManager.DecreaseAsync(
            model.SourceLocationType,
            model.SourceLocationId,
            model.ProductId,
            model.Quantity);

        return await _transactionManager.RecordAdjustmentAsync(model);
    }
}
