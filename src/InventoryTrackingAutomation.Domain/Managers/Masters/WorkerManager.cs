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
//işlevi: Worker etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class WorkerManager : BaseManager<Worker>
{
    private readonly IDepartmentRepository _departmentRepository;  // DepartmentId FK validasyonu için
    private readonly IWarehouseRepository _warehouseRepository;              // DefaultWarehouseId FK validasyonu için

    /// <summary>
    /// WorkerManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public WorkerManager(
        IWorkerRepository repository,
        IDepartmentRepository departmentRepository,
        IWarehouseRepository warehouseRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _departmentRepository = departmentRepository;
        _warehouseRepository = warehouseRepository;
    }

    /// <summary>
    /// Yeni çalışan oluşturur — DepartmentId, DefaultWarehouseId ve ManagerId varlık kontrolleri yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Worker> CreateAsync(CreateWorkerModel model)
    {
        await EnsureExistsInAsync(
            _departmentRepository,
            model.DepartmentId);

        await EnsureExistsInAsync(
            _warehouseRepository,
            model.DefaultWarehouseId);

        await EnsureExistsInAsync(
            Repository,
            model.ManagerId);

        await EnsureValidEnumAsync(model.WorkerType, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedWorkerTypes);

        var entity = new Worker(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Çalışanı günceller — DepartmentId, DefaultWarehouseId ve ManagerId varlık kontrolleri ile kendi kendini yönetici atamama kurallarını işletir.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Worker> UpdateAsync(Worker existing, UpdateWorkerModel model)
    {
        EnsureNotSelfAssigned(existing.Id, model.ManagerId);

        await EnsureExistsInAsync(
            _departmentRepository,
            model.DepartmentId);

        await EnsureExistsInAsync(
            _warehouseRepository,
            model.DefaultWarehouseId);

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
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
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

