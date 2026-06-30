using System.Collections.Generic;
using System.Linq;
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
    protected override string AlreadyExistsErrorCode => UnitTypeExceptionCodes.AlreadyExists;

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

    /// Toplu olcu birimi olusturma kurallarini tek benzersizlik sorgusuyla uygular.
    public async Task<List<CreateUnitTypeModel>> CreateManyAsync(List<CreateUnitTypeModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Count > 0)
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }

        return models;
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
