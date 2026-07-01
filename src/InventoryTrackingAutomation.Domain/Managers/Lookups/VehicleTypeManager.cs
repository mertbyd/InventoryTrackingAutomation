using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    private static readonly Expression<Func<VehicleType, string>> CodeSelector = x => x.Code;

    public VehicleTypeManager(
        IVehicleTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public Task<CreateVehicleTypeModel> CreateAsync(CreateVehicleTypeModel model) =>
        EnsureUniqueCodeForCreateAsync(model, x => x.Code, CodeSelector);

    /// <summary>
    /// Toplu arac tipi olusturma icin ortak BaseManager Code benzersizlik kontrolunu kullanir.
    /// </summary>
    public Task<List<CreateVehicleTypeModel>> CreateManyAsync(List<CreateVehicleTypeModel> models) =>
        EnsureUniqueCodesForCreateManyAsync(models, x => x.Code, CodeSelector);

    public Task<UpdateVehicleTypeModel> UpdateAsync(VehicleType existing, UpdateVehicleTypeModel model) =>
        EnsureUniqueCodeForUpdateAsync(existing, model, x => x.Code, x => x.Code, CodeSelector);
}
