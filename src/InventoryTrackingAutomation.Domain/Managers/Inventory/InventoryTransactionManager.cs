using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.ExceptionCodes;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// InventoryTransaction domain manager'i - stok hareketi denetim kaydi kurallarini yonetir.
/// </summary>
//işlevi: Stok hareketlerinin (ledger/defter) kayıt altına alınmasını yönetir.
//sistemdeki görevii: Tüm stok değişimlerinin tarihçesini (audit trail) tutar ve geçmişe dönük izlenebilirlik sağlar.
//işlevi: InventoryTransaction etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class InventoryTransactionManager : BaseManager<InventoryTransaction>
{
    private IProductRepository _productRepository => LazyGetRequiredService<IProductRepository>();
    private IMovementRequestRepository _movementRequestRepository => LazyGetRequiredService<IMovementRequestRepository>();

    public InventoryTransactionManager(IInventoryTransactionRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    protected override string UpdateNotSupportedErrorCode => InventoryTransactionExceptionCodes.UpdateNotSupported;
    protected override string DeleteNotSupportedErrorCode => InventoryTransactionExceptionCodes.DeleteNotSupported;

    /// Yeni bir stok hareket kaydı oluşturmak için kullanılır.
    public async Task<InventoryTransaction> CreateAsync(CreateInventoryTransactionModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.RelatedMovementRequestId);
        ValidateQuantity(model.Quantity);

        var entity = new InventoryTransaction(GuidGenerator.Create());
        return entity;
    }

    /// <summary>
    /// Birden fazla stok hareket kaydı oluşturmak için toplu validasyon yapar.
    /// </summary>
    public async Task<List<CreateInventoryTransactionModel>> CreateManyAsync(List<CreateInventoryTransactionModel> models)
    {
        var productIds = models.Select(x => x.ProductId).Distinct().ToList();
        if (productIds.Any()) await EnsureAllExistInAsync(_productRepository, productIds);

        var movementIds = models.Where(x => x.RelatedMovementRequestId.HasValue).Select(x => x.RelatedMovementRequestId.Value).Distinct().ToList();
        if (movementIds.Any()) await EnsureAllExistInAsync(_movementRequestRepository, movementIds);

        foreach (var model in models)
        {
            ValidateQuantity(model.Quantity);
        }

        return models;
    }



    /// Hareket referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(Guid productId, Guid? movementRequestId)
    {
        // Transaction kaynak referanslari domain katmaninda repository uzerinden dogrulanir.
        await EnsureExistsInAsync(_productRepository, productId);
        await EnsureExistsInAsync(_movementRequestRepository, movementRequestId);
    }

    /// Miktarın geçerliliğini doğrulamak için kullanılır.
    private static void ValidateQuantity(int quantity)
    {
        // Sifir veya negatif transfer miktari kaydedilmez.
        if (quantity <= 0)
        {
            throw new BusinessException(InventoryTransactionExceptionCodes.InvalidTransfer);
        }
    }



    /// Stok hareketini ledger'a kaydetmek için kullanılır.
    public async Task<InventoryTransaction> RecordAsync(StockTransferModel model)
    {
        var entity = new InventoryTransaction(GuidGenerator.Create())
        {
            TransactionType = model.TransactionType,
            SourceLocationType = model.SourceLocationType,
            SourceLocationId = model.SourceLocationId,
            TargetLocationType = model.DestinationLocationType,
            TargetLocationId = model.DestinationLocationId,
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            OccurredAt = System.DateTime.UtcNow,
            RelatedMovementRequestId = model.RelatedMovementRequestId,
            PerformedByUserId = model.PerformedByUserId,
            Note = model.Note
        };
        await Repository.InsertAsync(entity, autoSave: true);
        return entity;
    }

    /// Stok düzeltme kaydını ledger'a yazmak için kullanılır.
    public async Task<InventoryTransaction> RecordAdjustmentAsync(StockAdjustmentModel model)
    {
        var entity = new InventoryTransaction(GuidGenerator.Create())
        {
            TransactionType = InventoryTransactionTypeEnum.Adjustment,
            SourceLocationType = model.SourceLocationType,
            SourceLocationId = model.SourceLocationId,
            TargetLocationType = null,
            TargetLocationId = null,
            ProductId = model.ProductId,
            Quantity = model.Quantity,
            OccurredAt = System.DateTime.UtcNow,
            RelatedMovementRequestId = model.RelatedMovementRequestId,
            PerformedByUserId = model.PerformedByUserId,
            Note = model.Note
        };

        await Repository.InsertAsync(entity, autoSave: true);
        return entity;
    }
}


