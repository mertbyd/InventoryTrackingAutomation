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

    public async Task<InventoryTask> CreateAsync(CreateInventoryTaskModel model)
    {
        await ValidateCodeForCreateAsync(model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);

        var entity = new InventoryTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

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
}
