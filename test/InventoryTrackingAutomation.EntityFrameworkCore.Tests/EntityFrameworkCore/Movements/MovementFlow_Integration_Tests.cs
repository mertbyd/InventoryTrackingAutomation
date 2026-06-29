using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Xunit;
using Xunit.Abstractions;
using InventoryTaskEntity = InventoryTrackingAutomation.Entities.Tasks.InventoryTask;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Movements;

/// <summary>
/// En kritik hareket sureclerini gercek SQLite EFCore altyapisiyla adim adim dogrular.
/// Test ciktisi, her adimda ilgili tablolarin nasil degistigini okunabilir tablo olarak yazar.
/// </summary>
public class MovementFlow_Integration_Tests : InventoryTrackingAutomationEntityFrameworkCoreTestBase
{
    private const string LogDirectoryName = "movement-flow-logs";
    private const string LatestLogFileName = "latest.md";
    private static readonly object LogFileLock = new();

    private readonly ITestOutputHelper _output;
    private string? _currentLogFilePath;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Vehicle, Guid> _vehicleRepository;
    private readonly IRepository<Worker, Guid> _workerRepository;
    private readonly IRepository<WorkerType, Guid> _workerTypeRepository;
    private readonly IRepository<VehicleType, Guid> _vehicleTypeRepository;
    private readonly IRepository<UnitType, Guid> _unitTypeRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IInventoryTransactionRepository _inventoryTransactionRepository;
    private readonly IInventoryTaskRepository _taskRepository;
    private readonly IVehicleTaskRepository _vehicleTaskRepository;
    private readonly IMovementRequestRepository _movementRequestRepository;
    private readonly ITaskLineRepository _taskLineRepository;
    private readonly IVehicleTaskLineRepository _vehicleTaskLineRepository;
    private readonly MovementRequestManager _movementRequestManager;
    private readonly MovementApprovalManager _movementApprovalManager;
    private readonly MovementRequestWorkflowCompletionManager _movementRequestWorkflowCompletionManager;
    private readonly TaskReturnRequestManager _taskReturnRequestManager;
    private readonly IRepository<MovementApproval, Guid> _movementApprovalRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;
    private readonly IRepository<WorkflowStepDefinition, Guid> _workflowStepDefinitionRepository;
    private readonly IdentityUserManager _identityUserManager;

    static MovementFlow_Integration_Tests()
    {
        var logDirectory = ResolveLogDirectory();
        Directory.CreateDirectory(logDirectory);
        File.WriteAllText(
            ResolveLatestLogFilePath(),
            "# Movement flow latest test raporu" + Environment.NewLine +
            Environment.NewLine +
            "Bu dosya son MovementFlow_Integration_Tests kosusunda uretilen birlesik adim raporudur." + Environment.NewLine +
            "Her adimdan sonra ilgili operation, movement, workflow ve stock tablolarinin yeni hali yazilir." + Environment.NewLine +
            Environment.NewLine);
    }

    public MovementFlow_Integration_Tests(ITestOutputHelper output)
    {
        _output = output;
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _vehicleRepository = GetRequiredService<IRepository<Vehicle, Guid>>();
        _workerRepository = GetRequiredService<IRepository<Worker, Guid>>();
        _workerTypeRepository = GetRequiredService<IRepository<WorkerType, Guid>>();
        _vehicleTypeRepository = GetRequiredService<IRepository<VehicleType, Guid>>();
        _unitTypeRepository = GetRequiredService<IRepository<UnitType, Guid>>();
        _productRepository = GetRequiredService<IRepository<Product, Guid>>();
        _stockLocationRepository = GetRequiredService<IStockLocationRepository>();
        _inventoryTransactionRepository = GetRequiredService<IInventoryTransactionRepository>();
        _taskRepository = GetRequiredService<IInventoryTaskRepository>();
        _vehicleTaskRepository = GetRequiredService<IVehicleTaskRepository>();
        _movementRequestRepository = GetRequiredService<IMovementRequestRepository>();
        _taskLineRepository = GetRequiredService<ITaskLineRepository>();
        _vehicleTaskLineRepository = GetRequiredService<IVehicleTaskLineRepository>();
        _movementRequestManager = GetRequiredService<MovementRequestManager>();
        _movementApprovalManager = GetRequiredService<MovementApprovalManager>();
        _movementRequestWorkflowCompletionManager = GetRequiredService<MovementRequestWorkflowCompletionManager>();
        _taskReturnRequestManager = GetRequiredService<TaskReturnRequestManager>();
        _movementApprovalRepository = GetRequiredService<IRepository<MovementApproval, Guid>>();
        _workflowInstanceRepository = GetRequiredService<IWorkflowInstanceRepository>();
        _workflowInstanceStepRepository = GetRequiredService<IWorkflowInstanceStepRepository>();
        _workflowStepDefinitionRepository = GetRequiredService<IRepository<WorkflowStepDefinition, Guid>>();
        _identityUserManager = GetRequiredService<IdentityUserManager>();
    }

    [Fact]
    public async Task Warehouse_Transfer_Should_Move_Stock_Through_VehicleTask()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var scenario = await SeedScenarioAsync("WT", 20);
            await WriteStepAsync(
                "WT-01 Seed",
                "stock_locations: SourceWarehouse icin 20 adet baslangic stogu olustu.",
                scenario);

            var task = await InsertTaskAsync("WT", InventoryTaskTypeEnum.WarehouseTransfer, scenario.SourceWarehouseId, scenario.TargetWarehouseId);
            await WriteStepAsync(
                "WT-02 Task",
                "operation.tasks: Type=WarehouseTransfer, Status=Draft. Surecin tipi artik bu entity'den okunur.",
                scenario,
                task.Id);

            var request = await CreateRequestAsync("WT", task.Id, scenario, quantity: 8);
            await AssertTaskVehicleLineMappingAsync(task.Id, request.VehicleTaskId, scenario.ProductId, 8, 8);
            await WriteStepAsync(
                "WT-03 Request",
                "movement_requests: InReview talep olustu ve WorkflowInstanceId baglandi. vehicle_tasks: arac task'a baglandi. task_lines: gorev urun ihtiyaci 8. vehicle_task_lines: araca 8 tahsis edildi.",
                scenario,
                task.Id,
                request.Id);

            await ApproveWithWorkflowAsync(request.Id, scenario);
            await WriteStepAsync(
                "WT-04 Workflow Approvals",
                "workflow: InitiatorManager, TargetWarehouseManager ve SourceWarehouseManager adimlari gercek onaylarla tamamlandi. movement_approvals: 3 karar yazildi. movement_requests.Status: InReview -> Approved.",
                scenario,
                task.Id,
                request.Id);

            await _movementRequestManager.DispatchAsync(request.Id, "Araca yuklendi", scenario.UserId, scenario.WorkerId);
            await WriteStepAsync(
                "WT-05 Dispatch",
                "stock_locations: SourceWarehouse 20 -> 12, Vehicle 0 -> 8. inventory_transactions: WarehouseToVehicle(8). movement_requests.Status: Approved -> Shipped. tasks.Status: Draft -> InProgress.",
                scenario,
                task.Id,
                request.Id);

            await _movementRequestManager.ReceiveAsync(request.Id, new ReceiveMovementRequestModel { ReceiveNote = "Hedef depoya ulasti" }, scenario.UserId);
            await WriteStepAsync(
                "WT-06 Receive",
                "stock_locations: Vehicle 8 -> 0, TargetWarehouse 0 -> 8. inventory_transactions: VehicleToWarehouse(8). movement_requests.Status: Shipped -> Completed. tasks.Status: InProgress -> Completed. vehicle_tasks.ReleasedAt doldu.",
                scenario,
                task.Id,
                request.Id);

            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Warehouse, scenario.SourceWarehouseId, 12);
            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Vehicle, scenario.VehicleId, 0);
            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Warehouse, scenario.TargetWarehouseId, 8);

            var completedRequest = await _movementRequestRepository.GetAsync(request.Id);
            var completedTask = await _taskRepository.GetAsync(task.Id);
            var vehicleTask = await _vehicleTaskRepository.GetAsync(completedRequest.VehicleTaskId);

            Assert.Equal(MovementStatusEnum.Completed, completedRequest.Status);
            Assert.Equal(TaskStatusEnum.Completed, completedTask.Status);
            Assert.NotNull(vehicleTask.ReleasedAt);
        });
    }

    [Fact]
    public async Task Field_Operation_Should_Create_Return_Request_And_Reconcile_Vehicle_Stock()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var scenario = await SeedScenarioAsync("FO", 20);
            await WriteStepAsync(
                "FO-01 Seed",
                "stock_locations: SourceWarehouse icin 20 adet baslangic stogu olustu.",
                scenario);

            var task = await InsertTaskAsync("FO", InventoryTaskTypeEnum.FieldOperation, scenario.SourceWarehouseId, returnWarehouseId: scenario.SourceWarehouseId);
            await WriteStepAsync(
                "FO-02 Task",
                "operation.tasks: Type=FieldOperation, ReturnWarehouseId=SourceWarehouse, Status=Draft.",
                scenario,
                task.Id);

            var request = await CreateRequestAsync("FO", task.Id, scenario, quantity: 10);
            await AssertTaskVehicleLineMappingAsync(task.Id, request.VehicleTaskId, scenario.ProductId, 10, 10);
            await WriteStepAsync(
                "FO-03 Main Request",
                "movement_requests: ParentMovementRequestId=null ana hareket InReview olustu ve WorkflowInstanceId baglandi. vehicle_tasks: arac task'a baglandi. task_lines: gorev urun ihtiyaci 10. vehicle_task_lines: araca 10 tahsis edildi.",
                scenario,
                task.Id,
                request.Id);

            await ApproveWithWorkflowAsync(request.Id, scenario);
            await WriteStepAsync(
                "FO-04 Workflow Approvals",
                "workflow: InitiatorManager ve LogisticsManager adimlari gercek onaylarla tamamlandi. movement_approvals: 2 karar yazildi. movement_requests.Status: InReview -> Approved.",
                scenario,
                task.Id,
                request.Id);

            await _movementRequestManager.DispatchAsync(request.Id, "Sahaya cikis", scenario.UserId, scenario.WorkerId);
            await WriteStepAsync(
                "FO-05 Dispatch",
                "stock_locations: SourceWarehouse 20 -> 10, Vehicle 0 -> 10. inventory_transactions: WarehouseToVehicle(10). tasks.Status: Draft -> InProgress. movement_requests.Status: Approved -> Shipped.",
                scenario,
                task.Id,
                request.Id);

            await _movementRequestManager.ReceiveAsync(request.Id, new ReceiveMovementRequestModel { ReceiveNote = "Saha aracina teslim" }, scenario.UserId);
            await WriteStepAsync(
                "FO-06 Main Receive",
                "FieldOperation kurali: stok Vehicle uzerinde kalir. stock_locations: SourceWarehouse 10, Vehicle 10. movement_requests.Status: Shipped -> Completed. task henuz InProgress.",
                scenario,
                task.Id,
                request.Id);

            task.Status = TaskStatusEnum.Completed;
            await _taskRepository.UpdateAsync(task, autoSave: true);
            await WriteStepAsync(
                "FO-07 Task Completed",
                "operation.tasks.Status: InProgress -> Completed. Aractaki stok iade bekliyor.",
                scenario,
                task.Id,
                request.Id);

            await _taskReturnRequestManager.CreateReturnRequestsForTaskAsync(task.Id, scenario.UserId, scenario.WorkerId);
            var returnRequest = (await _movementRequestRepository.GetListAsync(x =>
                    x.ParentMovementRequestId == request.Id))
                .Single();
            await WriteStepAsync(
                "FO-08 Return Request",
                "movement_requests: ParentMovementRequestId ana talebi gosteren iade talebi olustu. Status=Shipped. vehicle_task_lines: Vehicle uzerinde kalan 10 adet iade satiri.",
                scenario,
                task.Id,
                request.Id,
                returnRequest.Id);

            // VehicleTaskLine ID ile uzlasma yapilir; ProductId degil.
            var returnVehicleTaskLines = await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(returnRequest.VehicleTaskId);
            var returnTaskLineIds = returnVehicleTaskLines.Select(x => x.TaskLineId).Distinct().ToList();
            var returnTaskLines = await _taskLineRepository.GetListAsync(x => returnTaskLineIds.Contains(x.Id));
            var returnProductByTaskLineId = returnTaskLines.ToDictionary(x => x.Id, x => x.ProductId);
            var vtlForReturn = returnVehicleTaskLines
                .Single(x => returnProductByTaskLineId[x.TaskLineId] == scenario.ProductId);

            await _movementRequestManager.ReceiveAsync(
                returnRequest.Id,
                new ReceiveMovementRequestModel
                {
                    ReceiveNote = "Saha donusu sayim ve uzlasi",
                    Lines =
                    {
                        new ReceiveMovementRequestVehicleTaskLineModel
                        {
                            VehicleTaskLineId = vtlForReturn.Id,
                            ReceivedQuantity = 7,
                            DamagedQuantity = 1,
                            LostQuantity = 0,
                            ConsumedQuantity = 2,
                            Note = "7 saglam dondu, 2 tuketildi, 1 kirildi."
                        }
                    }
                },
                scenario.UserId);
            await WriteStepAsync(
                "FO-09 Return Receive",
                "vehicle_task_lines: Received=7, Damaged=1, Lost=0, Consumed=2. stock_locations: SourceWarehouse 10 -> 17, Vehicle 10 -> 0. inventory_transactions: VehicleToWarehouse(7) + Adjustment(3). return request Completed, vehicle_task ReleasedAt doldu.",
                scenario,
                task.Id,
                request.Id,
                returnRequest.Id);

            var reconciledLine = await _vehicleTaskLineRepository.GetAsync(vtlForReturn.Id);
            Assert.Equal(7, reconciledLine.ReceivedQuantity);
            Assert.Equal(1, reconciledLine.DamagedQuantity);
            Assert.Equal(0, reconciledLine.LostQuantity);
            Assert.Equal(2, reconciledLine.ConsumedQuantity);

            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Warehouse, scenario.SourceWarehouseId, 17);
            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Vehicle, scenario.VehicleId, 0);

            var completedReturn = await _movementRequestRepository.GetAsync(returnRequest.Id);
            var vehicleTask = await _vehicleTaskRepository.GetAsync(completedReturn.VehicleTaskId);

            Assert.Equal(MovementStatusEnum.Completed, completedReturn.Status);
            Assert.NotNull(vehicleTask.ReleasedAt);
        });
    }

    [Fact]
    public async Task Warehouse_Transfer_Should_Not_Dispatch_When_Source_Stock_Is_Insufficient()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var scenario = await SeedScenarioAsync("WT-INS", 5);
            await WriteStepAsync(
                "WT-INS-01 Seed",
                "stock_locations: SourceWarehouse icin sadece 5 adet stok var. Test 10 adet dispatch deneyecek.",
                scenario);

            var task = await InsertTaskAsync("WT-INS", InventoryTaskTypeEnum.WarehouseTransfer, scenario.SourceWarehouseId, scenario.TargetWarehouseId);
            var request = await CreateRequestAsync("WT-INS", task.Id, scenario, quantity: 10);
            await AssertTaskVehicleLineMappingAsync(task.Id, request.VehicleTaskId, scenario.ProductId, 10, 10);
            await ApproveWithWorkflowAsync(request.Id, scenario);
            await WriteStepAsync(
                "WT-INS-02 Approved Request",
                "task_lines: gorev urun ihtiyaci 10. vehicle_task_lines: araca 10 tahsis edildi. stock_locations: SourceWarehouse 5. Dispatch BusinessException vermeli.",
                scenario,
                task.Id,
                request.Id);

            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _movementRequestManager.DispatchAsync(request.Id, "Yetersiz stok sevk denemesi", scenario.UserId, scenario.WorkerId);
            });
            await WriteStepAsync(
                "WT-INS-03 Dispatch Blocked",
                "Beklenen hata: StockLocations.InsufficientStock. stock_locations degismemeli: SourceWarehouse 5, Vehicle stok satiri yok. inventory_transactions yeni hareket yazmamali.",
                scenario,
                task.Id,
                request.Id);

            Assert.Equal(InventoryTrackingAutomationErrorCodes.StockLocations.InsufficientStock, exception.Code);
            await AssertStockAsync(scenario.ProductId, StockLocationTypeEnum.Warehouse, scenario.SourceWarehouseId, 5);

            var vehicleStock = await _stockLocationRepository.FindAsync(x =>
                x.ProductId == scenario.ProductId &&
                x.LocationType == StockLocationTypeEnum.Vehicle &&
                x.LocationId == scenario.VehicleId);

            Assert.Null(vehicleStock);
        });
    }

    [Fact]
    public async Task Field_Operation_Should_Not_Create_Duplicate_Open_Return_Request()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var scenario = await SeedScenarioAsync("FO-DUP", 20);
            await WriteStepAsync(
                "FO-DUP-01 Seed",
                "stock_locations: SourceWarehouse icin 20 adet stok var. Duplicate return request korumasi test edilecek.",
                scenario);

            var task = await InsertTaskAsync("FO-DUP", InventoryTaskTypeEnum.FieldOperation, scenario.SourceWarehouseId, returnWarehouseId: scenario.SourceWarehouseId);
            var request = await CreateRequestAsync("FO-DUP", task.Id, scenario, quantity: 10);
            await AssertTaskVehicleLineMappingAsync(task.Id, request.VehicleTaskId, scenario.ProductId, 10, 10);

            await ApproveWithWorkflowAsync(request.Id, scenario);
            await _movementRequestManager.DispatchAsync(request.Id, "Sahaya cikis", scenario.UserId, scenario.WorkerId);
            await _movementRequestManager.ReceiveAsync(request.Id, new ReceiveMovementRequestModel { ReceiveNote = "Saha araci teslimi" }, scenario.UserId);
            await WriteStepAsync(
                "FO-DUP-02 Main Movement Completed",
                "stock_locations: SourceWarehouse 20 -> 10, Vehicle 0 -> 10. Ana movement Completed; iade henuz yok.",
                scenario,
                task.Id,
                request.Id);

            task.Status = TaskStatusEnum.Completed;
            await _taskRepository.UpdateAsync(task, autoSave: true);

            await _taskReturnRequestManager.CreateReturnRequestsForTaskAsync(task.Id, scenario.UserId, scenario.WorkerId);
            var firstReturnRequests = await _movementRequestRepository.GetListAsync(x =>
                x.ParentMovementRequestId == request.Id);
            await WriteStepAsync(
                "FO-DUP-03 First Return Request",
                "movement_requests: bir adet acik iade talebi olustu. Bu talep tamamlanmadan ikinci kez ayni iade uretilmemeli.",
                scenario,
                task.Id,
                request.Id,
                firstReturnRequests.Single().Id);

            await _taskReturnRequestManager.CreateReturnRequestsForTaskAsync(task.Id, scenario.UserId, scenario.WorkerId);

            var returnRequests = await _movementRequestRepository.GetListAsync(x =>
                x.ParentMovementRequestId == request.Id);
            await WriteStepAsync(
                "FO-DUP-04 Second Return Create Attempt",
                "TaskReturnRequestManager existing open return request'i gordu ve duplicate olusturmadi. movement_requests return sayisi 1 kalmali.",
                scenario,
                task.Id,
                request.Id,
                returnRequests.Single().Id);

            Assert.Single(returnRequests);
        });
    }

    private async Task<MovementRequest> CreateRequestAsync(
        string prefix,
        Guid taskId,
        MovementScenario scenario,
        int quantity)
    {
        var taskLine = await _taskLineRepository.FindByTaskAndProductAsync(taskId, scenario.ProductId);
        if (taskLine == null)
        {
            taskLine = await _taskLineRepository.InsertAsync(new TaskLine(Guid.NewGuid())
            {
                TaskId = taskId,
                ProductId = scenario.ProductId,
                Quantity = quantity
            }, autoSave: true);
        }

        if (taskLine.Quantity < quantity)
        {
            taskLine.Quantity = quantity;
            await _taskLineRepository.UpdateAsync(taskLine, autoSave: true);
        }

        var vehicleTask = await _vehicleTaskRepository.FindAsync(x =>
            x.TaskId == taskId &&
            x.VehicleId == scenario.VehicleId &&
            !x.ReleasedAt.HasValue);

        if (vehicleTask == null)
        {
            vehicleTask = await _vehicleTaskRepository.InsertAsync(new VehicleTask(Guid.NewGuid())
            {
                TaskId = taskId,
                VehicleId = scenario.VehicleId,
                ResponsibleWorkerId = scenario.WorkerId,
                AssignedAt = DateTime.UtcNow
            }, autoSave: true);
        }

        var vehicleTaskLine = await _vehicleTaskLineRepository.FindByVehicleTaskAndTaskLineAsync(vehicleTask.Id, taskLine.Id);
        if (vehicleTaskLine == null)
        {
            await _vehicleTaskLineRepository.InsertAsync(new VehicleTaskLine(Guid.NewGuid())
            {
                VehicleTaskId = vehicleTask.Id,
                TaskLineId = taskLine.Id,
                AllocatedQuantity = quantity
            }, autoSave: true);
        }
        else
        {
            vehicleTaskLine.AllocatedQuantity = quantity;
            await _vehicleTaskLineRepository.UpdateAsync(vehicleTaskLine, autoSave: true);
        }

        return await _movementRequestManager.CreateWithWorkflowAsync(
            new CreateMovementRequestModel
            {
                RequestNumber = $"{prefix}-REQ-{Guid.NewGuid():N}"[..30],
                RequestedByWorkerId = scenario.WorkerId,
                VehicleTaskId = vehicleTask.Id,
                Priority = MovementPriorityEnum.Normal,
                RequestNote = $"{prefix} entegrasyon talebi",
                PlannedDate = DateTime.UtcNow.AddHours(1)
            },
            scenario.UserId);
    }

    private async Task AssertTaskVehicleLineMappingAsync(
        Guid taskId,
        Guid vehicleTaskId,
        Guid productId,
        int expectedTaskQuantity,
        int expectedVehicleQuantity)
    {
        var taskLine = await _taskLineRepository.FindByTaskAndProductAsync(taskId, productId);
        Assert.NotNull(taskLine);
        Assert.Equal(expectedTaskQuantity, taskLine!.Quantity);

        var vehicleTaskLine = await _vehicleTaskLineRepository.FindByVehicleTaskAndTaskLineAsync(vehicleTaskId, taskLine.Id);
        Assert.NotNull(vehicleTaskLine);
        Assert.Equal(expectedVehicleQuantity, vehicleTaskLine!.AllocatedQuantity);

        var derivedAllocatedQuantity = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(taskLine.Id);
        Assert.Equal(expectedVehicleQuantity, derivedAllocatedQuantity);
    }

    private async Task ApproveWithWorkflowAsync(Guid movementRequestId, MovementScenario scenario)
    {
        // Bu testler approval surecini artik status'u elle degistirerek gecmez.
        // Her pending workflow step'i kendi AssignedUserId'si ile onaylanir ve movement_approvals tablosuna denetim izi yazilir.
        for (var safetyCounter = 0; safetyCounter < 5; safetyCounter++)
        {
            var request = await _movementRequestRepository.GetAsync(movementRequestId);
            if (request.Status == MovementStatusEnum.Approved)
            {
                return;
            }

            Assert.Equal(MovementStatusEnum.InReview, request.Status);
            var workflowInstanceId = request.WorkflowInstanceId.GetValueOrDefault();
            Assert.NotEqual(Guid.Empty, workflowInstanceId);

            var pendingSteps = await _workflowInstanceStepRepository.GetListAsync(x =>
                x.WorkflowInstanceId == workflowInstanceId &&
                x.ActionTaken == WorkflowActionType.Pending);
            if (pendingSteps.Count == 0)
            {
                var workflowInstance = await _workflowInstanceRepository.GetAsync(workflowInstanceId);
                Assert.Equal(WorkflowState.Completed, workflowInstance.State);

                // Local event normalde UnitOfWork kapanisinda MovementRequest'e sonucu uygular.
                // Bu test ayni UnitOfWork icinde dispatch'e devam ettigi icin sonucu burada domain manager ile uygulariz.
                await _movementRequestWorkflowCompletionManager.ApplyWorkflowResultAsync(request.Id, workflowInstance.State);
                continue;
            }

            var pendingStepInfos = new List<(WorkflowInstanceStep Step, WorkflowStepDefinition StepDefinition)>();
            foreach (var pendingStep in pendingSteps)
            {
                var stepDefinition = await _workflowStepDefinitionRepository.GetAsync(pendingStep.WorkflowStepDefinitionId);
                pendingStepInfos.Add((pendingStep, stepDefinition));
            }

            var current = pendingStepInfos
                .OrderBy(x => x.StepDefinition.StepOrder)
                .First();
            var approverUserId = current.Step.AssignedUserId ?? ResolveFallbackApproverUserId(current.StepDefinition, scenario);

            LogLine(
                $"Approval actor: StepOrder={current.StepDefinition.StepOrder}, Resolver={current.StepDefinition.ResolverKey ?? current.StepDefinition.RequiredRoleName ?? "-"}, Approver={ActorLabel(approverUserId, scenario)} ({Short(approverUserId)})");

            await _movementApprovalManager.ApproveAsync(
                movementRequestId,
                approverUserId,
                $"{current.StepDefinition.ResolverKey ?? current.StepDefinition.RequiredRoleName ?? "Workflow"} test onayi.");
        }

        var latestRequest = await _movementRequestRepository.GetAsync(movementRequestId);
        Assert.Equal(MovementStatusEnum.Approved, latestRequest.Status);
    }

    private async Task<InventoryTaskEntity> InsertTaskAsync(
        string prefix,
        InventoryTaskTypeEnum type,
        Guid sourceWarehouseId,
        Guid? targetWarehouseId = null,
        Guid? returnWarehouseId = null)
    {
        var task = new InventoryTaskEntity(Guid.NewGuid())
        {
            Type = type,
            Code = $"{prefix}-TASK-{Guid.NewGuid():N}"[..40],
            Name = $"{prefix} Entegrasyon Gorevi",
            Region = type == InventoryTaskTypeEnum.FieldOperation ? "Test Bolgesi" : null,
            SourceWarehouseId = sourceWarehouseId,
            TargetWarehouseId = targetWarehouseId,
            StartDate = DateTime.UtcNow,
            Status = TaskStatusEnum.Draft,
            Description = $"{prefix} entegrasyon gorevi aciklamasi",
            ReturnWarehouseId = returnWarehouseId
        };

        return await _taskRepository.InsertAsync(task, autoSave: true);
    }

    private async Task<MovementScenario> SeedScenarioAsync(string prefix, int sourceQuantity)
    {
        var initiatorManager = await InsertActorAsync(prefix, "IM", "WHITE_COLLAR");
        var sourceWarehouseManager = await InsertActorAsync(prefix, "SM", "WHITE_COLLAR");
        var targetWarehouseManager = await InsertActorAsync(prefix, "TM", "WHITE_COLLAR");
        var requester = await InsertActorAsync(prefix, "RQ", "BLUE_COLLAR", initiatorManager.WorkerId);

        var sourceWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"{prefix}-SRC-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} Kaynak Depo",
            ManagerWorkerId = sourceWarehouseManager.WorkerId,
            IsActive = true
        }, autoSave: true);

        var targetWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"{prefix}-TRG-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} Hedef Depo",
            ManagerWorkerId = targetWarehouseManager.WorkerId,
            IsActive = true
        }, autoSave: true);

        var vehicleType = await EnsureVehicleTypeAsync("VAN");
        var unitType = await EnsureUnitTypeAsync("PIECE");

        var vehicle = await _vehicleRepository.InsertAsync(new Vehicle(Guid.NewGuid())
        {
            PlateNumber = $"{prefix}-{Guid.NewGuid():N}"[..12],
            VehicleTypeId = vehicleType.Id,
            IsActive = true
        }, autoSave: true);

        var product = await _productRepository.InsertAsync(new Product(Guid.NewGuid())
        {
            Code = $"{prefix}-PRD-{Guid.NewGuid():N}"[..30],
            Name = $"{prefix} Urun",
            UnitTypeId = unitType.Id,
            IsActive = true,
            IsSerializable = false
        }, autoSave: true);

        await _stockLocationRepository.InsertAsync(new StockLocation(Guid.NewGuid())
        {
            ProductId = product.Id,
            LocationType = StockLocationTypeEnum.Warehouse,
            LocationId = sourceWarehouse.Id,
            Quantity = sourceQuantity,
            ReservedQuantity = 0
        }, autoSave: true);

        return new MovementScenario(
            sourceWarehouse.Id,
            targetWarehouse.Id,
            vehicle.Id,
            requester.WorkerId,
            requester.UserId,
            product.Id,
            initiatorManager.WorkerId,
            initiatorManager.UserId,
            sourceWarehouseManager.WorkerId,
            sourceWarehouseManager.UserId,
            targetWarehouseManager.WorkerId,
            targetWarehouseManager.UserId);
    }

    private async Task<TestActor> InsertActorAsync(
        string prefix,
        string roleCode,
        string workerTypeCode,
        Guid? managerWorkerId = null)
    {
        var suffix = Guid.NewGuid().ToString("N")[..8];
        var safePrefix = SafeToken(prefix);
        var safeRole = SafeToken(roleCode);
        var userId = Guid.NewGuid();
        var userName = $"test{safePrefix}{safeRole}{suffix}".ToLowerInvariant();

        var user = new IdentityUser(userId, userName, $"{userName}@inventory.test")
        {
            Name = $"{prefix} {roleCode} test aktoru"
        };

        var createResult = await _identityUserManager.CreateAsync(user, "123456aA@");
        Assert.True(
            createResult.Succeeded,
            string.Join("; ", createResult.Errors.Select(x => x.Description)));

        var workerType = await EnsureWorkerTypeAsync(workerTypeCode);

        var worker = await _workerRepository.InsertAsync(new Worker(Guid.NewGuid())
        {
            UserId = userId,
            RegistrationNumber = BuildRegistrationNumber(safePrefix, safeRole, suffix),
            WorkerTypeId = workerType.Id,
            ManagerId = managerWorkerId,
            IsActive = true
        }, autoSave: true);

        return new TestActor(worker.Id, userId);
    }

    private async Task<WorkerType> EnsureWorkerTypeAsync(string code)
    {
        // islevi: Test aktoru icin gerekli lookup worker type kaydini hazirlar.
        // sistemdeki gorevi: Enum yerine FK kullanan Worker modeline uygun test verisi uretir.
        var existing = await _workerTypeRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null)
        {
            return existing;
        }

        var name = code == "WHITE_COLLAR" ? "Beyaz Yaka" : "Mavi Yaka";
        return await _workerTypeRepository.InsertAsync(new WorkerType(Guid.NewGuid(), code, name), autoSave: true);
    }

    private async Task<VehicleType> EnsureVehicleTypeAsync(string code)
    {
        // islevi: Test araci icin gerekli lookup vehicle type kaydini hazirlar.
        // sistemdeki gorevi: Enum yerine FK kullanan Vehicle modeline uygun test verisi uretir.
        var existing = await _vehicleTypeRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null)
        {
            return existing;
        }

        return await _vehicleTypeRepository.InsertAsync(new VehicleType(Guid.NewGuid(), code, "Panelvan"), autoSave: true);
    }

    private async Task<UnitType> EnsureUnitTypeAsync(string code)
    {
        // islevi: Test urunu icin gerekli lookup unit type kaydini hazirlar.
        // sistemdeki gorevi: Enum yerine FK kullanan Product modeline uygun test verisi uretir.
        var existing = await _unitTypeRepository.FirstOrDefaultAsync(x => x.Code == code);
        if (existing != null)
        {
            return existing;
        }

        return await _unitTypeRepository.InsertAsync(new UnitType(Guid.NewGuid(), code, "Adet"), autoSave: true);
    }

    private static string BuildRegistrationNumber(string safePrefix, string roleCode, string suffix)
    {
        var trimmedPrefix = safePrefix.Length > 6 ? safePrefix[..6] : safePrefix;
        var value = $"{trimmedPrefix}-{roleCode}-{suffix}".ToUpperInvariant();
        return value.Length <= 20 ? value : value[..20];
    }

    private static string SafeToken(string value)
    {
        var token = new string(value.Where(char.IsLetterOrDigit).ToArray());
        return string.IsNullOrWhiteSpace(token) ? "T" : token.ToUpperInvariant();
    }

    private async Task AssertStockAsync(
        Guid productId,
        StockLocationTypeEnum locationType,
        Guid locationId,
        int expectedQuantity)
    {
        var stock = await _stockLocationRepository.FindAsync(x =>
            x.ProductId == productId &&
            x.LocationType == locationType &&
            x.LocationId == locationId);

        Assert.NotNull(stock);
        Assert.Equal(expectedQuantity, stock!.Quantity);
    }

    private async Task WriteStepAsync(
        string title,
        string expectedChange,
        MovementScenario scenario,
        Guid? taskId = null,
        Guid? mainRequestId = null,
        Guid? returnRequestId = null)
    {
        EnsureScenarioLogStarted(title);

        LogLine("");
        LogLine($"========== {title} ==========");
        LogLine($"Simdi ne yaptik: {DescribeAction(title)}");
        LogLine($"Beklenen degisim: {expectedChange}");
        LogLine($"Scenario IDs: SourceWarehouse={Short(scenario.SourceWarehouseId)}, TargetWarehouse={Short(scenario.TargetWarehouseId)}, Vehicle={Short(scenario.VehicleId)}, RequesterWorker={Short(scenario.WorkerId)}, Product={Short(scenario.ProductId)}, InitiatorManagerUser={Short(scenario.InitiatorManagerUserId)}, SourceManagerUser={Short(scenario.SourceWarehouseManagerUserId)}, TargetManagerUser={Short(scenario.TargetWarehouseManagerUserId)}");

        await WriteStockTableAsync(scenario);

        if (taskId.HasValue)
        {
            await WriteTaskTablesAsync(taskId.Value);
        }

        var requestIds = new List<Guid>();
        if (mainRequestId.HasValue)
        {
            requestIds.Add(mainRequestId.Value);
        }

        if (returnRequestId.HasValue)
        {
            requestIds.Add(returnRequestId.Value);
        }

        if (requestIds.Count > 0)
        {
            await WriteMovementTablesAsync(requestIds);
            await WriteWorkflowAndApprovalTablesAsync(requestIds);
            await WriteTransactionTableAsync(requestIds);
        }
    }

    private void EnsureScenarioLogStarted(string title)
    {
        if (_currentLogFilePath != null)
        {
            return;
        }

        var logDirectory = ResolveLogDirectory();
        Directory.CreateDirectory(logDirectory);

        var scenarioKey = GetScenarioLogKey(title);
        _currentLogFilePath = Path.Combine(logDirectory, $"{scenarioKey}.md");
        File.WriteAllText(
            _currentLogFilePath,
            $"# {scenarioKey} hareket sureci test logu{Environment.NewLine}{Environment.NewLine}");

        AppendLineToFile(
            ResolveLatestLogFilePath(),
            $"## {scenarioKey} hareket sureci{Environment.NewLine}");
        _output.WriteLine($"Movement flow log file: {_currentLogFilePath}");
        _output.WriteLine($"Movement flow latest report: {ResolveLatestLogFilePath()}");
    }

    private void LogLine(string line)
    {
        _output.WriteLine(line);

        if (_currentLogFilePath != null)
        {
            AppendLineToFile(_currentLogFilePath, line);
            AppendLineToFile(ResolveLatestLogFilePath(), line);
        }
    }

    private static string ResolveLogDirectory()
    {
        return Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "TestResults",
            LogDirectoryName));
    }

    private static string ResolveLatestLogFilePath()
    {
        return Path.Combine(ResolveLogDirectory(), LatestLogFileName);
    }

    private static void AppendLineToFile(string filePath, string line)
    {
        lock (LogFileLock)
        {
            File.AppendAllText(filePath, line + Environment.NewLine);
        }
    }

    private static string GetScenarioLogKey(string title)
    {
        var firstToken = title.Split(' ', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "movement-flow";
        var lastDashIndex = firstToken.LastIndexOf('-');
        var scenarioToken = lastDashIndex > 0 ? firstToken[..lastDashIndex] : firstToken;
        var safeChars = scenarioToken
            .ToLowerInvariant()
            .Where(x => char.IsLetterOrDigit(x) || x == '-')
            .ToArray();

        return safeChars.Length == 0 ? "movement-flow" : new string(safeChars);
    }

    private async Task WriteStockTableAsync(MovementScenario scenario)
    {
        var stocks = (await _stockLocationRepository.GetListAsync(x => x.ProductId == scenario.ProductId))
            .OrderBy(x => x.LocationType)
            .ThenBy(x => LocationLabel(x.LocationType, x.LocationId, scenario))
            .ToList();

        LogLine("");
        LogLine("stock.stock_locations");
        LogLine("| Location | LocationId | Quantity | Reserved |");
        LogLine("| --- | --- | ---: | ---: |");

        if (stocks.Count == 0)
        {
            LogLine("| none | - | 0 | 0 |");
            return;
        }

        foreach (var stock in stocks)
        {
            LogLine($"| {stock.LocationType}:{LocationLabel(stock.LocationType, stock.LocationId, scenario)} | {Short(stock.LocationId)} | {stock.Quantity} | {stock.ReservedQuantity} |");
        }
    }

    private async Task WriteTaskTablesAsync(Guid taskId)
    {
        var task = await _taskRepository.GetAsync(taskId);
        var vehicleTasks = (await _vehicleTaskRepository.GetListAsync(x => x.TaskId == taskId))
            .OrderBy(x => x.AssignedAt)
            .ToList();

        LogLine("");
        LogLine("operation.tasks");
        LogLine("| TaskId | Type | Status | SourceWarehouseId | TargetWarehouseId | ReturnWarehouseId |");
        LogLine("| --- | --- | --- | --- | --- | --- |");
        LogLine($"| {Short(task.Id)} | {task.Type} | {task.Status} | {Short(task.SourceWarehouseId)} | {Short(task.TargetWarehouseId)} | {Short(task.ReturnWarehouseId)} |");

        var taskLines = (await _taskLineRepository.GetListAsync(x => x.TaskId == taskId))
            .OrderBy(x => x.CreationTime)
            .ToList();

        LogLine("");
        LogLine("operation.task_lines");
        LogLine("| TaskLineId | TaskId | ProductId | Quantity |");
        LogLine("| --- | --- | --- | ---: |");

        if (taskLines.Count == 0)
        {
            LogLine("| none | - | - | 0 |");
        }
        else
        {
            foreach (var taskLine in taskLines)
            {
                LogLine($"| {Short(taskLine.Id)} | {Short(taskLine.TaskId)} | {Short(taskLine.ProductId)} | {taskLine.Quantity} |");
            }
        }

        LogLine("");
        LogLine("operation.vehicle_tasks");
        LogLine("| VehicleTaskId | TaskId | VehicleId | ResponsibleWorkerId | AssignedAt | ReleasedAt |");
        LogLine("| --- | --- | --- | --- | --- | --- |");

        if (vehicleTasks.Count == 0)
        {
            LogLine("| none | - | - | - | - | - |");
            return;
        }

        foreach (var vehicleTask in vehicleTasks)
        {
            LogLine($"| {Short(vehicleTask.Id)} | {Short(vehicleTask.TaskId)} | {Short(vehicleTask.VehicleId)} | {Short(vehicleTask.ResponsibleWorkerId)} | {vehicleTask.AssignedAt:HH:mm:ss} | {FormatTime(vehicleTask.ReleasedAt)} |");
        }
    }

    private async Task WriteMovementTablesAsync(IReadOnlyCollection<Guid> requestIds)
    {
        var requests = (await _movementRequestRepository.GetListAsync(x => requestIds.Contains(x.Id)))
            .OrderBy(x => x.CreationTime)
            .ToList();
        LogLine("");
        LogLine("movement.movement_requests");
        LogLine("| RequestId | RequestNumber | Status | TaskId | VehicleTaskId | ParentMovementRequestId | SourceWarehouseId | TargetWarehouseId |");
        LogLine("| --- | --- | --- | --- | --- | --- | --- | --- |");

        var vehicleTaskIds = new HashSet<Guid>();
        foreach (var request in requests)
        {
            var context = await _movementRequestRepository.GetOperationalContextAsync(request.Id);
            LogLine($"| {Short(request.Id)} | {request.RequestNumber} | {request.Status} | {Short(context?.TaskId)} | {Short(request.VehicleTaskId)} | {Short(request.ParentMovementRequestId)} | {Short(context?.SourceWarehouseId)} | {Short(context?.TargetWarehouseId)} |");
            if (request.VehicleTaskId != Guid.Empty) vehicleTaskIds.Add(request.VehicleTaskId);
        }

        LogLine("");
        LogLine("operation.vehicle_task_lines");
        LogLine("| LineId | VehicleTaskId | ProductId | Allocated | Received | Damaged | Lost | Consumed | Note |");
        LogLine("| --- | --- | --- | ---: | ---: | ---: | ---: | ---: | --- |");

        var vtLines = new List<VehicleTaskLine>();
        foreach (var vtId in vehicleTaskIds)
        {
            vtLines.AddRange(await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(vtId));
        }

        if (vtLines.Count == 0)
        {
            LogLine("| none | - | - | 0 | 0 | 0 | 0 | 0 | - |");
            return;
        }

        var taskLineIds = vtLines.Select(x => x.TaskLineId).Distinct().ToList();
        var taskLines = await _taskLineRepository.GetListAsync(x => taskLineIds.Contains(x.Id));
        var productByTaskLineId = taskLines.ToDictionary(x => x.Id, x => x.ProductId);

        foreach (var line in vtLines.OrderBy(x => x.CreationTime))
        {
            var productId = productByTaskLineId.TryGetValue(line.TaskLineId, out var resolvedProductId)
                ? resolvedProductId
                : Guid.Empty;
            LogLine($"| {Short(line.Id)} | {Short(line.VehicleTaskId)} | {Short(productId)} | {line.AllocatedQuantity} | {line.ReceivedQuantity} | {line.DamagedQuantity} | {line.LostQuantity} | {line.ConsumedQuantity} | {line.ReceiveNote ?? "-"} |");
        }
    }

    private async Task WriteWorkflowAndApprovalTablesAsync(IReadOnlyCollection<Guid> requestIds)
    {
        var approvals = (await _movementApprovalRepository.GetListAsync(x => requestIds.Contains(x.MovementRequestId)))
            .OrderBy(x => x.MovementRequestId)
            .ThenBy(x => x.StepOrder)
            .ToList();

        LogLine("");
        LogLine("movement.movement_approvals");
        LogLine("| MovementRequestId | StepOrder | ApproverWorkerId | Status | DecidedAt | Note |");
        LogLine("| --- | ---: | --- | --- | --- | --- |");

        if (approvals.Count == 0)
        {
            LogLine("| none | 0 | - | - | - | - |");
        }
        else
        {
            foreach (var approval in approvals)
            {
                LogLine($"| {Short(approval.MovementRequestId)} | {approval.StepOrder} | {Short(approval.ApproverWorkerId)} | {approval.Status} | {approval.DecidedAt:HH:mm:ss} | {approval.Note ?? "-"} |");
            }
        }

        var requests = await _movementRequestRepository.GetListAsync(x => requestIds.Contains(x.Id));
        var workflowIds = requests
            .Where(x => x.WorkflowInstanceId.HasValue)
            .Select(x => x.WorkflowInstanceId!.Value)
            .Distinct()
            .ToList();

        LogLine("");
        LogLine("workflow.workflow_instances + workflow.workflow_instance_steps");
        LogLine("| WorkflowInstanceId | State | StepOrder | Resolver | AssignedUserId | Action | ActionDate | Note |");
        LogLine("| --- | --- | ---: | --- | --- | --- | --- | --- |");

        if (workflowIds.Count == 0)
        {
            LogLine("| none | - | 0 | - | - | - | - | - |");
            return;
        }

        var instances = await _workflowInstanceRepository.GetListAsync(x => workflowIds.Contains(x.Id));
        var steps = await _workflowInstanceStepRepository.GetListAsync(x => workflowIds.Contains(x.WorkflowInstanceId));
        var stepDefinitionIds = steps.Select(x => x.WorkflowStepDefinitionId).Distinct().ToList();
        var stepDefinitions = await _workflowStepDefinitionRepository.GetListAsync(x => stepDefinitionIds.Contains(x.Id));
        var stepDefinitionById = stepDefinitions.ToDictionary(x => x.Id);
        var instanceById = instances.ToDictionary(x => x.Id);

        foreach (var step in steps.OrderBy(x => stepDefinitionById[x.WorkflowStepDefinitionId].StepOrder))
        {
            var stepDefinition = stepDefinitionById[step.WorkflowStepDefinitionId];
            var instance = instanceById[step.WorkflowInstanceId];
            var resolverName = stepDefinition.ResolverKey ?? stepDefinition.RequiredRoleName ?? "-";

            LogLine($"| {Short(instance.Id)} | {instance.State} | {stepDefinition.StepOrder} | {resolverName} | {Short(step.AssignedUserId)} | {step.ActionTaken} | {FormatTime(step.ActionDate)} | {step.Note ?? "-"} |");
        }
    }

    private async Task WriteTransactionTableAsync(IReadOnlyCollection<Guid> requestIds)
    {
        var transactions = (await _inventoryTransactionRepository.GetListAsync(x =>
                x.RelatedMovementRequestId.HasValue &&
                requestIds.Contains(x.RelatedMovementRequestId.Value)))
            .OrderBy(x => x.OccurredAt)
            .ToList();

        LogLine("");
        LogLine("stock.inventory_transactions");
        LogLine("| TxId | Type | Quantity | Source | Target | RelatedMovementRequestId | Note |");
        LogLine("| --- | --- | ---: | --- | --- | --- | --- |");

        if (transactions.Count == 0)
        {
            LogLine("| none | - | 0 | - | - | - | - |");
            return;
        }

        foreach (var transaction in transactions)
        {
            var source = $"{transaction.SourceLocationType}:{Short(transaction.SourceLocationId)}";
            var target = $"{transaction.TargetLocationType}:{Short(transaction.TargetLocationId)}";
            LogLine($"| {Short(transaction.Id)} | {transaction.TransactionType} | {transaction.Quantity} | {source} | {target} | {Short(transaction.RelatedMovementRequestId)} | {transaction.Note ?? "-"} |");
        }
    }

    private static string LocationLabel(StockLocationTypeEnum locationType, Guid locationId, MovementScenario scenario)
    {
        if (locationType == StockLocationTypeEnum.Warehouse && locationId == scenario.SourceWarehouseId)
        {
            return "SourceWarehouse";
        }

        if (locationType == StockLocationTypeEnum.Warehouse && locationId == scenario.TargetWarehouseId)
        {
            return "TargetWarehouse";
        }

        if (locationType == StockLocationTypeEnum.Vehicle && locationId == scenario.VehicleId)
        {
            return "Vehicle";
        }

        return "Other";
    }

    private static string DescribeAction(string title)
    {
        return title switch
        {
            "WT-04 Workflow Approvals" =>
                "Uc adimli depo transfer workflow'unu gercek approver kullanicilariyla onayladik; movement_approvals ve workflow step izleri yazildi.",
            "FO-04 Workflow Approvals" =>
                "Iki adimli saha cikis workflow'unu gercek approver kullanicilariyla onayladik; arac yukleme adimina gecilebilir hale geldi.",
            "WT-01 Seed" =>
                "Test dunyasini kurduk: kaynak depo, hedef depo, arac, sorumlu worker, urun ve kaynak depo stogu actik.",
            "WT-02 Task" =>
                "WarehouseTransfer tipinde InventoryTask actik. Artik surecin depo transferi oldugunu task belirliyor.",
            "WT-03 Request" =>
                "8 adet urun icin MovementRequest actik. Sistem bu sirada araci task'a baglayan VehicleTask kaydini da acti.",
            "WT-04 Approve" =>
                "Workflow kismini simule edip talebi Approved durumuna cektik; dispatch icin kapıyı actik.",
            "WT-05 Dispatch" =>
                "Sevk yaptik: stok kaynak depodan araca indi, ledger'a WarehouseToVehicle hareketi yazildi.",
            "WT-06 Receive" =>
                "Teslim aldik: stok aractan hedef depoya gecti, hareket ve task tamamlandi, VehicleTask release edildi.",

            "FO-01 Seed" =>
                "Saha operasyonu icin test dunyasini kurduk: depo, arac, worker, urun ve kaynak stok hazirlandi.",
            "FO-02 Task" =>
                "FieldOperation tipinde InventoryTask actik ve iade deposunu ReturnWarehouseId olarak belirledik.",
            "FO-03 Main Request" =>
                "Sahaya cikacak 10 urun icin ana MovementRequest actik. Sistem VehicleTask kaydini olusturup talebe bagladi.",
            "FO-04 Approve" =>
                "Ana saha talebini Approved yaptik; arac yukleme adimina gecilebilir hale geldi.",
            "FO-05 Dispatch" =>
                "Saha sevki yaptik: 10 urun kaynak depodan araca gecti, WarehouseToVehicle ledger kaydi olustu.",
            "FO-06 Main Receive" =>
                "Saha teslimini kapattik; FieldOperation kuralindan dolayi stok depoya inmedi, aracta kaldi.",
            "FO-07 Task Completed" =>
                "Saha gorevini Completed yaptik; bu andan sonra aractaki kalan stok icin iade sureci dogacak.",
            "FO-08 Return Request" =>
                "TaskReturnRequestManager calisti ve ana harekete bagli iade MovementRequest kaydini acti.",
            "FO-09 Return Receive" =>
                "Iade teslimini saydik: 7 saglam depoya dondu, 1 hasarli ve 2 tuketilen stoktan adjustment ile dustu.",

            "WT-INS-01 Seed" =>
                "Yetersiz stok senaryosunu kurduk: kaynak depoya sadece 5 adet stok koyduk.",
            "WT-INS-02 Approved Request" =>
                "10 adet isteyen warehouse transfer talebini actik ve Approved yaptik; bilerek stoktan fazla istiyoruz.",
            "WT-INS-03 Dispatch Blocked" =>
                "Dispatch denedik ama is kurali durdurdu; stok ve transaction tablolarinin degismedigini kontrol ettik.",

            "FO-DUP-01 Seed" =>
                "Duplicate iade senaryosu icin saha operasyonu test dunyasini kurduk.",
            "FO-DUP-02 Main Movement Completed" =>
                "Ana saha hareketini tamamladik; 10 adet stok aracta kaldi ve iade bekliyor.",
            "FO-DUP-03 First Return Request" =>
                "Ilk iade talebini olusturduk; hareket ana talebe ParentMovementRequestId ile baglandi.",
            "FO-DUP-04 Second Return Create Attempt" =>
                "Ayni iade olusturma islemini tekrar calistirdik; sistem acik iade talebini gorup ikinci kaydi acmadi.",

            _ => "Bu adim icin tablo snapshot'i alindi."
        };
    }

    private static Guid ResolveFallbackApproverUserId(WorkflowStepDefinition stepDefinition, MovementScenario scenario)
    {
        return stepDefinition.ResolverKey switch
        {
            "InitiatorManager" => scenario.InitiatorManagerUserId,
            "LogisticsManager" => scenario.SourceWarehouseManagerUserId,
            "SourceWarehouseManager" => scenario.SourceWarehouseManagerUserId,
            "TargetWarehouseManager" => scenario.TargetWarehouseManagerUserId,
            _ => scenario.UserId
        };
    }

    private static string ActorLabel(Guid userId, MovementScenario scenario)
    {
        if (userId == scenario.UserId)
        {
            return "Requester";
        }

        if (userId == scenario.InitiatorManagerUserId)
        {
            return "InitiatorManager";
        }

        if (userId == scenario.SourceWarehouseManagerUserId)
        {
            return "SourceWarehouseManager/LogisticsManager";
        }

        if (userId == scenario.TargetWarehouseManagerUserId)
        {
            return "TargetWarehouseManager";
        }

        return "UnknownApprover";
    }

    private static string Short(Guid? id)
    {
        return id.HasValue ? id.Value.ToString("N")[..8] : "-";
    }

    private static string FormatTime(DateTime? value)
    {
        return value.HasValue ? value.Value.ToString("HH:mm:ss") : "active";
    }

    private sealed record MovementScenario(
        Guid SourceWarehouseId,
        Guid TargetWarehouseId,
        Guid VehicleId,
        Guid WorkerId,
        Guid UserId,
        Guid ProductId,
        Guid InitiatorManagerWorkerId,
        Guid InitiatorManagerUserId,
        Guid SourceWarehouseManagerWorkerId,
        Guid SourceWarehouseManagerUserId,
        Guid TargetWarehouseManagerWorkerId,
        Guid TargetWarehouseManagerUserId);

    private sealed record TestActor(Guid WorkerId, Guid UserId);
}

