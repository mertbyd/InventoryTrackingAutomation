using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: UnitType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Olcu birimi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class UnitTypeManager : BaseManager<UnitType>
{


    public UnitTypeManager(
        IUnitTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<CreateUnitTypeModel> CreateAsync(CreateUnitTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);
        return model;
    }

    public async Task<UpdateUnitTypeModel> UpdateAsync(UnitType existing, UpdateUnitTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        return model;
    }
}
