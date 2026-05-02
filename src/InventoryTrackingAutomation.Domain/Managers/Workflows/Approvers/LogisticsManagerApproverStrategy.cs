using System;
using InventoryTrackingAutomation.Managers;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Sahaya malzeme çıkışlarında (TaskMovementRequest), çıkışın yapıldığı kaynak deponun yöneticisini onaycı olarak çözer.
// Bu adım, hedef depo olmadığı durumlarda lojistik operasyon onayını temsil eder.
public class LogisticsManagerApproverStrategy : InventoryTrackingAutomationLazyService, IApproverStrategy, ITransientDependency
{
    public LogisticsManagerApproverStrategy(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IMovementRequestRepository _movementRequestRepository => LazyGetRequiredService<IMovementRequestRepository>();
    private IWarehouseRepository _warehouseRepository => LazyGetRequiredService<IWarehouseRepository>();
    private IWorkerRepository _workerRepository => LazyGetRequiredService<IWorkerRepository>();



    public string Key => WorkflowResolverKeys.LogisticsManager;

    public Task<Guid?> ResolveAsync(ApproverContext context)
    {
        // Sahaya çıkışlarda her zaman SourceWarehouse üzerinden onaycı çözümlenir.
        return WarehouseApproverResolver.ResolveAsync(
            context,
            useSourceWarehouse: true,
            _movementRequestRepository,
            _warehouseRepository,
            _workerRepository);
    }
}
