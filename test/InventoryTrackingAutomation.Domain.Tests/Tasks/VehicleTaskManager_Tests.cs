using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Xunit;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Tasks;

/*
 * DOMAIN KATMANI TESTLERİ — VehicleTaskManager iş kuralları.
 *
 * Bu testler araç-görev atama sürecinin iş kurallarını test eder:
 * - Bir araç aynı anda birden fazla aktif görevde olamaz.
 * - Release edildikten sonra yeniden atanabilir.
 * - Var olmayan referanslar (Vehicle, Task, Worker) hata vermeli.
 */
public abstract class VehicleTaskManager_Tests<TStartupModule> : InventoryTrackingAutomationDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly VehicleTaskManager _manager;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly IRepository<Vehicle, Guid> _vehicleRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IInventoryTaskRepository _taskRepository;
    private readonly IRepository<Worker, Guid> _workerRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid> _vehicleTypeRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid> _workerTypeRepository;

    protected VehicleTaskManager_Tests()
    {
        _manager = GetRequiredService<VehicleTaskManager>();
        _vehicleTaskRepository = GetRequiredService<IVehicleTaskRepository>();
        _vehicleRepository = GetRequiredService<IRepository<Vehicle, Guid>>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _taskRepository = GetRequiredService<IInventoryTaskRepository>();
        _workerRepository = GetRequiredService<IRepository<Worker, Guid>>();
        _vehicleTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid>>();
        _workerTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid>>();
    }

    // ───────────────────────────────────────────────
    //  Araç Çakışma Kuralı
    // ───────────────────────────────────────────────

    /*
     * SENARYO 1: Aynı araç zaten aktif bir görevdeyken, ikinci bir göreve atanamaz.
     * Kural: "Bir arac ayni anda birden fazla aktif gorevde olamaz."
     */
    [Fact]
    public async Task EnsureAssigned_Should_Throw_When_Vehicle_Already_Active_On_Another_Task()
    {
        var sourceWarehouse = await InsertWarehouseAsync("VT-SRC-001");
        var targetWarehouse = await InsertWarehouseAsync("VT-TRG-001");

        var vehicleType = await _vehicleTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.VehicleType(Guid.NewGuid(), "FORKLIFT", "Forklift"), autoSave: true);
        var workerType = await _workerTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.WorkerType(Guid.NewGuid(), "DRIVER", "Driver"), autoSave: true);

        // Test verileri oluştur.
        var vehicle = await _vehicleRepository.InsertAsync(new Vehicle(Guid.NewGuid())
        {
            PlateNumber = "34-VT-001",
            VehicleTypeId = vehicleType.Id,
            IsActive = true
        }, autoSave: true);

        var worker = await _workerRepository.InsertAsync(new Worker(Guid.NewGuid())
        {
            UserId = Guid.NewGuid(),
            RegistrationNumber = "VT-WRK-001",
            WorkerTypeId = workerType.Id,
            IsActive = true
        }, autoSave: true);

        var task1 = await _taskRepository.InsertAsync(new InventoryTask(Guid.NewGuid())
        {
            Code = "VT-TSK-001",
            Name = "First Task",
            Type = InventoryTaskTypeEnum.FieldOperation,
            Status = TaskStatusEnum.InProgress,
            SourceWarehouseId = sourceWarehouse.Id,
            StartDate = DateTime.UtcNow
        }, autoSave: true);

        var task2 = await _taskRepository.InsertAsync(new InventoryTask(Guid.NewGuid())
        {
            Code = "VT-TSK-002",
            Name = "Second Task",
            Type = InventoryTaskTypeEnum.WarehouseTransfer,
            Status = TaskStatusEnum.Draft,
            SourceWarehouseId = sourceWarehouse.Id,
            TargetWarehouseId = targetWarehouse.Id,
            StartDate = DateTime.UtcNow
        }, autoSave: true);

        // Aracı ilk göreve ata — başarılı olmalı.
        await _manager.EnsureAssignedAsync(task1.Id, vehicle.Id, worker.Id);

        // AYNI ARACI ikinci göreve atamaya çalış — HATA BEKLİYORUZ!
        await Assert.ThrowsAsync<BusinessException>(async () =>
        {
            await _manager.EnsureAssignedAsync(task2.Id, vehicle.Id, worker.Id);
        });
    }

    // ───────────────────────────────────────────────
    //  Release (Serbest Bırakma) Kuralı
    // ───────────────────────────────────────────────

    /*
     * SENARYO 2: Araç serbest bırakıldıktan (Release) sonra başka bir göreve atanabilmeli.
     */
    [Fact]
    public async Task Vehicle_Should_Be_Assignable_After_Release()
    {
        var sourceWarehouse = await InsertWarehouseAsync("VT-SRC-002");
        var targetWarehouse = await InsertWarehouseAsync("VT-TRG-002");

        var vehicleType = await _vehicleTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.VehicleType(Guid.NewGuid(), "FORKLIFT2", "Forklift 2"), autoSave: true);
        var workerType = await _workerTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.WorkerType(Guid.NewGuid(), "DRIVER2", "Driver 2"), autoSave: true);

        var vehicle = await _vehicleRepository.InsertAsync(new Vehicle(Guid.NewGuid())
        {
            PlateNumber = "34-VT-002",
            VehicleTypeId = vehicleType.Id,
            IsActive = true
        }, autoSave: true);

        var worker = await _workerRepository.InsertAsync(new Worker(Guid.NewGuid())
        {
            UserId = Guid.NewGuid(),
            RegistrationNumber = "VT-WRK-002",
            WorkerTypeId = workerType.Id,
            IsActive = true
        }, autoSave: true);

        var task1 = await _taskRepository.InsertAsync(new InventoryTask(Guid.NewGuid())
        {
            Code = "VT-REL-001",
            Name = "Release Test Task 1",
            Type = InventoryTaskTypeEnum.FieldOperation,
            Status = TaskStatusEnum.InProgress,
            SourceWarehouseId = sourceWarehouse.Id,
            StartDate = DateTime.UtcNow
        }, autoSave: true);

        var task2 = await _taskRepository.InsertAsync(new InventoryTask(Guid.NewGuid())
        {
            Code = "VT-REL-002",
            Name = "Release Test Task 2",
            Type = InventoryTaskTypeEnum.WarehouseTransfer,
            Status = TaskStatusEnum.Draft,
            SourceWarehouseId = sourceWarehouse.Id,
            TargetWarehouseId = targetWarehouse.Id,
            StartDate = DateTime.UtcNow
        }, autoSave: true);

        // Aracı ilk göreve ata.
        var assignment = await _manager.EnsureAssignedAsync(task1.Id, vehicle.Id, worker.Id);

        // Aracı ilk görevden serbest bırak.
        await _manager.ReleaseForTaskVehicleAsync(task1.Id, vehicle.Id);

        // Şimdi aracı İKİNCİ göreve atayabilmeli — HATA VERMEMELİ.
        var secondAssignment = await _manager.EnsureAssignedAsync(task2.Id, vehicle.Id, worker.Id);

        Assert.NotNull(secondAssignment);
        Assert.Equal(task2.Id, secondAssignment.TaskId);
    }

    private async Task<Warehouse> InsertWarehouseAsync(string prefix)
    {
        return await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"{prefix}-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} depo",
            IsActive = true
        }, autoSave: true);
    }
}

