using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// InventoryTask domain manager'i - gorev is kurallari ve validasyonlari.
/// </summary>
public class InventoryTaskManager : BaseManager<InventoryTask>
{
    private IMapper _mapper => LazyGetRequiredService<IMapper>();
    private IWarehouseRepository _warehouseRepository => LazyGetRequiredService<IWarehouseRepository>();
    private TaskLineManager _taskLineManager => LazyGetRequiredService<TaskLineManager>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IInventoryTaskRepository _inventoryTaskRepository => (IInventoryTaskRepository)Repository;

    public InventoryTaskManager(IInventoryTaskRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Envanter gorevini kalemleri ile birlikte atomik olarak olusturur.
    /// </summary>
    public async Task<InventoryTask> CreateWithLinesAsync(CreateInventoryTaskModel model, List<CreateTaskLineModel> lines)
    {
        var task = await CreateAsync(model);
        var insertedTask = await _inventoryTaskRepository.InsertAsync(task, autoSave: true);

        if (lines != null)
        {
            foreach (var lineModel in lines)
            {
                var line = await _taskLineManager.CreateAsync(insertedTask.Id, lineModel);
                await _taskLineRepository.InsertAsync(line, autoSave: true);
            }
        }

        return insertedTask;
    }

    /// <summary>
    /// Yeni bir envanter görevi oluşturmak için kullanılır.
    /// </summary>
    public async Task<InventoryTask> CreateAsync(CreateInventoryTaskModel model)
    {
        await ValidateCodeForCreateAsync(model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);
        await ValidateRouteAsync(model.Type, model.SourceWarehouseId, model.TargetWarehouseId, model.ReturnWarehouseId);

        var entity = new InventoryTask(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Mevcut bir envanter görevini güncellemek için kullanılır.
    /// </summary>
    public async Task<InventoryTask> UpdateAsync(InventoryTask existing, UpdateInventoryTaskModel model)
    {
        await ValidateCodeForUpdateAsync(existing, model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);
        await ValidateRouteAsync(model.Type, model.SourceWarehouseId, model.TargetWarehouseId, model.ReturnWarehouseId);

        _mapper.Map(model, existing);
        return existing;
    }

    /// <summary>
    /// Gorev guncellemesini ve status gecisini koordine eder.
    /// </summary>
    public async Task<InventoryTask> UpdateWithStatusAsync(
        Guid id, 
        UpdateInventoryTaskModel model, 
        InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum targetStatus,
        Volo.Abp.EventBus.Local.ILocalEventBus localEventBus,
        Guid currentUserId,
        Guid currentWorkerId)
    {
        var existing = await EnsureExistsAsync(id);
        var updated = await UpdateAsync(existing, model);

        if (updated.Status != targetStatus)
        {
            await TransitionStatusAsync(
                updated,
                targetStatus,
                localEventBus,
                currentUserId,
                currentWorkerId);
        }

        return await _inventoryTaskRepository.UpdateAsync(updated, autoSave: true);
    }

    /// <summary>
    /// Görev kodunu oluşturma aşamasında doğrulamak için kullanılır.
    /// </summary>
    private async Task ValidateCodeForCreateAsync(string code)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            await EnsureUniqueAsync(x => x.Code == code);
        }
    }

    /// <summary>
    /// Görev kodunu güncelleme aşamasında doğrulamak için kullanılır.
    /// </summary>
    private async Task ValidateCodeForUpdateAsync(InventoryTask existing, string code)
    {
        if (!string.IsNullOrWhiteSpace(code) && existing.Code != code)
        {
            await EnsureUniqueAsync(x => x.Code == code, existing.Id);
        }
    }

    /// <summary>
    /// Tarih aralığını doğrulamak için kullanılır.
    /// </summary>
    private static void ValidateDateRange(System.DateTime startDate, System.DateTime? endDate)
    {
        if (endDate.HasValue && endDate.Value < startDate)
        {
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
        }
    }

    /// <summary>
    /// Operasyon rota alanlarini task seviyesinde dogrulamak icin kullanilir.
    /// </summary>
    private async Task ValidateRouteAsync(
        InventoryTaskTypeEnum type,
        Guid sourceWarehouseId,
        Guid? targetWarehouseId,
        Guid? returnWarehouseId)
    {
        if (sourceWarehouseId == Guid.Empty)
        {
            throw new BusinessException(WarehouseExceptionCodes.NotFound)
                .WithData("SourceWarehouseId", sourceWarehouseId);
        }

        await EnsureExistsInAsync(_warehouseRepository, sourceWarehouseId);

        if (targetWarehouseId.HasValue && targetWarehouseId.Value != Guid.Empty)
        {
            await EnsureExistsInAsync(_warehouseRepository, targetWarehouseId.Value);
        }

        if (returnWarehouseId.HasValue && returnWarehouseId.Value != Guid.Empty)
        {
            await EnsureExistsInAsync(_warehouseRepository, returnWarehouseId.Value);
        }

        if (type == InventoryTaskTypeEnum.WarehouseTransfer)
        {
            if (!targetWarehouseId.HasValue || targetWarehouseId.Value == Guid.Empty)
            {
                throw new BusinessException(MovementRequestExceptionCodes.TargetRequired);
            }

            if (targetWarehouseId.Value == sourceWarehouseId)
            {
                throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair)
                    .WithData("SourceWarehouseId", sourceWarehouseId)
                    .WithData("TargetWarehouseId", targetWarehouseId.Value);
            }
        }
    }

    /// <summary>
    /// Görev durum geçişini gerçekleştirmek için kullanılır.
    /// </summary>
    public async Task TransitionStatusAsync(
        InventoryTask task,
        InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum target,
        Volo.Abp.EventBus.Local.ILocalEventBus localEventBus,
        System.Guid? changedByUserId = null,
        System.Guid? changedByWorkerId = null)
    {
        if (task.Status == target) return;

        var previous = task.Status;

        var allowed = (task.Status, target) switch
        {
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Draft, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Completed) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Draft, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Cancelled) => true,
            (InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.InProgress, InventoryTrackingAutomation.Enums.Tasks.TaskStatusEnum.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation)
                .WithData("From", task.Status).WithData("To", target);

        task.Status = target;

        await localEventBus.PublishAsync(new InventoryTrackingAutomation.Events.Tasks.InventoryTaskStatusChangedEto
        {
            TaskId = task.Id,
            PreviousStatus = previous,
            NewStatus = target,
            ChangedByUserId = changedByUserId,
            ChangedByWorkerId = changedByWorkerId
        });
    }
}
