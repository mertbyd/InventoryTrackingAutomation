using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// Atomik stok transfer orkestratöru. (PITON plani - 8.6)
/// </summary>
//işlevi: StockTransfer etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class StockTransferManager : DomainService
{
    private readonly StockLocationManager _stockLocationManager;
    private readonly InventoryTransactionManager _transactionManager;

    public StockTransferManager(
        StockLocationManager stockLocationManager,
        InventoryTransactionManager transactionManager)
    {
        _stockLocationManager = stockLocationManager;
        _transactionManager = transactionManager;
    }

    /// <summary>
    /// Atomik transfer: kaynaktan dus, hedefe ekle, ledger'a yaz. Hepsi tek UoW; biri patlarsa hepsi rollback.
    /// </summary>
    //işlevi: İki lokasyon arasında güvenli (transactional) stok transferi yapar.
    //sistemdeki görevi: Atomik stok operasyonlarının ve mutabakatın merkezi yöneticisidir.
    public async Task<InventoryTransaction> ExecuteAsync(StockTransferModel model)
    {
        if (model.Quantity <= 0)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.QuantityMustBePositive);
        if (model.SourceLocationType == model.DestinationLocationType && model.SourceLocationId == model.DestinationLocationId)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidLocationPair);

        await _stockLocationManager.DecreaseAsync(model.SourceLocationType, model.SourceLocationId, model.ProductId, model.Quantity);
        await _stockLocationManager.IncreaseAsync(model.DestinationLocationType, model.DestinationLocationId, model.ProductId, model.Quantity);
        
        return await _transactionManager.RecordAsync(model);
    }
}
