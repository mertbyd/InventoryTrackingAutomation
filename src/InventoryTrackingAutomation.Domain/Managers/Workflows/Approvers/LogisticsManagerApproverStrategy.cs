using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Sahaya malzeme çıkışlarında (TaskMovementRequest), çıkışın yapıldığı kaynak deponun yöneticisini onaycı olarak çözer.
// Bu adım, hedef depo olmadığı durumlarda lojistik operasyon onayını temsil eder.
public class LogisticsManagerApproverStrategy : IApproverStrategy, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IWorkerRepository _workerRepository;

    public LogisticsManagerApproverStrategy(
        IMovementRequestRepository movementRequestRepository,
        IWarehouseRepository warehouseRepository,
        IWorkerRepository workerRepository)
    {
        _movementRequestRepository = movementRequestRepository;
        _warehouseRepository = warehouseRepository;
        _workerRepository = workerRepository;
    }

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
