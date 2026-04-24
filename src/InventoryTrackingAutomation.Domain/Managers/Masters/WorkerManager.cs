using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Çalışan domain manager'ı — Worker entity'si için iş kuralları ve validasyonları.
/// </summary>
public class WorkerManager : BaseManager<Worker>
{
    private readonly IDepartmentRepository _departmentRepository;  // DepartmentId FK validasyonu için
    private readonly ISiteRepository _siteRepository;              // DefaultSiteId FK validasyonu için

    /// <summary>
    /// WorkerManager constructor'ı.
    /// </summary>
    public WorkerManager(
        IWorkerRepository repository,
        IDepartmentRepository departmentRepository,
        ISiteRepository siteRepository)
        : base(repository)
    {
        _departmentRepository = departmentRepository;
        _siteRepository = siteRepository;
    }

    /// <summary>
    /// Yeni çalışan oluşturur — DepartmentId, DefaultSiteId ve ManagerId varlık kontrolleri yapar.
    /// </summary>
    public async Task<Worker> CreateAsync(CreateWorkerModel model)
    {
        await EnsureExistsInAsync(
            _departmentRepository,
            model.DepartmentId,
            InventoryTrackingAutomationDomainErrorCodes.Departments.NotFound);

        await EnsureExistsInAsync(
            _siteRepository,
            model.DefaultSiteId,
            InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);

        await EnsureExistsInAsync(
            Repository,
            model.ManagerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        await EnsureValidEnumAsync(model.WorkerType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes);

        return MapAndAssignId(model);
    }

    /// <summary>
    /// Çalışanı günceller — DepartmentId, DefaultSiteId ve ManagerId varlık kontrolleri ile kendi kendini yönetici atamama kurallarını işletir.
    /// </summary>
    public async Task<Worker> UpdateAsync(Worker existing, UpdateWorkerModel model)
    {
        EnsureNotSelfAssigned(existing.Id, model.ManagerId);

        await EnsureExistsInAsync(
            _departmentRepository,
            model.DepartmentId,
            InventoryTrackingAutomationDomainErrorCodes.Departments.NotFound);

        await EnsureExistsInAsync(
            _siteRepository,
            model.DefaultSiteId,
            InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);

        await EnsureExistsInAsync(
            Repository,
            model.ManagerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        await EnsureValidEnumAsync(model.WorkerType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes);

        return MapForUpdate(model, existing);
    }

    private void EnsureNotSelfAssigned(System.Guid existingWorkerId, System.Guid? assignedManagerId)
    {
        if (assignedManagerId.HasValue && assignedManagerId.Value == existingWorkerId)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workers.SelfAssignmentNotAllowed);
        }
    }
}
