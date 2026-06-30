using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// islevi: WorkerType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Calisan tipi referans verisi icin validation, manager ve repository akisini base sinif uzerinden yurutur.
public class WorkerTypeAppService : LookupCrudAppService<WorkerType, WorkerTypeDto, CreateWorkerTypeDto, UpdateWorkerTypeDto, CreateWorkerTypeModel, UpdateWorkerTypeModel>, IWorkerTypeAppService
{
    private WorkerTypeManager _manager => LazyGetRequiredService<WorkerTypeManager>();

    public WorkerTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IWorkerTypeRepository repository) 
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override Task<WorkerType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);
    protected override Task<WorkerType> CreateEntityAsync(CreateWorkerTypeModel model) => _manager.CreateAsync(model);
    protected override Task<WorkerType> UpdateEntityAsync(WorkerType entity, UpdateWorkerTypeModel model) => _manager.UpdateAsync(entity, model);
}
