using System.Threading.Tasks;
using InventoryTrackingAutomation.Managers;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.ExceptionCodes;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// Atomik stok transfer orkestratöru. (PITON plani - 8.6)
/// </summary>
//işlevi: StockTransfer etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class StockTransferManager : InventoryTrackingAutomationDomainService
{
    public StockTransferManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private StockLocationManager _stockLocationManager => LazyGetRequiredService<StockLocationManager>();
    private InventoryTransactionManager _transactionManager => LazyGetRequiredService<InventoryTransactionManager>();



    /// Stok transfer işlemini gerçekleştirmek için kullanılır.
    public async Task<InventoryTransaction> ExecuteAsync(StockTransferModel model)
    {
        if (model.Quantity <= 0)
            throw new BusinessException(InventoryTransactionExceptionCodes.QuantityMustBePositive);
        if (model.SourceLocationType == model.DestinationLocationType && model.SourceLocationId == model.DestinationLocationId)
            throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair);

        await _stockLocationManager.DecreaseAsync(model.SourceLocationType, model.SourceLocationId, model.ProductId, model.Quantity);
        await _stockLocationManager.IncreaseAsync(model.DestinationLocationType, model.DestinationLocationId, model.ProductId, model.Quantity);

        return await _transactionManager.RecordAsync(model);
    }
}
