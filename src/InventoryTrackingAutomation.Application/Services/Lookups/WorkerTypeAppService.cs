using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using InventoryTrackingAutomation.Application.Mappers.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// islevi: WorkerType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Calisan tipi referans verisi icin validation, manager ve repository akisini base sinif uzerinden yurutur.
public class WorkerTypeAppService : LookupCrudAppService<WorkerType, WorkerTypeDto, CreateWorkerTypeDto, UpdateWorkerTypeDto, CreateWorkerTypeModel, UpdateWorkerTypeModel>, IWorkerTypeAppService
{
    private WorkerTypeManager _manager => LazyGetRequiredService<WorkerTypeManager>();
    private static readonly WorkerTypeMapper _mapper = new WorkerTypeMapper();

    public WorkerTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IWorkerTypeRepository repository)
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override WorkerTypeDto MapToDto(WorkerType entity) => _mapper.MapToDto(entity);
    protected override List<WorkerTypeDto> MapToDto(List<WorkerType> entities) => _mapper.MapToDto(entities);
    protected override CreateWorkerTypeModel MapToCreateModel(CreateWorkerTypeDto input) => _mapper.MapToModel(input);
    protected override UpdateWorkerTypeModel MapToUpdateModel(UpdateWorkerTypeDto input) => _mapper.MapToModel(input);

    protected override Task<WorkerType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);

    protected override Task<CreateWorkerTypeModel> CreateModelAsync(CreateWorkerTypeModel model) =>
        _manager.CreateAsync(model);

    protected override Task<List<CreateWorkerTypeModel>> CreateModelsAsync(List<CreateWorkerTypeModel> models) =>
        _manager.CreateManyAsync(models);

    protected override WorkerType CreateEntity(CreateWorkerTypeModel model)
    {
        var entity = new WorkerType(GuidGenerator.Create(), model.Code, model.Name);
        _mapper.MapToEntity(model, entity);
        return entity;
    }

    protected override async Task<WorkerType> UpdateEntityAsync(WorkerType entity, UpdateWorkerTypeModel model)
    {
        var validatedModel = await _manager.UpdateAsync(entity, model);
        _mapper.MapToEntity(validatedModel, entity);
        return entity;
    }
}

