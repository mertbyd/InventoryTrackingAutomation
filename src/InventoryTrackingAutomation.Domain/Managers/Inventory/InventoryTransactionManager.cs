using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;

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
    private readonly IProductRepository _productRepository;
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly IMapper _mapper;

    public InventoryTransactionManager(
        IInventoryTransactionRepository repository,
        IProductRepository productRepository,
        IMovementRequestRepository movementRequestRepository,
        IVehicleTaskRepository vehicleTaskRepository,
        IMapper mapper)
        : base(repository)
    {
        _productRepository = productRepository;
        _movementRequestRepository = movementRequestRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _mapper = mapper;
    }

    /// Yeni bir stok hareket kaydı oluşturmak için kullanılır.
    public async Task<InventoryTransaction> CreateAsync(CreateInventoryTransactionModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.RelatedMovementRequestId, model.RelatedTaskId);
        ValidateQuantity(model.Quantity);

        var entity = new InventoryTransaction(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// Mevcut bir stok hareket kaydını güncellemek için kullanılır.
    public async Task<InventoryTransaction> UpdateAsync(InventoryTransaction existing, UpdateInventoryTransactionModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.RelatedMovementRequestId, model.RelatedTaskId);
        ValidateQuantity(model.Quantity);

        _mapper.Map(model, existing);
        return existing;
    }

    /// Hareket referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(System.Guid productId, System.Guid? movementRequestId, System.Guid? vehicleTaskId)
    {
        // Transaction kaynak referanslari domain katmaninda dogrulanir.
        await EnsureExistsInAsync(_productRepository, productId);
        await EnsureExistsInAsync(_movementRequestRepository, movementRequestId);
        await EnsureExistsInAsync(_vehicleTaskRepository, vehicleTaskId);
    }

    /// Miktarın geçerliliğini doğrulamak için kullanılır.
    private static void ValidateQuantity(int quantity)
    {
        // Sifir veya negatif transfer miktari kaydedilmez.
        if (quantity <= 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.InventoryTransactions.InvalidTransfer);
        }
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
            RelatedTaskId = model.RelatedTaskId,
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
            RelatedTaskId = model.RelatedTaskId,
            PerformedByUserId = model.PerformedByUserId,
            Note = model.Note
        };

        await Repository.InsertAsync(entity, autoSave: true);
        return entity;
    }
}
