using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// VehicleTask domain manager'i - arac gorev atama kurallarini yonetir.
/// </summary>
//işlevi: Araçların saha görevlerine (InventoryTask) atanması ve bu atamaların geçerliliğini (çakışma kontrolü vb.) yönetir.
//sistemdeki görevii: Araç-Görev eşleşmelerinin bütünlüğünü sağlar; bir aracın aynı anda birden fazla görevde aktif olmasını engeller.
//işlevi: VehicleTask etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class VehicleTaskManager : BaseManager<VehicleTask>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IInventoryTaskRepository _inventoryTaskRepository;
    private readonly IMapper _mapper;

    public VehicleTaskManager(
        IVehicleTaskRepository repository,
        IVehicleRepository vehicleRepository,
        IInventoryTaskRepository inventoryTaskRepository,
        IMapper mapper)
        : base(repository)
    {
        _vehicleRepository = vehicleRepository;
        _inventoryTaskRepository = inventoryTaskRepository;
        _mapper = mapper;
    }

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<VehicleTask> CreateAsync(CreateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.InventoryTaskId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, null);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        var entity = new VehicleTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<VehicleTask> UpdateAsync(VehicleTask existing, UpdateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.InventoryTaskId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, existing.Id);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        _mapper.Map(model, existing);
        return existing;
    }

    private async Task ValidateReferencesAsync(Guid vehicleId, Guid inventoryTaskId)
    {
        // Arac ve gorev varligi domain katmaninda dogrulanir.
        await EnsureExistsInAsync<Vehicle>(_vehicleRepository, vehicleId);
        await EnsureExistsInAsync(_inventoryTaskRepository, inventoryTaskId);
    }

    private async Task ValidateVehicleActiveTaskAsync(Guid vehicleId, Guid? excludeId)
    {
        // Ayni arac ayni anda yalnizca bir aktif gorevde bulunabilir.
        var activeVehicleTasks = await Repository.GetListAsync(x =>
            x.VehicleId == vehicleId &&
            x.IsActive &&
            !x.ReleasedAt.HasValue);

        if (activeVehicleTasks.Any(x => !excludeId.HasValue || x.Id != excludeId.Value))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTasks.VehicleAlreadyAssigned);
        }
    }

    private static void ValidateDateRange(DateTime assignedAt, DateTime? releasedAt)
    {
        // Birakma zamani atama zamanindan once olamaz.
        if (releasedAt.HasValue && releasedAt.Value < assignedAt)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    //işlevi: Aracın ilgili göreve atanmasını garantiler; zaten atanmışsa işlem yapmaz.
    //sistemdeki görevi: Stok transferi sırasında, eğer araca ürün yükleniyorsa o aracın göreve resmi olarak atanmış olmasını (tutarlılık) sağlar.
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task EnsureAssignedAsync(Guid inventoryTaskId, Guid vehicleId, Guid driverWorkerId)
    {
        var existing = await Repository.FindAsync(x => x.InventoryTaskId == inventoryTaskId && x.VehicleId == vehicleId && x.IsActive);
        if (existing != null)
            return;

        var entity = new VehicleTask(GuidGenerator.Create())
        {
            InventoryTaskId = inventoryTaskId,
            VehicleId = vehicleId,
            DriverWorkerId = driverWorkerId,
            AssignedAt = System.DateTime.UtcNow,
            IsActive = true
        };
        await Repository.InsertAsync(entity, autoSave: true);
    }

    //işlevi: Bir göreve bağlı tüm aktif araç atamalarını sonlandırır.
    //sistemdeki görevi: Task tamamlandığında veya iptal edildiğinde araçların üzerindeki bağları (kilitleri) kaldırıp onları serbest bırakır.
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task ReleaseAllForTaskAsync(Guid taskId)
    {
        var actives = await Repository.GetListAsync(x => x.InventoryTaskId == taskId && x.IsActive);
        foreach (var vt in actives)
        {
            vt.ReleasedAt = System.DateTime.UtcNow;
            vt.IsActive = false;
            await Repository.UpdateAsync(vt, autoSave: true);
        }
    }
}
