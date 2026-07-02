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
    protected override string AlreadyExistsErrorCode => InventoryTaskExceptionCodes.CodeNotUnique;

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
    /// Yeni bir envanter görevini oluşturmak için kullanılır.
    /// </summary>
    public async Task<CreateInventoryTaskModel> CreateAsync(CreateInventoryTaskModel model)
    {
        await ValidateCodeForCreateAsync(model.Code);
        ValidateDateRange(model.StartDate, model.EndDate);
        await ValidateRouteAsync(model.Type, model.SourceWarehouseId, model.TargetWarehouseId, model.ReturnWarehouseId);

        return model;
    }

    /// <summary>
    /// Birden fazla envanter görevini toplu oluşturmak ve doğrulamak için kullanılır.
    /// </summary>
    public async Task<List<CreateInventoryTaskModel>> CreateManyAsync(List<CreateInventoryTaskModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Any())
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }

        var warehouseIds = new HashSet<Guid>();
        foreach (var model in models)
        {
            ValidateDateRange(model.StartDate, model.EndDate);
            if (model.SourceWarehouseId == Guid.Empty) throw new BusinessException(WarehouseExceptionCodes.NotFound);
            
            warehouseIds.Add(model.SourceWarehouseId);
            if (model.TargetWarehouseId.HasValue && model.TargetWarehouseId.Value != Guid.Empty) warehouseIds.Add(model.TargetWarehouseId.Value);
            if (model.ReturnWarehouseId.HasValue && model.ReturnWarehouseId.Value != Guid.Empty) warehouseIds.Add(model.ReturnWarehouseId.Value);

            if (model.Type == InventoryTaskTypeEnum.WarehouseTransfer)
            {
                if (!model.TargetWarehouseId.HasValue || model.TargetWarehouseId.Value == Guid.Empty)
                    throw new BusinessException(MovementRequestExceptionCodes.TargetRequired);
                if (model.TargetWarehouseId.Value == model.SourceWarehouseId)
                    throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair);
            }
        }

        if (warehouseIds.Any())
        {
            await EnsureAllExistInAsync(_warehouseRepository, warehouseIds);
        }

        return models;
    }

    /// <summary>
    /// Mevcut bir envanter görevini güncellemek için kullanılır.
    /// </summary>
    public async Task<UpdateInventoryTaskModel> UpdateAsync(InventoryTask existing, UpdateInventoryTaskModel model)
    {
        if (existing.Status == TaskStatusEnum.Completed ||
            existing.Status == TaskStatusEnum.Cancelled)
        {
            throw new BusinessException(InventoryTaskExceptionCodes.CannotUpdateCompletedOrCancelled);
        }

        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(x => x.Code == model.Code, existing.Id);
        }

        ValidateDateRange(model.StartDate, model.EndDate);
        await ValidateRouteAsync(model.Type, model.SourceWarehouseId, model.TargetWarehouseId, model.ReturnWarehouseId);

        return model;
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
            throw new BusinessException(WarehouseExceptionCodes.NotFound);
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
                throw new BusinessException(InventoryTransactionExceptionCodes.InvalidLocationPair);
            }
        }
    }

    /// <summary>
    /// Görev durum geçişini gerçekleştirmek için kullanılır.
    /// </summary>
    public async Task TransitionStatusAsync(
        InventoryTask task,
        TaskStatusEnum target,
        Volo.Abp.EventBus.Local.ILocalEventBus localEventBus,
        Guid? changedByUserId = null,
        Guid? changedByWorkerId = null)
    {
        if (task.Status == target) return;

        var previous = task.Status;

        var allowed = (task.Status, target) switch
        {
            (TaskStatusEnum.Draft, TaskStatusEnum.InProgress) => true,
            (TaskStatusEnum.InProgress, TaskStatusEnum.Completed) => true,
            (TaskStatusEnum.Draft, TaskStatusEnum.Cancelled) => true,
            (TaskStatusEnum.InProgress, TaskStatusEnum.Cancelled) => true,
            _ => false
        };

        if (!allowed)
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);

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


