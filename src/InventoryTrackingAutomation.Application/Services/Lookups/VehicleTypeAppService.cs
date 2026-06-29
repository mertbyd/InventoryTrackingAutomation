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

// islevi: VehicleType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Arac tipi referans verisi icin validation, manager ve repository akisini base sinif uzerinden yurutur.
public class VehicleTypeAppService : LookupCrudAppService<VehicleType, VehicleTypeDto, CreateVehicleTypeDto, UpdateVehicleTypeDto, CreateVehicleTypeModel, UpdateVehicleTypeModel>, IVehicleTypeAppService
{
    private VehicleTypeManager _manager => LazyGetRequiredService<VehicleTypeManager>();

    public VehicleTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IVehicleTypeRepository repository) 
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override Task<VehicleType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);
    protected override Task<VehicleType> CreateEntityAsync(CreateVehicleTypeModel model) => _manager.CreateAsync(model);
    protected override Task<VehicleType> UpdateEntityAsync(VehicleType entity, UpdateVehicleTypeModel model) => _manager.UpdateAsync(entity, model);
}