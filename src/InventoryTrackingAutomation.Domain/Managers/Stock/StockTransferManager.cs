using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Models.Stock;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Depo ve arac stoklari arasindaki bakiyeli transferleri yoneten domain manager.
/// </summary>
public class StockTransferManager : DomainService
{
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;

    public StockTransferManager(
        IStockLocationRepository stockLocationRepository,
        IInventoryTransactionRepository inventoryTransactionRepository)
    {
        _stockLocationRepository = stockLocationRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
    }

    public async Task ExecuteAsync(StockTransferModel model)
    {
        ValidateTransfer(model);

        var sourceLocation = await _stockLocationRepository.FindAsync(x =>
            x.ProductId == model.ProductId &&
            x.LocationType == model.SourceLocationType &&
            x.LocationId == model.SourceLocationId);

        if (sourceLocation == null || sourceLocation.Quantity < model.Quantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.StockLocations.InsufficientStock)
                .WithData("ProductId", model.ProductId)
                .WithData("RequestedQuantity", model.Quantity)
                .WithData("AvailableQuantity", sourceLocation?.Quantity ?? 0);
        }

        var targetLocation = await _stockLocationRepository.FindAsync(x =>
            x.ProductId == model.ProductId &&
            x.LocationType == model.TargetLocationType &&
            x.LocationId == model.TargetLocationId);

        sourceLocation.Quantity -= model.Quantity;
        await _stockLocationRepository.UpdateAsync(sourceLocation, autoSave: true);

        if (targetLocation == null)
        {
            targetLocation = new StockLocation(GuidGenerator.Create())
            {
                ProductId = model.ProductId,
                LocationType = model.TargetLocationType,
                LocationId = model.TargetLocationId,
                Quantity = model.Quantity,
                ReservedQuantity = 0
            };
            await _stockLocationRepository.InsertAsync(targetLocation, autoSave: true);
        }
        else
        {
            targetLocation.Quantity += model.Quantity;
            await _stockLocationRepository.UpdateAsync(targetLocation, autoSave: true);
        }

        await CreateTransactionAsync(model);
    }

    private async Task CreateTransactionAsync(StockTransferModel model)
    {
        // Ledger kaydi transferin denetim izini stok bakiyesinden ayri tutar.
        var transaction = new InventoryTransaction(GuidGenerator.Create())
        {
            ProductId = model.ProductId,
            TransactionType = model.TransactionType,
            Quantity = model.Quantity,
            SourceLocationType = model.SourceLocationType,
            SourceLocationId = model.SourceLocationId,
            TargetLocationType = model.TargetLocationType,
            TargetLocationId = model.TargetLocationId,
            RelatedMovementRequestId = model.RelatedMovementRequestId,
            RelatedTaskId = model.RelatedTaskId,
            PerformedByUserId = model.PerformedByUserId,
            OccurredAt = Clock.Now,
            Note = model.Note
        };

        await _inventoryTransactionRepository.InsertAsync(transaction, autoSave: true);
    }

    private static void ValidateTransfer(StockTransferModel model)
    {
        // Transfer miktari ve lokasyon ciftinin is kurali tek noktada korunur.
        if (model.Quantity <= 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidTransfer);
        }

        if (model.SourceLocationType == model.TargetLocationType &&
            model.SourceLocationId == model.TargetLocationId)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidTransfer);
        }
    }
}
