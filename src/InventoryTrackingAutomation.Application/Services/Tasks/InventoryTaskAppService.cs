using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
using Microsoft.Extensions.Caching.Distributed;
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
    private IDistributedCache _cache => LazyGetRequiredService<IDistributedCache>();
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    /// <summary>
    /// Belirli bir envanter görevini getirir.
    /// </summary>
    public async Task<InventoryTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(entity);
    }

    /// <summary>
    /// Sayfalanmış envanter görevi listesini getirir.
    /// </summary>
    public async Task<PagedResultDto<InventoryTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTaskDto>(totalCount, _mapper.Map<List<InventoryTask>, List<InventoryTaskDto>>(entities));
    }

    /// <summary>
    /// Göreve atanmış araçları getirir (Cache destekli).
    /// </summary>
    public async Task<List<TaskVehicleDto>> GetVehiclesAsync(Guid id)
    {
        var cacheKey = CacheKeys.TaskVehicles(id);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<TaskVehicleDto>>(cached)!;

        var vehicles = await _inventoryQueryManager.GetTaskVehiclesAsync(id);
        var dto = _mapper.Map<List<TaskVehicleModel>, List<TaskVehicleDto>>(vehicles);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

        return dto;
    }

    /// <summary>
    /// Görevin mevcut envanter durumunu getirir (Cache destekli).
    /// </summary>
    public async Task<List<TaskInventoryDto>> GetInventoryAsync(Guid id)
    {
        var cacheKey = CacheKeys.TaskInventory(id);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<TaskInventoryDto>>(cached)!;

        var inventory = await _inventoryQueryManager.GetTaskInventoryAsync(id);
        var dto = _mapper.Map<List<TaskInventoryModel>, List<TaskInventoryDto>>(inventory);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

        return dto;
    }

    /// <summary>
    /// Yeni bir envanter görevini kalemleri ile birlikte oluşturur.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CreateAsync(CreateInventoryTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        
        var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(input);
        var lines = _mapper.Map<List<CreateTaskLineDto>, List<CreateTaskLineModel>>(input.Lines);

        var inserted = await _manager.CreateWithLinesAsync(model, lines);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(inserted);
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
            
            var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(dto);
            var lines = _mapper.Map<List<CreateTaskLineDto>, List<CreateTaskLineModel>>(dto.Lines);
            
            result.Add(await _manager.CreateWithLinesAsync(model, lines));
        }

        return _mapper.Map<List<InventoryTask>, List<InventoryTaskDto>>(result);
    }

    /// <summary>
    /// Envanter görevini günceller ve gerekirse durum geçişini koordine eder.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> UpdateAsync(Guid id, UpdateInventoryTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        
        var model = _mapper.Map<UpdateInventoryTaskDto, UpdateInventoryTaskModel>(input);
        var updated = await _manager.UpdateWithStatusAsync(
            id, 
            model, 
            input.Status, 
            _localEventBus, 
            CurrentUser.GetId(), 
            await ResolveCurrentWorkerIdAsync());

        return _mapper.Map<InventoryTask, InventoryTaskDto>(updated);
    }

    /// <summary>
    /// Envanter görevini tamamlandı durumuna çeker.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CompleteAsync(Guid id)
    {
        var updated = await _manager.UpdateWithStatusAsync(
            id, 
            new UpdateInventoryTaskModel(), // Sadece statu degisimi icin bos model
            TaskStatusEnum.Completed, 
            _localEventBus, 
            CurrentUser.GetId(), 
            await ResolveCurrentWorkerIdAsync());

        await InvalidateTaskCachesAsync(id);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(updated);
    }

    /// <summary>
    /// Envanter görevini iptal durumuna çeker.
    /// </summary>
    [UnitOfWork]
    public async Task<InventoryTaskDto> CancelAsync(Guid id)
    {
        var updated = await _manager.UpdateWithStatusAsync(
            id, 
            new UpdateInventoryTaskModel(), 
            TaskStatusEnum.Cancelled, 
            _localEventBus, 
            CurrentUser.GetId(), 
            await ResolveCurrentWorkerIdAsync());

        await InvalidateTaskCachesAsync(id);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(updated);
    }

    /// <summary>
    /// Envanter görevini siler (Soft Delete).
    /// </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
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
}
