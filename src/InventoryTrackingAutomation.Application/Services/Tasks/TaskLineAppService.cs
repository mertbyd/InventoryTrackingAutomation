using InventoryTrackingAutomation.Application.Mappers.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.ExceptionCodes;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

/// <summary>
/// Görev kalemi application servisi.
/// </summary>
//işlevi: TaskLine işlemlerini HTTP/API katmanından domain manager'a taşır.
//sistemdeki görevi: Görev ürün ihtiyacını ve araç tahsislerinden türeyen kalan miktarı API yüzeyine taşır.
public class TaskLineAppService : InventoryTrackingAutomationAppService, ITaskLineAppService
{
    public TaskLineAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private ITaskLineRepository _repository => LazyGetRequiredService<ITaskLineRepository>();
    private IInventoryTaskRepository _taskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private TaskLineManager _manager => LazyGetRequiredService<TaskLineManager>();
    private IValidator<CreateTaskLineDto> _createValidator => LazyGetRequiredService<IValidator<CreateTaskLineDto>>();
    private IValidator<UpdateTaskLineDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateTaskLineDto>>();
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    private static readonly TaskLineMapper _mapper = new TaskLineMapper();

    /// Görev kalemini Id ile getirmek için kullanılır.
    public async Task<TaskLineDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return await MapTaskLineAsync(entity);
    }

    /// Bir göreve bağlı tüm kalemleri getirmek için kullanılır.
    public async Task<List<TaskLineDto>> GetByTaskAsync(Guid taskId)
    {
        await EnsureTaskExistsAsync(taskId);
        var lines = await _manager.GetByTaskIdAsync(taskId);
        return await MapTaskLinesAsync(lines);
    }

    /// Göreve yeni ürün kalemi eklemek için kullanılır.
    [UnitOfWork]
    public async Task<TaskLineDto> CreateForTaskAsync(Guid taskId, CreateTaskLineDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        await EnsureTaskExistsAsync(taskId);

        var model = _mapper.MapToModel(input);
        model.TaskId = taskId;

        var validatedModel = await _manager.CreateAsync(taskId, model);
        var entity = new TaskLine(GuidGenerator.Create());
        entity.TaskId = taskId;
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        await InvalidateTaskLineCachesAsync(taskId);
        return await MapTaskLineAsync(inserted);
    }

    /// Görev kalemi miktarını veya ürün bağlamını güncellemek için kullanılır.
    [UnitOfWork]
    public async Task<TaskLineDto> UpdateAsync(Guid taskId, Guid lineId, UpdateTaskLineDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await EnsureTaskLineBelongsToTaskAsync(taskId, lineId);

        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        await InvalidateTaskLineCachesAsync(taskId);
        return await MapTaskLineAsync(saved);
    }

    /// Tahsis edilmemis gorev kalemini silmek icin kullanilir.
    [UnitOfWork]
    public async Task DeleteAsync(Guid taskId, Guid lineId)
    {
        await EnsureTaskLineBelongsToTaskAsync(taskId, lineId);
        await _manager.DeleteAsync(lineId);
        await InvalidateTaskLineCachesAsync(taskId);
    }

    private async Task EnsureTaskExistsAsync(Guid taskId)
    {
        var task = await _taskRepository.FindAsync(taskId);
        if (task == null)
        {
            throw new BusinessException(InventoryTaskExceptionCodes.NotFound)
                .WithData("TaskId", taskId);
        }
    }

    private async Task<TaskLine> EnsureTaskLineBelongsToTaskAsync(Guid taskId, Guid lineId)
    {
        var existing = await _manager.EnsureExistsAsync(lineId);
        if (existing.TaskId != taskId)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound)
                .WithData("TaskId", taskId)
                .WithData("TaskLineId", lineId);
        }

        return existing;
    }

    private Task InvalidateTaskLineCachesAsync(Guid taskId)
    {
        return _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(
            CacheKeys.TaskInventory(taskId),
            CacheKeys.TaskVehicles(taskId)));
    }

    private async Task<TaskLineDto> MapTaskLineAsync(TaskLine entity)
    {
        var dto = _mapper.MapToDto(entity);
        dto.AllocatedQuantity = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(entity.Id);
        return dto;
    }

    private async Task<List<TaskLineDto>> MapTaskLinesAsync(List<TaskLine> entities)
    {
        var dtos = _mapper.MapToDto(entities);
        var taskLineIds = entities.Select(x => x.Id).ToList();
        var vehicleLines = await _vehicleTaskLineRepository.GetByTaskLineIdsAsync(taskLineIds);
        var allocationsByTaskLineId = vehicleLines
            .GroupBy(x => x.TaskLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.AllocatedQuantity));

        foreach (var dto in dtos)
        {
            dto.AllocatedQuantity = allocationsByTaskLineId.TryGetValue(dto.Id, out var allocatedQuantity)
                ? allocatedQuantity
                : 0;
        }

        return dtos;
    }
}
