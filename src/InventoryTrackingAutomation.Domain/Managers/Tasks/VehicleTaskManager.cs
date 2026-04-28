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

    public async Task<VehicleTask> CreateAsync(CreateVehicleTaskModel model)
    {
        await ValidateReferencesAsync(model.VehicleId, model.InventoryTaskId);
        await ValidateVehicleActiveTaskAsync(model.VehicleId, null);
        ValidateDateRange(model.AssignedAt, model.ReleasedAt);

        var entity = new VehicleTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

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
}
