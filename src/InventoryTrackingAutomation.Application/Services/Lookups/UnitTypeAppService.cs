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

public class UnitTypeAppService : LookupCrudAppService<UnitType, UnitTypeDto, CreateUnitTypeDto, UpdateUnitTypeDto, CreateUnitTypeModel, UpdateUnitTypeModel>, IUnitTypeAppService
{
    private UnitTypeManager _manager => LazyGetRequiredService<UnitTypeManager>();

    public UnitTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider, IUnitTypeRepository repository) 
        : base(abpLazyServiceProvider, repository)
    {
    }

    protected override Task<UnitType> EnsureExistsAsync(Guid id) => _manager.EnsureExistsAsync(id);
    protected override Task<UnitType> CreateEntityAsync(CreateUnitTypeModel model) => _manager.CreateAsync(model);
    protected override Task<UnitType> UpdateEntityAsync(UnitType entity, UpdateUnitTypeModel model) => _manager.UpdateAsync(entity, model);
}