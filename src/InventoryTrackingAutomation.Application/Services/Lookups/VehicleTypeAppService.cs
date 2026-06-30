using System;
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

// islevi: VehicleType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Arac tipi referans verisi icin validation, manager ve repository akisini base sinif uzerinden yurutur.
public class VehicleTypeAppService : LookupCrudAppService<VehicleType, VehicleTypeDto, CreateVehicleTypeDto, UpdateVehicleTypeDto, CreateVehicleTypeModel, UpdateVehicleTypeModel>, IVehicleTypeAppService
{
    private VehicleTypeManager _manager => LazyGetRequiredService<VehicleTypeManager>();
    private static readonly VehicleTypeMapper _mapper = new VehicleTypeMapper();

    public VehicleTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IVehicleTypeRepository repository)
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override VehicleTypeDto MapToDto(VehicleType entity) => _mapper.MapToDto(entity);
    protected override System.Collections.Generic.List<VehicleTypeDto> MapToDto(System.Collections.Generic.List<VehicleType> entities) => _mapper.MapToDto(entities);
    protected override CreateVehicleTypeModel MapToCreateModel(CreateVehicleTypeDto input) => _mapper.MapToModel(input);
    protected override UpdateVehicleTypeModel MapToUpdateModel(UpdateVehicleTypeDto input) => _mapper.MapToModel(input);

    protected override Task<VehicleType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);

    protected override async Task<VehicleType> CreateEntityAsync(CreateVehicleTypeModel model)
    {
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new VehicleType(GuidGenerator.Create(), validatedModel.Code, validatedModel.Name);
        _mapper.MapToEntity(validatedModel, entity);
        return entity;
    }

    protected override async Task<VehicleType> UpdateEntityAsync(VehicleType entity, UpdateVehicleTypeModel model)
    {
        var validatedModel = await _manager.UpdateAsync(entity, model);
        _mapper.MapToEntity(validatedModel, entity);
        return entity;
    }
}