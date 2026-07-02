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

// islevi: UnitType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Olcu birimi referans verisi icin validation, manager ve repository akisini base sinif uzerinden yurutur.
public class UnitTypeAppService : LookupCrudAppService<UnitType, UnitTypeDto, CreateUnitTypeDto, UpdateUnitTypeDto, CreateUnitTypeModel, UpdateUnitTypeModel>, IUnitTypeAppService
{
    private UnitTypeManager _manager => LazyGetRequiredService<UnitTypeManager>();
    private static readonly UnitTypeMapper _mapper = new UnitTypeMapper();

    public UnitTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IUnitTypeRepository repository)
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override UnitTypeDto MapToDto(UnitType entity) => _mapper.MapToDto(entity);
    protected override List<UnitTypeDto> MapToDto(List<UnitType> entities) => _mapper.MapToDto(entities);
    protected override CreateUnitTypeModel MapToCreateModel(CreateUnitTypeDto input) => _mapper.MapToModel(input);
    protected override UpdateUnitTypeModel MapToUpdateModel(UpdateUnitTypeDto input) => _mapper.MapToModel(input);

    protected override Task<UnitType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);

    protected override Task<CreateUnitTypeModel> CreateModelAsync(CreateUnitTypeModel model) =>
        _manager.CreateAsync(model);

    protected override Task<List<CreateUnitTypeModel>> CreateModelsAsync(List<CreateUnitTypeModel> models) =>
        _manager.CreateManyAsync(models);

    protected override UnitType CreateEntity(CreateUnitTypeModel model)
    {
        var entity = new UnitType(GuidGenerator.Create(), model.Code, model.Name);
        _mapper.MapToEntity(model, entity);
        return entity;
    }

    protected override async Task<UnitType> UpdateEntityAsync(UnitType entity, UpdateUnitTypeModel model)
    {
        var validatedModel = await _manager.UpdateAsync(entity, model);
        _mapper.MapToEntity(validatedModel, entity);
        return entity;
    }
}

