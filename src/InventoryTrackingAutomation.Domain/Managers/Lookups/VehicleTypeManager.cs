using System.Threading.Tasks;
using AutoMapper;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: VehicleType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Arac tipi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class VehicleTypeManager : BaseManager<VehicleType>
{
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public VehicleTypeManager(
        IVehicleTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<VehicleType> CreateAsync(CreateVehicleTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);

        var entity = new VehicleType(GuidGenerator.Create(), model.Code, model.Name);
        _mapper.Map(model, entity);
        return entity;
    }

    public async Task<VehicleType> UpdateAsync(VehicleType existing, UpdateVehicleTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        _mapper.Map(model, existing);
        return existing;
    }
}
