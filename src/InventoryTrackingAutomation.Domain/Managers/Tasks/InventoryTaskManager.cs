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

    /// Yeni bir envanter görevi oluşturmak için kullanılır.
    public async Task<InventoryTask> CreateAsync(CreateInventoryTaskModel model)
    {
        await ValidateCodeForCreateAsync(model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);

        var entity = new InventoryTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// Mevcut bir envanter görevini güncellemek için kullanılır.
    public async Task<InventoryTask> UpdateAsync(InventoryTask existing, UpdateInventoryTaskModel model)
    {
        await ValidateCodeForUpdateAsync(existing, model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);

        _mapper.Map(model, existing);
        return existing;
    }

    /// Görev kodunu oluşturma aşamasında doğrulamak için kullanılır.
    private async Task ValidateCodeForCreateAsync(string code)
    {
        // Gorev kodu benzersiz olmalidir.
        if (!string.IsNullOrWhiteSpace(code))
        {
            await EnsureUniqueAsync(x => x.Code == code);
        }
    }

    /// Görev kodunu güncelleme aşamasında doğrulamak için kullanılır.
    private async Task ValidateCodeForUpdateAsync(InventoryTask existing, string code)
    {
        // Gorev kodu degisiyorsa kendisi haric unique kontrolu yapilir.
        if (!string.IsNullOrWhiteSpace(code) && existing.Code != code)
        {
            await EnsureUniqueAsync(x => x.Code == code, existing.Id);
        }
    }

    /// Tarih aralığını doğrulamak için kullanılır.
    private static void ValidateDateRange(System.DateTime startDate, System.DateTime? endDate)
    {
        // Bitis tarihi baslangictan once olamaz.
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    /// Görev durum geçişini gerçekleştirmek için kullanılır.
    public async Task TransitionStatusAsync(
        InventoryTask task,
        InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum target,
        Volo.Abp.EventBus.Local.ILocalEventBus localEventBus,
        System.Guid? changedByUserId = null,
        System.Guid? changedByWorkerId = null)
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
            NewStatus = target,
            ChangedByUserId = changedByUserId,
            ChangedByWorkerId = changedByWorkerId
        });
    }
}
