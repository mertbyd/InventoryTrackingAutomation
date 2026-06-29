using System.Threading.Tasks;
using AutoMapper;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: UnitType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Olcu birimi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class UnitTypeManager : BaseManager<UnitType>
{
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public UnitTypeManager(
        IUnitTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<UnitType> CreateAsync(CreateUnitTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);

        var entity = new UnitType(GuidGenerator.Create(), model.Code, model.Name);
        _mapper.Map(model, entity);
        return entity;
    }

    public async Task<UnitType> UpdateAsync(UnitType existing, UpdateUnitTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        _mapper.Map(model, existing);
        return existing;
    }
}
