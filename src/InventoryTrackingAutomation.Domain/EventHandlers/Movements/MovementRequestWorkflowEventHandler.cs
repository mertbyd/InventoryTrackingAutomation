using System.Threading.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Interface.Movements;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace InventoryTrackingAutomation.EventHandlers.Movements;

/// <summary>
/// Workflow motorundan gelen "iş akışı tamamlandı/reddedildi" event'lerini dinleyen ve
/// MovementRequest tablosunu asenkron/izole olarak güncelleyen Event Handler.
/// </summary>
public class MovementRequestWorkflowEventHandler : ILocalEventHandler<WorkflowCompletedEto>, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly InventoryTrackingAutomation.Interface.Movements.IMovementRequestLineRepository _movementRequestLineRepository;
    private readonly InventoryTrackingAutomation.Interface.Stock.IProductStockRepository _productStockRepository;

    public MovementRequestWorkflowEventHandler(
        IMovementRequestRepository movementRequestRepository,
        InventoryTrackingAutomation.Interface.Movements.IMovementRequestLineRepository movementRequestLineRepository,
        InventoryTrackingAutomation.Interface.Stock.IProductStockRepository productStockRepository)
    {
        _movementRequestRepository = movementRequestRepository;
        _movementRequestLineRepository = movementRequestLineRepository;
        _productStockRepository = productStockRepository;
    }

    [Volo.Abp.Uow.UnitOfWork]
    public virtual async Task HandleEventAsync(WorkflowCompletedEto eventData)
    {
        // Event sadece "MovementRequest" nesnesi için fırlatılmışsa ilgileniyoruz.
        if (eventData.EntityType != "MovementRequest")
        {
            return;
        }

        var request = await _movementRequestRepository.FindAsync(eventData.EntityId);
        if (request == null)
        {
            return;
        }

        // Workflow sonucuna göre MovementRequest'in asıl durumunu (Status) güncelliyoruz.
        if (eventData.FinalState == WorkflowState.Completed)
        {
            // Stok Kontrolü ve Düşümü
            var lines = await _movementRequestLineRepository.GetListAsync(x => x.MovementRequestId == request.Id);
            foreach (var line in lines)
            {
                var stock = await _productStockRepository.FindAsync(x => x.ProductId == line.ProductId && x.SiteId == request.SourceSiteId);
                
                if (stock == null || stock.TotalQuantity < line.Quantity)
                {
                    // UOW rollback yapması için hata fırlatıyoruz!
                    // Böylece Workflow'un onaylanması (Approved durumu) da veritabanına Commit edilmeyecek!
                    throw new Volo.Abp.UserFriendlyException($"Stok yetersiz! İstenen: {line.Quantity}, Mevcut: {stock?.TotalQuantity ?? 0}");
                }

                stock.TotalQuantity -= line.Quantity;
                await _productStockRepository.UpdateAsync(stock, autoSave: true);
            }

            request.Status = MovementStatusEnum.Approved;
        }
        else if (eventData.FinalState == WorkflowState.Rejected)
        {
            request.Status = MovementStatusEnum.Rejected;
        }

        await _movementRequestRepository.UpdateAsync(request, autoSave: true);
    }
}
