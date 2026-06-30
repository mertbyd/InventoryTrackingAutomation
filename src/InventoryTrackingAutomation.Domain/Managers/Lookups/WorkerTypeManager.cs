using System.Collections.Generic;
using System.Linq;
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

    public WorkerTypeManager(
        IWorkerTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<CreateWorkerTypeModel> CreateAsync(CreateWorkerTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);
        return model;
    }

    /// Toplu calisan tipi olusturma kurallarini tek benzersizlik sorgusuyla uygular.
    public async Task<List<CreateWorkerTypeModel>> CreateManyAsync(List<CreateWorkerTypeModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Count > 0)
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }

        return models;
    }

    public async Task<UpdateWorkerTypeModel> UpdateAsync(WorkerType existing, UpdateWorkerTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        return model;
    }
}
