using InventoryTrackingAutomation.Application.Mappers.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Caching;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

/// <summary>
/// Envanter gorevi uygulama servisi.
/// Is kuralları InventoryTaskManager ve TaskLineManager tarafından yonetilir.
/// </summary>
public class InventoryTaskAppService : InventoryTrackingAutomationAppService, IInventoryTaskAppService
{
    public InventoryTaskAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IInventoryTaskRepository _repository => LazyGetRequiredService<IInventoryTaskRepository>();
    private InventoryTaskManager _manager => LazyGetRequiredService<InventoryTaskManager>();
    private ITaskLineAppService _taskLineAppService => LazyGetRequiredService<ITaskLineAppService>();
    private InventoryQueryManager _inventoryQueryManager => LazyGetRequiredService<InventoryQueryManager>();
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    private IValidator<CreateInventoryTaskDto> _createValidator => LazyGetRequiredService<IValidator<CreateInventoryTaskDto>>();
    private IValidator<UpdateInventoryTaskDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateInventoryTaskDto>>();
    private static readonly InventoryTaskMapper _mapper = new InventoryTaskMapper();
    private static readonly TaskLineMapper _taskLineMapper = new TaskLineMapper();

    /// <summary>
    /// Belirli bir envanter görevini getirir.
    /// </summary>
    public async Task<InventoryTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return await MapTaskWithLinesAsync(entity);
    }

    /// <summary>
    /// Sayfalanmış envanter görevi listesini getirir.
    /// </summary>
    public async Task<PagedResultDto<InventoryTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTaskDto>(totalCount, _mapper.MapToDto(entities));
    }

    /// <summary>
    /// Göreve atanmış araçları getirir (attribute cache destekli).
    /// </summary>
    // CacheKeys.TaskVehiclesTemplate invalidation tarafindaki TaskVehicles key'i ile ayni sozlesmeyi kullanir.
    [InventoryCache(CacheKeys.TaskVehiclesTemplate, 10)]
    public async Task<List<TaskVehicleDto>> GetVehiclesAsync(Guid id)
    {
        var vehicles = await _inventoryQueryManager.GetTaskVehiclesAsync(id);
        return _mapper.MapToDto(vehicles);
    }

    /// <summary>
    /// Görevin mevcut envanter durumunu getirir (attribute cache destekli).
    /// </summary>
    // CacheKeys.TaskInventoryTemplate invalidation tarafindaki TaskInventory key'i ile ayni sozlesmeyi kullanir.
    [InventoryCache(CacheKeys.TaskInventoryTemplate, 10)]
    public async Task<List<TaskInventoryDto>> GetInventoryAsync(Guid id)
    {
        var inventory = await _inventoryQueryManager.GetTaskInventoryAsync(id);
        return _mapper.MapToDto(inventory);
    }

    /// <summary>
    /// Yeni bir envanter görevini kalemleri ile birlikte oluşturur.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CreateAsync(CreateInventoryTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);

        var model = _mapper.MapToModel(input);
        var lines = _mapper.MapToModel(input.Lines ?? new List<CreateTaskLineDto>());

        var validatedModel = await _manager.CreateAsync(model);
        var taskEntity = new InventoryTask(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, taskEntity);
        var insertedTask = await _repository.InsertAsync(taskEntity, autoSave: true);

        if (lines != null)
        {
            foreach (var lineDto in input.Lines ?? new List<CreateTaskLineDto>())
            {
                await _taskLineAppService.CreateForTaskAsync(insertedTask.Id, lineDto);
            }
        }
        
        return await MapTaskWithLinesAsync(insertedTask);
    }

    /// <summary>
    /// Birden fazla envanter görevini toplu olarak oluşturur.
    /// </summary>
    [UnitOfWork]
    public async Task<List<InventoryTaskDto>> CreateManyAsync(List<CreateInventoryTaskDto> inputs)
    {
        var result = new List<InventoryTask>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);

            var model = _mapper.MapToModel(dto);
            var lines = _mapper.MapToModel(dto.Lines ?? new List<CreateTaskLineDto>());

            var validatedModel = await _manager.CreateAsync(model);
            var taskEntity = new InventoryTask(GuidGenerator.Create());
            _mapper.MapToEntity(validatedModel, taskEntity);
            var insertedTask = await _repository.InsertAsync(taskEntity, autoSave: true);

            if (lines != null)
            {
                foreach (var lineDto in dto.Lines ?? new List<CreateTaskLineDto>())
                {
                    await _taskLineAppService.CreateForTaskAsync(insertedTask.Id, lineDto);
                }
            }
            
            result.Add(insertedTask);
        }

        return _mapper.MapToDto(result);
    }

    /// <summary>
    /// Envanter görevini günceller ve gerekirse durum geçişini koordine eder.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> UpdateAsync(Guid id, UpdateInventoryTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);

        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        
        var validatedModel = await _manager.UpdateAsync(existing, model);
        
        if (existing.Status != input.Status)
        {
            await _manager.TransitionStatusAsync(
                existing,
                input.Status,
                _localEventBus,
                CurrentUser.GetId(),
                await ResolveCurrentWorkerIdAsync());
        }

        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    /// <summary>
    /// Envanter görevini tamamlandı durumuna çeker.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CompleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        
        await _manager.TransitionStatusAsync(
            existing,
            TaskStatusEnum.Completed,
            _localEventBus,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        await InvalidateTaskCachesAsync(id);
        return _mapper.MapToDto(saved);
    }

    /// <summary>
    /// Envanter görevini iptal durumuna çeker.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CancelAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);

        await _manager.TransitionStatusAsync(
            existing,
            TaskStatusEnum.Cancelled,
            _localEventBus,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        await InvalidateTaskCachesAsync(id);
        return _mapper.MapToDto(saved);
    }

    /// <summary>
    /// Envanter görevini siler (Soft Delete).
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.DeleteAsync(id);
    }

    // ────────────────────── Lines (Koordinasyon) ──────────────────────

    /// <summary>
    /// Görev kalemlerini getirir.
    /// </summary>
    public async Task<List<TaskLineDto>> GetLinesAsync(Guid taskId)
    {
        return await _taskLineAppService.GetByTaskAsync(taskId);
    }

    /// <summary>
    /// Göreve yeni bir kalem ekler.
    /// </summary>
    [UnitOfWork]
    public async Task<TaskLineDto> AddLineAsync(Guid taskId, CreateTaskLineDto input)
    {
        return await _taskLineAppService.CreateForTaskAsync(taskId, input);
    }

    /// <summary>
    /// Mevcut bir görev kalemini günceller.
    /// </summary>
    [UnitOfWork]
    public async Task<TaskLineDto> UpdateLineAsync(Guid taskId, Guid lineId, UpdateTaskLineDto input)
    {
        return await _taskLineAppService.UpdateAsync(taskId, lineId, input);
    }

    /// <summary>
    /// Görev kalemini siler.
    /// </summary>
    [UnitOfWork]
    public async Task DeleteLineAsync(Guid taskId, Guid lineId)
    {
        await _taskLineAppService.DeleteAsync(taskId, lineId);
    }

    /// <summary>
    /// Göreve ait cache kayıtlarını temizler.
    /// </summary>
    private Task InvalidateTaskCachesAsync(Guid taskId)
    {
        return _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(
            CacheKeys.TaskInventory(taskId),
            CacheKeys.TaskVehicles(taskId)));
    }

    private async Task<InventoryTaskDto> MapTaskWithLinesAsync(InventoryTask entity)
    {
        var dto = _mapper.MapToDto(entity);
        dto.Lines = await _taskLineAppService.GetByTaskAsync(entity.Id);
        return dto;
    }

    private async Task<UpdateInventoryTaskModel> BuildCurrentModelAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return _mapper.MapToModel(entity);
    }
}

