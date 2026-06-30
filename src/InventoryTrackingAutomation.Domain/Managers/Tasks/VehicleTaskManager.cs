using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

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
    private IVehicleRepository _vehicleRepository => LazyGetRequiredService<IVehicleRepository>();
    private IInventoryTaskRepository _inventoryTaskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private IWorkerRepository _workerRepository => LazyGetRequiredService<IWorkerRepository>();

    public VehicleTaskManager(IVehicleTaskRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// Yeni bir araç görev ataması oluşturmak için kullanılır.
    public async Task<CreateVehicleTaskModel> CreateAsync(CreateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.TaskId, model.ResponsibleWorkerId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, null);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        return model;
    }

    /// Mevcut bir araç görev atamasını güncellemek için kullanılır.
    public async Task<UpdateVehicleTaskModel> UpdateAsync(VehicleTask existing, UpdateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.TaskId, model.ResponsibleWorkerId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, existing.Id);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        return model;
    }

    /// Atama referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(Guid vehicleId, Guid taskId, Guid responsibleWorkerId)
    {
        // Arac, operasyon isi ve sorumlu calisan varligi repository uzerinden dogrulanir.
        await EnsureExistsInAsync<Vehicle>(_vehicleRepository, vehicleId);
        await EnsureExistsInAsync(_inventoryTaskRepository, taskId);
        await EnsureExistsInAsync(_workerRepository, responsibleWorkerId);
    }

    /// Aracın aktif görev durumunu doğrulamak için kullanılır.
    private async Task ValidateVehicleActiveTaskAsync(Guid vehicleId, Guid? excludeId)
    {
        // Ayni arac ayni anda yalnizca bir aktif gorevde bulunabilir.
        var activeVehicleTasks = await Repository.GetListAsync(x =>
            x.VehicleId == vehicleId &&
            !x.ReleasedAt.HasValue);

        if (activeVehicleTasks.Any(x => !excludeId.HasValue || x.Id != excludeId.Value))
        {
            throw new BusinessException(VehicleTaskExceptionCodes.VehicleAlreadyAssigned);
        }
    }

    /// Atama zaman aralığını doğrulamak için kullanılır.
    private static void ValidateDateRange(DateTime assignedAt, DateTime? releasedAt)
    {
        // Birakma zamani atama zamanindan once olamaz.
        if (releasedAt.HasValue && releasedAt.Value < assignedAt)
        {
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
        }
    }

    /// Aracın göreve atanmış olmasını garantilemek için kullanılır.
    public async Task<VehicleTask> EnsureAssignedAsync(Guid taskId, Guid vehicleId, Guid responsibleWorkerId)
    {
        await ValidateReferencesAsync(vehicleId, taskId, responsibleWorkerId);

        var existing = await Repository.FindAsync(x =>
            x.TaskId == taskId &&
            x.VehicleId == vehicleId &&
            !x.ReleasedAt.HasValue);
        if (existing != null)
        {
            return existing;
        }
        await ValidateVehicleActiveTaskAsync(vehicleId, null);
        var entity = new VehicleTask(GuidGenerator.Create())
        {
            TaskId = taskId,
            VehicleId = vehicleId,
            ResponsibleWorkerId = responsibleWorkerId,
            AssignedAt = System.DateTime.UtcNow,
        };
        return await Repository.InsertAsync(entity, autoSave: true);
    }

    /// Göreve bağlı tüm araçları serbest bırakmak için kullanılır.
    public async Task ReleaseAllForTaskAsync(Guid taskId)
    {
        var actives = await Repository.GetListAsync(x => x.TaskId == taskId && !x.ReleasedAt.HasValue);
        foreach (var vt in actives)
        {
            vt.ReleasedAt = System.DateTime.UtcNow;
            await Repository.UpdateAsync(vt, autoSave: true);
        }
    }

    /// Göreve bağlı belirli bir aracı serbest bırakmak için kullanılır.
    public async Task ReleaseForTaskVehicleAsync(Guid taskId, Guid vehicleId)
    {
        var actives = await Repository.GetListAsync(x =>
            x.TaskId == taskId &&
            x.VehicleId == vehicleId &&
            !x.ReleasedAt.HasValue);

        foreach (var vt in actives)
        {
            vt.ReleasedAt = System.DateTime.UtcNow;
            await Repository.UpdateAsync(vt, autoSave: true);
        }
    }
}
