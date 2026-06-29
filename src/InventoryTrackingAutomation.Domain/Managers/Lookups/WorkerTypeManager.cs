using System.Threading.Tasks;
using AutoMapper;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

// islevi: WorkerType icin domain kurallarini ve veri butunlugu kontrollerini uygular.
// sistemdeki gorevi: Calisan tipi lookup kayitlarinda Code benzersizligini merkezi olarak garanti eder.
public class WorkerTypeManager : BaseManager<WorkerType>
{
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public WorkerTypeManager(
        IWorkerTypeRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    public async Task<WorkerType> CreateAsync(CreateWorkerTypeModel model)
    {
        await EnsureUniqueAsync(x => x.Code == model.Code);

        var entity = new WorkerType(GuidGenerator.Create(), model.Code, model.Name);
        _mapper.Map(model, entity);
        return entity;
    }

    public async Task<WorkerType> UpdateAsync(WorkerType existing, UpdateWorkerTypeModel model)
    {
        if (existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        _mapper.Map(model, existing);
        return existing;
    }
}
