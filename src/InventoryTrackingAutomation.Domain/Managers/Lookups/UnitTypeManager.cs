using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
    private static readonly Expression<Func<UnitType, string>> CodeSelector = x => x.Code;

    public UnitTypeManager(
        IUnitTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public Task<CreateUnitTypeModel> CreateAsync(CreateUnitTypeModel model) =>
        EnsureUniqueCodeForCreateAsync(model, x => x.Code, CodeSelector);

    /// <summary>
    /// Toplu olcu birimi olusturma icin ortak BaseManager Code benzersizlik kontrolunu kullanir.
    /// </summary>
    public Task<List<CreateUnitTypeModel>> CreateManyAsync(List<CreateUnitTypeModel> models) =>
        EnsureUniqueCodesForCreateManyAsync(models, x => x.Code, CodeSelector);

    public Task<UpdateUnitTypeModel> UpdateAsync(UnitType existing, UpdateUnitTypeModel model) =>
        EnsureUniqueCodeForUpdateAsync(existing, model, x => x.Code, x => x.Code, CodeSelector);
}
