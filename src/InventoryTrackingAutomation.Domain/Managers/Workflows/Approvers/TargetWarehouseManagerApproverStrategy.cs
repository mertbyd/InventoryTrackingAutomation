using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Hareket talebinin hedef (Target) lokasyonunun yöneticisini onaycı olarak çözer.
// Şu an sadece MovementRequest tipinde aktif; başka entity tipleri için ayrı bir strategy eklenmeli.
public class TargetWarehouseManagerApproverStrategy : IApproverStrategy, ITransientDependency
{
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IWarehouseRepository _WarehouseRepository;
    private readonly IWorkerRepository _workerRepository;

    public TargetWarehouseManagerApproverStrategy(
        IMovementRequestRepository movementRequestRepository,
        IWarehouseRepository WarehouseRepository,
        IWorkerRepository workerRepository)
    {
        _movementRequestRepository = movementRequestRepository;
        _WarehouseRepository = WarehouseRepository;
        _workerRepository = workerRepository;
    }

    public string Key => WorkflowResolverKeys.TargetWarehouseManager;

    public Task<Guid?> ResolveAsync(ApproverContext context)
        => WarehouseApproverResolver.ResolveAsync(
            context, useSourceWarehouse: false,
            _movementRequestRepository, _WarehouseRepository, _workerRepository);
}
