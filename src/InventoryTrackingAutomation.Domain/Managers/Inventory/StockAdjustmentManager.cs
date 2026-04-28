using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// Hedef lokasyona girmeyen hasar, kayip veya tuketim stok duzeltmelerini yonetir.
/// </summary>
public class StockAdjustmentManager : DomainService
{
    private readonly StockLocationManager _stockLocationManager;
    private readonly InventoryTransactionManager _transactionManager;

    public StockAdjustmentManager(
        StockLocationManager stockLocationManager,
        InventoryTransactionManager transactionManager)
    {
        _stockLocationManager = stockLocationManager;
        _transactionManager = transactionManager;
    }

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
