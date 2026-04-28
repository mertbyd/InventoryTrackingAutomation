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

    /// Yeni bir araç görev ataması oluşturmak için kullanılır.
    public async Task<VehicleTask> CreateAsync(CreateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.InventoryTaskId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, null);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        var entity = new VehicleTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// Mevcut bir araç görev atamasını güncellemek için kullanılır.
    public async Task<VehicleTask> UpdateAsync(VehicleTask existing, UpdateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.InventoryTaskId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, existing.Id);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        _mapper.Map(model, existing);
        return existing;
    }

    /// Atama referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(Guid vehicleId, Guid inventoryTaskId)
    {
        // Arac ve gorev varligi domain katmaninda dogrulanir.
        await EnsureExistsInAsync<Vehicle>(_vehicleRepository, vehicleId);
        await EnsureExistsInAsync(_inventoryTaskRepository, inventoryTaskId);
    }

    /// Aracın aktif görev durumunu doğrulamak için kullanılır.
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

    /// Atama zaman aralığını doğrulamak için kullanılır.
    private static void ValidateDateRange(DateTime assignedAt, DateTime? releasedAt)
    {
        // Birakma zamani atama zamanindan once olamaz.
        if (releasedAt.HasValue && releasedAt.Value < assignedAt)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    /// Aracın göreve atanmış olmasını garantilemek için kullanılır.
    public async Task EnsureAssignedAsync(Guid inventoryTaskId, Guid vehicleId, Guid driverWorkerId)
    {
        await ValidateReferencesAsync(vehicleId, inventoryTaskId);

        var existing = await Repository.FindAsync(x =>
            x.InventoryTaskId == inventoryTaskId &&
            x.VehicleId == vehicleId &&
            x.IsActive &&
            !x.ReleasedAt.HasValue);

        if (existing != null)
        {
            return;
        }

        await ValidateVehicleActiveTaskAsync(vehicleId, null);

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

    /// Göreve bağlı tüm araçları serbest bırakmak için kullanılır.
    public async Task ReleaseAllForTaskAsync(Guid taskId)
    {
        var actives = await Repository.GetListAsync(x => x.InventoryTaskId == taskId && x.IsActive);
        foreach (var vt in actives)
        {
            vt.IsActive = false;
            await Repository.UpdateAsync(vt, autoSave: true);
        }
    }

    /// Göreve bağlı belirli bir aracı serbest bırakmak için kullanılır.
    public async Task ReleaseForTaskVehicleAsync(Guid taskId, Guid vehicleId)
    {
        var actives = await Repository.GetListAsync(x =>
            x.InventoryTaskId == taskId &&
            x.VehicleId == vehicleId &&
            x.IsActive);

        foreach (var vt in actives)
        {
            vt.ReleasedAt = System.DateTime.UtcNow;
            vt.IsActive = false;
            await Repository.UpdateAsync(vt, autoSave: true);
        }
    }
}
