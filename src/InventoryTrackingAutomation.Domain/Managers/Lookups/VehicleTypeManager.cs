using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: VehicleType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Arac tipi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class VehicleTypeManager : BaseManager<VehicleType>
{
    protected override string AlreadyExistsErrorCode => VehicleTypeExceptionCodes.AlreadyExists;

    public VehicleTypeManager(
        IVehicleTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<CreateVehicleTypeModel> CreateAsync(CreateVehicleTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);
        return model;
    }

    /// Toplu arac tipi olusturma kurallarini tek benzersizlik sorgusuyla uygular.
    public async Task<List<CreateVehicleTypeModel>> CreateManyAsync(List<CreateVehicleTypeModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Count > 0)
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }

        return models;
    }

    public async Task<UpdateVehicleTypeModel> UpdateAsync(VehicleType existing, UpdateVehicleTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        return model;
    }
}
