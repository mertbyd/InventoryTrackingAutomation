using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Modularity;
using Xunit;

namespace InventoryTrackingAutomation.Tasks;

public abstract class InventoryTaskManager_Tests<TStartupModule> : InventoryTrackingAutomationDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly InventoryTaskManager _manager;
    private readonly IInventoryTaskRepository _repository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly ILocalEventBus _localEventBus;

    protected InventoryTaskManager_Tests()
    {
        _manager = GetRequiredService<InventoryTaskManager>();
        _repository = GetRequiredService<IInventoryTaskRepository>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
        _localEventBus = GetRequiredService<ILocalEventBus>();
    }

    [Fact]
    public async Task Create_Should_Throw_When_EndDate_Before_StartDate()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var model = new CreateInventoryTaskModel
            {
                Code = $"TSK-DATE-{Guid.NewGuid():N}"[..30],
                Name = "Date Validation Test",
                Type = InventoryTaskTypeEnum.FieldOperation,
                Status = TaskStatusEnum.Draft,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.UtcNow.AddDays(-1)
            };

            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _manager.CreateAsync(model);
            });
        });
    }

    [Fact]
    public async Task Create_Should_Throw_When_Duplicate_Code()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var sourceWarehouse = await InsertWarehouseAsync("DUP-SRC");
            var targetWarehouse = await InsertWarehouseAsync("DUP-TRG");
            var code = $"TSK-UNQ-{Guid.NewGuid():N}"[..30];

            var firstTask = await _manager.CreateAsync(new CreateInventoryTaskModel
            {
                Code = code,
                Name = "Ilk gorev",
                Type = InventoryTaskTypeEnum.WarehouseTransfer,
                Status = TaskStatusEnum.Draft,
                SourceWarehouseId = sourceWarehouse.Id,
                TargetWarehouseId = targetWarehouse.Id,
                StartDate = DateTime.UtcNow
            });

            await _repository.InsertAsync(firstTask, autoSave: true);

            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _manager.CreateAsync(new CreateInventoryTaskModel
                {
                    Code = code,
                    Name = "Ikinci gorev",
                    Type = InventoryTaskTypeEnum.FieldOperation,
                    Status = TaskStatusEnum.Draft,
                    SourceWarehouseId = sourceWarehouse.Id,
                    StartDate = DateTime.UtcNow
                });
            });
        });
    }

    [Fact]
    public async Task TransitionStatus_Draft_To_InProgress_Should_Succeed()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var sourceWarehouse = await InsertWarehouseAsync("TRN-SRC");
            var task = new InventoryTask(Guid.NewGuid())
            {
                Code = $"TSK-TRN-{Guid.NewGuid():N}"[..30],
                Name = "Transition Test",
                Type = InventoryTaskTypeEnum.FieldOperation,
                Status = TaskStatusEnum.Draft,
                SourceWarehouseId = sourceWarehouse.Id,
                StartDate = DateTime.UtcNow
            };

            await _repository.InsertAsync(task, autoSave: true);

            await _manager.TransitionStatusAsync(task, TaskStatusEnum.InProgress, _localEventBus);

            Assert.Equal(TaskStatusEnum.InProgress, task.Status);
        });
    }

    [Fact]
    public async Task TransitionStatus_InProgress_To_Draft_Should_Throw()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var sourceWarehouse = await InsertWarehouseAsync("BACK-SRC");
            var targetWarehouse = await InsertWarehouseAsync("BACK-TRG");
            var task = new InventoryTask(Guid.NewGuid())
            {
                Code = $"TSK-BACK-{Guid.NewGuid():N}"[..30],
                Name = "Invalid Transition Test",
                Type = InventoryTaskTypeEnum.WarehouseTransfer,
                Status = TaskStatusEnum.InProgress,
                SourceWarehouseId = sourceWarehouse.Id,
                TargetWarehouseId = targetWarehouse.Id,
                StartDate = DateTime.UtcNow
            };

            await _repository.InsertAsync(task, autoSave: true);

            await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _manager.TransitionStatusAsync(task, TaskStatusEnum.Draft, _localEventBus);
            });
        });
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
