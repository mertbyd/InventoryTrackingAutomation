using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Workflows;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace InventoryTrackingAutomation.Movements;

public abstract class MovementRequestManager_Tests<TStartupModule> : InventoryTrackingAutomationDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly MovementRequestManager _movementRequestManager;
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IWorkerRepository _workerRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IInventoryTaskRepository _inventoryTaskRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly ITaskLineRepository _taskLineRepository;
    private readonly IVehicleTaskLineRepository _vehicleTaskLineRepository;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid> _workerTypeRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid> _vehicleTypeRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.UnitType, Guid> _unitTypeRepository;

    protected MovementRequestManager_Tests()
    {
        _movementRequestManager = GetRequiredService<MovementRequestManager>();
        _movementRequestRepository = GetRequiredService<IMovementRequestRepository>();
        _warehouseRepository = GetRequiredService<IWarehouseRepository>();
        _vehicleRepository = GetRequiredService<IVehicleRepository>();
        _workerRepository = GetRequiredService<IWorkerRepository>();
        _productRepository = GetRequiredService<IRepository<Product, Guid>>();
        _inventoryTaskRepository = GetRequiredService<IInventoryTaskRepository>();
        _vehicleTaskRepository = GetRequiredService<IVehicleTaskRepository>();
        _taskLineRepository = GetRequiredService<ITaskLineRepository>();
        _vehicleTaskLineRepository = GetRequiredService<IVehicleTaskLineRepository>();
        _workflowDefinitionRepository = GetRequiredService<IWorkflowDefinitionRepository>();
        _workerTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.WorkerType, Guid>>();
        _vehicleTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.VehicleType, Guid>>();
        _unitTypeRepository = GetRequiredService<IRepository<InventoryTrackingAutomation.Entities.Lookups.UnitType, Guid>>();
    }

    [Fact]
    public async Task CreateWithWorkflowAsync_Should_Initialize_Request_And_Workflow()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var currentUserId = Guid.NewGuid();
            var sourceWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
            {
                Code = $"WH-MGR-SRC-{Guid.NewGuid():N}"[..30],
                Name = "Movement Manager Source Warehouse",
                IsActive = true
            }, autoSave: true);

            var targetWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
            {
                Code = $"WH-MGR-TGT-{Guid.NewGuid():N}"[..30],
                Name = "Movement Manager Target Warehouse",
                IsActive = true
            }, autoSave: true);

            var workerType = await _workerTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.WorkerType(Guid.NewGuid(), "MGR-WT", "WT"), autoSave: true);
            var vehicleType = await _vehicleTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.VehicleType(Guid.NewGuid(), "MGR-VT", "VT"), autoSave: true);
            var unitType = await _unitTypeRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.UnitType(Guid.NewGuid(), "MGR-UT", "UT"), autoSave: true);

            var worker = await _workerRepository.InsertAsync(new Worker(Guid.NewGuid())
            {
                UserId = currentUserId,
                RegistrationNumber = $"MGR-WRK-{Guid.NewGuid():N}"[..30],
                WorkerTypeId = workerType.Id,
                IsActive = true
            }, autoSave: true);

            var vehicle = await _vehicleRepository.InsertAsync(new Vehicle(Guid.NewGuid())
            {
                PlateNumber = $"34-MGR-{Guid.NewGuid():N}"[..16],
                VehicleTypeId = vehicleType.Id,
                IsActive = true
            }, autoSave: true);

            var task = await _inventoryTaskRepository.InsertAsync(new InventoryTask(Guid.NewGuid())
            {
                Code = $"TSK-MGR-{Guid.NewGuid():N}"[..30],
                Name = "Movement Manager Task",
                Type = InventoryTaskTypeEnum.WarehouseTransfer,
                Status = TaskStatusEnum.Draft,
                StartDate = DateTime.UtcNow,
                SourceWarehouseId = sourceWarehouse.Id,
                TargetWarehouseId = targetWarehouse.Id
            }, autoSave: true);

            var product = await _productRepository.InsertAsync(new Product(Guid.NewGuid())
            {
                Code = $"PRD-MGR-{Guid.NewGuid():N}"[..30],
                Name = "Movement Manager Product",
                UnitTypeId = unitType.Id,
                IsActive = true
            }, autoSave: true);

            var vehicleTask = await _vehicleTaskRepository.InsertAsync(new VehicleTask(Guid.NewGuid())
            {
                TaskId = task.Id,
                VehicleId = vehicle.Id,
                ResponsibleWorkerId = worker.Id,
                AssignedAt = DateTime.UtcNow
            }, autoSave: true);

            var taskLine = await _taskLineRepository.InsertAsync(new TaskLine(Guid.NewGuid())
            {
                TaskId = task.Id,
                ProductId = product.Id,
                Quantity = 1
            }, autoSave: true);

            await _vehicleTaskLineRepository.InsertAsync(new VehicleTaskLine(Guid.NewGuid())
            {
                VehicleTaskId = vehicleTask.Id,
                TaskLineId = taskLine.Id,
                AllocatedQuantity = 1
            }, autoSave: true);

            var workflowDefinition = await _workflowDefinitionRepository.FindAsync(x =>
                x.Name == WorkflowDefinitionNames.MovementRequest && x.IsActive);

            if (workflowDefinition == null)
            {
                workflowDefinition = new WorkflowDefinition(
                    Guid.NewGuid(),
                    WorkflowDefinitionNames.MovementRequest,
                    "Movement request test workflow",
                    isActive: true);

                workflowDefinition.Steps.Add(new WorkflowStepDefinition(
                    Guid.NewGuid(),
                    workflowDefinition.Id,
                    stepOrder: 1,
                    requiredRoleName: null));

                await _workflowDefinitionRepository.InsertAsync(workflowDefinition, autoSave: true);
            }

            var model = new CreateMovementRequestModel
            {
                RequestNumber = "REQ-MGR-001",
                Priority = MovementPriorityEnum.Normal,
                RequestedByWorkerId = worker.Id,
                VehicleTaskId = vehicleTask.Id,
                RequestNote = "Movement manager integration test",
                PlannedDate = DateTime.UtcNow.AddHours(1)
            };

            var validatedModel = await _movementRequestManager.CreateAsync(model);
            var request = new MovementRequest(Guid.NewGuid());
            request.RequestNumber = validatedModel.RequestNumber;
            request.Priority = validatedModel.Priority;
            request.RequestedByWorkerId = validatedModel.RequestedByWorkerId;
            request.VehicleTaskId = validatedModel.VehicleTaskId;
            request.RequestNote = validatedModel.RequestNote;
            request.PlannedDate = validatedModel.PlannedDate;
            request.Status = MovementStatusEnum.Pending;

            var workflowInstance = await _movementRequestManager.AssignWorkflowAsync(request, currentUserId);
            var inserted = await _movementRequestRepository.InsertAsync(request, autoSave: true);
            if (workflowInstance != null)
            {
                await _movementRequestManager.PublishInitialWorkflowStepAssignedAsync(workflowInstance);
            }

            // Assert
            request.ShouldNotBeNull();
            request.RequestNumber.ShouldBe("REQ-MGR-001");
            request.Status.ShouldBe(MovementStatusEnum.InReview); // Should be InReview if workflow started
            request.VehicleTaskId.ShouldNotBe(Guid.Empty);

            var savedRequest = await _movementRequestRepository.GetAsync(request.Id);
            savedRequest.WorkflowInstanceId.ShouldNotBeNull();
        });
    }
}

