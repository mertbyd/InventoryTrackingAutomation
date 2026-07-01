using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: WorkerType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Calisan tipi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class WorkerTypeManager : BaseManager<WorkerType>
{
    protected override string AlreadyExistsErrorCode => WorkerTypeExceptionCodes.AlreadyExists;
    private static readonly Expression<Func<WorkerType, string>> CodeSelector = x => x.Code;

    public WorkerTypeManager(
        IWorkerTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public Task<CreateWorkerTypeModel> CreateAsync(CreateWorkerTypeModel model) =>
        EnsureUniqueCodeForCreateAsync(model, x => x.Code, CodeSelector);

    /// <summary>
    /// Toplu calisan tipi olusturma icin ortak BaseManager Code benzersizlik kontrolunu kullanir.
    /// </summary>
    public Task<List<CreateWorkerTypeModel>> CreateManyAsync(List<CreateWorkerTypeModel> models) =>
        EnsureUniqueCodesForCreateManyAsync(models, x => x.Code, CodeSelector);

    public Task<UpdateWorkerTypeModel> UpdateAsync(WorkerType existing, UpdateWorkerTypeModel model) =>
        EnsureUniqueCodeForUpdateAsync(existing, model, x => x.Code, x => x.Code, CodeSelector);
}
