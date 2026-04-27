using AutoMapper;
using System;
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
    private readonly IMapper _mapper;
    public WorkerManager(
        IWorkerRepository repository,
        IDepartmentRepository departmentRepository,
        ISiteRepository siteRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
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
            model.DepartmentId);

        await EnsureExistsInAsync(
            _siteRepository,
            model.DefaultSiteId);

        await EnsureExistsInAsync(
            Repository,
            model.ManagerId);

        await EnsureValidEnumAsync(model.WorkerType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes);

        var entity = new Worker(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Çalışanı günceller — DepartmentId, DefaultSiteId ve ManagerId varlık kontrolleri ile kendi kendini yönetici atamama kurallarını işletir.
    /// </summary>
    public async Task<Worker> UpdateAsync(Worker existing, UpdateWorkerModel model)
    {
        EnsureNotSelfAssigned(existing.Id, model.ManagerId);

        await EnsureExistsInAsync(
            _departmentRepository,
            model.DepartmentId);

        await EnsureExistsInAsync(
            _siteRepository,
            model.DefaultSiteId);

        await EnsureExistsInAsync(
            Repository,
            model.ManagerId);

        await EnsureValidEnumAsync(model.WorkerType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes);

        _mapper.Map(model, existing);
        return existing;
    }

    private void EnsureNotSelfAssigned(Guid existingWorkerId, Guid? assignedManagerId)
    {
        if (assignedManagerId.HasValue && assignedManagerId.Value == existingWorkerId)
        {
            throw new Volo.Abp.BusinessException(InventoryTrackingAutomationErrorCodes.Workers.SelfAssignmentNotAllowed);
        }
    }

    // Verilen kullanıcının yöneticisinin User Id'sini Worker zinciri üzerinden çözer (User → Worker → Manager Worker → Manager User).
    // Worker, Manager veya Manager User yoksa null döner. Workflow ve onay zinciri tüm tüketicilerinde tek doğruluk kaynağı.
    public async Task<Guid?> GetManagerUserIdAsync(Guid userId)
    {
        var worker = await Repository.FindAsync(w => w.UserId == userId);
        if (worker?.ManagerId == null)
        {
            return null;
        }

        var managerWorker = await Repository.FindAsync(worker.ManagerId.Value);
        return managerWorker?.UserId;
    }
}

