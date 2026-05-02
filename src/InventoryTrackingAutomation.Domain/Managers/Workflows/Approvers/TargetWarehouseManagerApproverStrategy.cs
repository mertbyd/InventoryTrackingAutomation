using System;
using InventoryTrackingAutomation.Managers;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Workflows;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Hareket talebinin hedef (Target) lokasyonunun yöneticisini onaycı olarak çözer.
// Şu an sadece MovementRequest tipinde aktif; başka entity tipleri için ayrı bir strategy eklenmeli.
//işlevi: TargetWarehouseManagerApproverStrategy.cs etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class TargetWarehouseManagerApproverStrategy : InventoryTrackingAutomationLazyService, IApproverStrategy, ITransientDependency
{
    public TargetWarehouseManagerApproverStrategy(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IMovementRequestRepository _movementRequestRepository => LazyGetRequiredService<IMovementRequestRepository>();
    private IWarehouseRepository _WarehouseRepository => LazyGetRequiredService<IWarehouseRepository>();
    private IWorkerRepository _workerRepository => LazyGetRequiredService<IWorkerRepository>();



    public string Key => WorkflowResolverKeys.TargetWarehouseManager;

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public Task<Guid?> ResolveAsync(ApproverContext context)
        => WarehouseApproverResolver.ResolveAsync(
            context, useSourceWarehouse: false,
            _movementRequestRepository, _WarehouseRepository, _workerRepository);
}
