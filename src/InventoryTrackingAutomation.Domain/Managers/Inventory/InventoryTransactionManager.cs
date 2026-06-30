using AutoMapper;
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
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public InventoryTransactionManager(IInventoryTransactionRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// Yeni bir stok hareket kaydı oluşturmak için kullanılır.
    public async Task<InventoryTransaction> CreateAsync(CreateInventoryTransactionModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.RelatedMovementRequestId);
        ValidateQuantity(model.Quantity);

        var entity = new InventoryTransaction(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Mevcut stok hareketini guncelleme istegini append-only ledger kuralina gore reddeder.
    /// </summary>
    // islevi: Ledger kaydinin sonradan degistirilmesini engeller.
    // sistemdeki gorevi: Stok gecmisinin kanonik audit kaydi olarak guvenilir kalmasini saglar.
    public async Task<InventoryTransaction> RejectUpdateAsync(System.Guid id)
    {
        await EnsureExistsAsync(id);
        throw CreateImmutableLedgerException("Update");
    }

    /// <summary>
    /// Stok hareketi silme istegini append-only ledger kuralina gore reddeder.
    /// </summary>
    // islevi: Ledger kaydinin soft/hard delete ile kaybolmasini engeller.
    // sistemdeki gorevi: InventoryTransaction tablosunu operasyonel geriye donuk izleme defteri olarak korur.
    public async Task DeleteAsync(System.Guid id)
    {
        await EnsureExistsAsync(id);
        throw CreateImmutableLedgerException("Delete");
    }

    /// Hareket referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(System.Guid productId, System.Guid? movementRequestId)
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

    /// <summary>
    /// Degistirilemez ledger ihlalleri icin ortak domain hatasini olusturur.
    /// </summary>
    // islevi: Update/Delete gibi mutasyon denemelerini tek hata kodu ile standartlastirir.
    // sistemdeki gorevi: API, log ve UI tarafinda stok defteri mutasyon hatalarinin ayni kodla izlenmesini saglar.
    private static BusinessException CreateImmutableLedgerException(string operation)
    {
        return new BusinessException(InventoryTransactionExceptionCodes.ImmutableLedger)
            .WithData("Operation", operation);
    }

    /// Stok hareketini ledger'a kaydetmek için kullanılır.
    public async Task<InventoryTransaction> RecordAsync(InventoryTrackingAutomation.Models.Inventory.StockTransferModel model)
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
