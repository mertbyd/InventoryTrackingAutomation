using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;
using InventoryTaskEntity = InventoryTrackingAutomation.Entities.Tasks.InventoryTask;

namespace InventoryTrackingAutomation.Movements;

public abstract class MovementRequestAppService_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IMovementRequestAppService _appService;
    private readonly IMovementRequestRepository _repository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Vehicle, Guid> _vehicleRepository;
    private readonly IRepository<Worker, Guid> _workerRepository;
    private readonly IRepository<InventoryTaskEntity, Guid> _taskRepository;
    private readonly IRepository<VehicleTask, Guid> _vehicleTaskRepository;

    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid> _vehicleTypeRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid> _workerTypeRepository;

    private sealed record MovementRequestSeed(MovementRequest Request, Worker Worker, InventoryTaskEntity Task);

    protected MovementRequestAppService_Tests()
    {
        _appService = GetRequiredService<IMovementRequestAppService>();
        _repository = GetRequiredService<IMovementRequestRepository>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _vehicleRepository = GetRequiredService<IRepository<Vehicle, Guid>>();
        _workerRepository = GetRequiredService<IRepository<Worker, Guid>>();
        _taskRepository = GetRequiredService<IRepository<InventoryTaskEntity, Guid>>();
        _vehicleTaskRepository = GetRequiredService<IRepository<VehicleTask, Guid>>();
        _vehicleTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid>>();
        _workerTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid>>();
    }

    [Fact]
    public async Task Should_Delete_Pending_Movement_Request()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var request = await InsertMovementRequestAsync(MovementStatusEnum.Pending, "DEL-PENDING");

            await _appService.DeleteAsync(request.Id);

            var deletedRequest = await _repository.FindAsync(request.Id);
            Assert.Null(deletedRequest);
        });
    }

    [Fact]
    public async Task GetAsync_Should_Fill_Display_References()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var seed = await InsertMovementRequestSeedAsync(MovementStatusEnum.Pending, "GET-ENRICH");

            var dto = await _appService.GetAsync(seed.Request.Id);

            AssertDisplayReferences(dto, seed);
        });
    }

    [Fact]
    public async Task GetListAsync_Should_Fill_Display_References()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var seed = await InsertMovementRequestSeedAsync(MovementStatusEnum.Pending, "LIST-ENRICH");

            var result = await _appService.GetListAsync(new PagedResultRequestDto
            {
                SkipCount = 0,
                MaxResultCount = 100
            });

            var dto = result.Items.First(item => item.Id == seed.Request.Id);

            AssertDisplayReferences(dto, seed);
        });
    }

    [Fact]
    public async Task Should_Not_Delete_Completed_Movement_Request()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var request = await InsertMovementRequestAsync(MovementStatusEnum.Completed, "DEL-COMPLETED");

            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _appService.DeleteAsync(request.Id);
            });

            Assert.Equal(MovementRequestExceptionCodes.InvalidStateTransition, exception.Code);
        });
    }

    private async Task<MovementRequest> InsertMovementRequestAsync(MovementStatusEnum status, string prefix)
    {
        return (await InsertMovementRequestSeedAsync(status, prefix)).Request;
    }

    private static void AssertDisplayReferences(MovementRequestDto dto, MovementRequestSeed seed)
    {
        Assert.True(dto.References.TryGetValue(nameof(MovementRequestDto.RequestedByWorkerId), out var workerReference));
        Assert.NotNull(workerReference);
        Assert.Equal(seed.Worker.Id, workerReference!.Id);
        Assert.Equal(seed.Worker.RegistrationNumber, workerReference.Code);
        Assert.Null(workerReference.Name);

        Assert.True(dto.References.TryGetValue(nameof(MovementRequestDto.TaskId), out var taskReference));
        Assert.NotNull(taskReference);
        Assert.Equal(seed.Task.Id, taskReference!.Id);
        Assert.Equal(seed.Task.Code, taskReference.Code);
        Assert.Equal(seed.Task.Name, taskReference.Name);

        Assert.False(dto.References.ContainsKey(nameof(MovementRequestDto.VehicleTaskId)));
    }

    private async Task<MovementRequestSeed> InsertMovementRequestSeedAsync(MovementStatusEnum status, string prefix)
    {
        var warehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"{prefix}-WH-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} depo",
            IsActive = true
        }, autoSave: true);

        var targetWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"{prefix}-TWH-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} hedef depo",
            IsActive = true
        }, autoSave: true);

        var vehicleType = await _vehicleTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.VehicleType(Guid.NewGuid(), $"{prefix}-VT", "VT"), autoSave: true);
        var workerType = await _workerTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.WorkerType(Guid.NewGuid(), $"{prefix}-WT", "WT"), autoSave: true);

        var vehicle = await _vehicleRepository.InsertAsync(new Vehicle(Guid.NewGuid())
        {
            PlateNumber = $"{prefix}-{Guid.NewGuid():N}"[..12],
            VehicleTypeId = vehicleType.Id,
            IsActive = true
        }, autoSave: true);

        var worker = await _workerRepository.InsertAsync(new Worker(Guid.NewGuid())
        {
            UserId = Guid.NewGuid(),
            RegistrationNumber = $"{prefix}-WRK-{Guid.NewGuid():N}"[..30],
            WorkerTypeId = workerType.Id,
            DefaultWarehouseId = warehouse.Id,
            IsActive = true
        }, autoSave: true);

        var task = await _taskRepository.InsertAsync(new InventoryTaskEntity(Guid.NewGuid())
        {
            Code = $"{prefix}-TSK-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} gorev",
            Type = InventoryTaskTypeEnum.WarehouseTransfer,
            Status = TaskStatusEnum.Draft,
            SourceWarehouseId = warehouse.Id,
            TargetWarehouseId = targetWarehouse.Id,
            StartDate = DateTime.UtcNow
        }, autoSave: true);

        var vehicleTask = await _vehicleTaskRepository.InsertAsync(new VehicleTask(Guid.NewGuid())
        {
            VehicleId = vehicle.Id,
            TaskId = task.Id,
            ResponsibleWorkerId = worker.Id,
            AssignedAt = DateTime.UtcNow
        }, autoSave: true);

        var request = await _repository.InsertAsync(new MovementRequest(Guid.NewGuid())
        {
            RequestNumber = $"{prefix}-REQ-{Guid.NewGuid():N}"[..30],
            Status = status,
            Priority = MovementPriorityEnum.Normal,
            RequestNote = $"{prefix} test talebi",
            PlannedDate = DateTime.UtcNow,
            RequestedByWorkerId = worker.Id,
            VehicleTaskId = vehicleTask.Id
        }, autoSave: true);

        return new MovementRequestSeed(request, worker, task);
    }
}

