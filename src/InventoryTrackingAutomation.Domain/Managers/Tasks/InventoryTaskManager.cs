using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// InventoryTask domain manager'i - gorev is kurallari ve validasyonlari.
/// </summary>
//işlevi: Saha görevlerinin (InventoryTask) yaşam döngüsünü, kod benzersizliğini ve tarih geçerliliğini yönetir.
//sistemdeki görevii: Operasyonel görevlerin oluşturulması ve güncellenmesi aşamasındaki iş kurallarını denetler.
//işlevi: InventoryTask etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class InventoryTaskManager : BaseManager<InventoryTask>
{
    private readonly IMapper _mapper;

    public InventoryTaskManager(
        IInventoryTaskRepository repository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
    }

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<InventoryTask> CreateAsync(CreateInventoryTaskModel model)
    {
        await ValidateCodeForCreateAsync(model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);

        var entity = new InventoryTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<InventoryTask> UpdateAsync(InventoryTask existing, UpdateInventoryTaskModel model)
    {
        await ValidateCodeForUpdateAsync(existing, model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);

        _mapper.Map(model, existing);
        return existing;
    }

    private async Task ValidateCodeForCreateAsync(string code)
    {
        // Gorev kodu benzersiz olmalidir.
        if (!string.IsNullOrWhiteSpace(code))
        {
            await EnsureUniqueAsync(x => x.Code == code);
        }
    }

    private async Task ValidateCodeForUpdateAsync(InventoryTask existing, string code)
    {
        // Gorev kodu degisiyorsa kendisi haric unique kontrolu yapilir.
        if (!string.IsNullOrWhiteSpace(code) && existing.Code != code)
        {
            await EnsureUniqueAsync(x => x.Code == code, existing.Id);
        }
    }

    private static void ValidateDateRange(System.DateTime startDate, System.DateTime? endDate)
    {
        // Bitis tarihi baslangictan once olamaz.
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    //işlevi: Görevin durum (status) geçişlerini (Draft → InProgress vb.) doğrular ve geçerli ise uygular, olay fırlatır.
    //sistemdeki görevi: Görev yaşam döngüsünün durum makinesi (state machine) kurallarını işletir.
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task TransitionStatusAsync(InventoryTask task, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum target, Volo.Abp.EventBus.Local.ILocalEventBus localEventBus)
    {
        if (task.Status == target) return;

        var previous = task.Status;

        var allowed = (task.Status, target) switch
        {
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Draft, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Completed) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Draft, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Cancelled) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
                .WithData("From", task.Status).WithData("To", target);

        task.Status = target;

        await localEventBus.PublishAsync(new InventoryTrackingAutomation.Events.Tasks.InventoryTaskStatusChangedEto
        {
            TaskId = task.Id,
            PreviousStatus = previous,
            NewStatus = target
        });
    }
}
