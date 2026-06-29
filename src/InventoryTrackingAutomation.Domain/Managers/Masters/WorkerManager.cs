using AutoMapper;
using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Çalışan domain manager'ı — Worker entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Worker etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class WorkerManager : BaseManager<Worker>
{
    private IDepartmentRepository _departmentRepository => LazyGetRequiredService<IDepartmentRepository>();  // DepartmentId FK validasyonu için
    private IWarehouseRepository _warehouseRepository => LazyGetRequiredService<IWarehouseRepository>();              // DefaultWarehouseId FK validasyonu için
    private IRepository<WorkerType, Guid> _workerTypeRepository => LazyGetRequiredService<IRepository<WorkerType, Guid>>();

    /// <summary>
    /// WorkerManager constructor'ı.
    /// </summary>
    private IMapper _mapper => LazyGetRequiredService<IMapper>();
    public WorkerManager(IWorkerRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
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

        // islevi: Enum yerine gelen WorkerType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_workerTypeRepository, model.WorkerTypeId);

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

        // islevi: Enum yerine gelen WorkerType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_workerTypeRepository, model.WorkerTypeId);

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

    /// <summary>
    /// Calisani operasyon gecmisi bozulmadan pasife almak icin kullanilir.
    /// </summary>
    // islevi: Master calisan kaydini silmeden gorev, talep ve onaylarda kullanima kapatir.
    // sistemdeki gorevi: Worker baglantili operasyonel gecmisi koruyarak soft-delete kolonlarina olan ihtiyaci kaldirir.
    public Task<Worker> PassivateAsync(Worker existing)
    {
        existing.IsActive = false;
        return Task.FromResult(existing);
    }
}

