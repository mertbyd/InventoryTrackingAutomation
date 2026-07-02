using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Application.Mappers.Movements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Movements;

// Hareket talebi application servisi — HTTP endpoint'leri için ince orkestra katmanı.
// İş kuralları manager'da, persist sorumluluğu manager + repository'de.
//işlevi: MovementRequest iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class MovementRequestAppService : InventoryTrackingAutomationAppService, IMovementRequestAppService
{
    public MovementRequestAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list ve update/delete persist için ana repository.
    private IMovementRequestRepository _repository => LazyGetRequiredService<IMovementRequestRepository>();
    private InventoryTrackingAutomation.Interface.Tasks.IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<InventoryTrackingAutomation.Interface.Tasks.IVehicleTaskLineRepository>();
    private InventoryTrackingAutomation.Interface.Tasks.ITaskLineRepository _taskLineRepository => LazyGetRequiredService<InventoryTrackingAutomation.Interface.Tasks.ITaskLineRepository>();
    // Domain manager — iş kuralları, validasyon, workflow tetikleme.
    private MovementRequestManager _manager => LazyGetRequiredService<MovementRequestManager>();
    // Cache temizleme eventleri uygulama katmanindan local event bus ile yayinlanir.
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    // Tüm bağımlılıkları DI ile alır.
    private static readonly MovementRequestMapper _mapper = new MovementRequestMapper();
    /// Hareket talebi verisini getirmek için kullanılır.
    public async Task<MovementRequestDto> GetAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        var entity = await _repository.GetAsync(id, includeDetails: true);
        return await MapToDtoAsync(entity);
    }
    /// Hareket talebi listesini getirmek için kullanılır.
    public async Task<PagedResultDto<MovementRequestDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty, includeDetails: true);
        return new PagedResultDto<MovementRequestDto>(
            totalCount,
            await MapToDtosAsync(entities));
    }
    /// Yeni bir hareket talebi oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input)
    {
        var currentUserId = CurrentUser.GetId();
        var currentWorkerId = await ResolveCurrentWorkerIdAsync();
        var model = _mapper.MapToModel(input);
        model.RequestedByWorkerId = currentWorkerId;
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new MovementRequest(GuidGenerator.Create());
        entity.Status = MovementStatusEnum.Pending;
        _mapper.MapToEntity(validatedModel, entity);

        var workflowInstance = await _manager.AssignWorkflowAsync(entity, currentUserId);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        
        if (workflowInstance != null)
        {
            await _manager.PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        }

        return await MapToDtoAsync(inserted);
    }
    /// Birden fazla hareket talebini toplu olarak oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task<List<MovementRequestDto>> CreateManyAsync(List<CreateMovementRequestDto> inputs)
    {
        var currentUserId = CurrentUser.GetId();
        var currentWorkerId = await ResolveCurrentWorkerIdAsync();

        var models = new List<CreateMovementRequestModel>();
        foreach (var dto in inputs)
        {
            var model = _mapper.MapToModel(dto);
            model.RequestedByWorkerId = currentWorkerId;
            models.Add(model);
        }

        var validatedModels = await _manager.CreateManyAsync(models);

        var entities = new List<MovementRequest>();
        var workflowInstances = new List<WorkflowInstance>();

        foreach (var model in validatedModels)
        {
            var entity = new MovementRequest(GuidGenerator.Create());
            entity.Status = MovementStatusEnum.Pending;
            _mapper.MapToEntity(model, entity);

            var workflowInstance = await _manager.AssignWorkflowAsync(entity, currentUserId);
            if (workflowInstance != null)
            {
                workflowInstances.Add(workflowInstance);
            }
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        foreach (var workflowInstance in workflowInstances)
        {
            await _manager.PublishInitialWorkflowStepAssignedAsync(workflowInstance);
        }

        return await MapToDtosAsync(inserted);
    }
    /// Mevcut bir hareket talebini güncellemek için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> UpdateAsync(Guid id, UpdateMovementRequestDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return await MapToDtoAsync(saved);
    }

    /// Hareket talebi sevkiyatını gerçekleştirmek için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> DispatchAsync(Guid id, DispatchMovementRequestDto input)
    {
        var dispatched = await _manager.DispatchAsync(
            id,
            input.DispatchNote,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        await InvalidateMovementCacheAsync(dispatched);
        return await MapToDtoAsync(dispatched);
    }

    /// Hareket talebini teslim almak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> ReceiveAsync(Guid id, ReceiveMovementRequestDto input)
    {
        var model = _mapper.MapToModel(input);
        var received = await _manager.ReceiveAsync(
            id,
            model,
            CurrentUser.GetId());

        await InvalidateMovementCacheAsync(received);
        return await MapToDtoAsync(received);
    }

    /// Hareket talebini silmek için kullanılır.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        var request = await _manager.EnsureExistsAsync(id);
        if (request.Status != MovementStatusEnum.Pending)
        {
            throw new BusinessException(MovementRequestExceptionCodes.InvalidStateTransition)
                .WithData("MovementRequestId", id)
                .WithData("CurrentStatus", request.Status)
                .WithData("AllowedStatus", MovementStatusEnum.Pending);
        }

        await _repository.DeleteAsync(id);
    }

    private async Task InvalidateMovementCacheAsync(MovementRequest request)
    {
        var context = await _repository.GetOperationalContextAsync(request.Id);
        var keys = new List<string>();

        if (context != null)
        {
            // Urun stok cache'ini VehicleTaskLine uzerinden temizle.
            if (context.VehicleTaskId != Guid.Empty)
            {
                var lines = await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(context.VehicleTaskId);
                var taskLineIds = lines.Select(l => l.TaskLineId).Distinct().ToList();
                var taskLines = await _taskLineRepository.GetListAsync(x => taskLineIds.Contains(x.Id));
                keys.AddRange(taskLines.Select(l => CacheKeys.ProductStockSummary(l.ProductId)));
            }
            if (context.VehicleId != Guid.Empty)
                keys.Add(CacheKeys.VehicleInventories(context.VehicleId));

            if (context.TaskId != Guid.Empty)
                keys.Add(CacheKeys.TaskInventory(context.TaskId));
        }

        if (keys.Count > 0)
        {
            await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(keys.ToArray()));
        }
    }

    private async Task<MovementRequestDto> MapToDtoAsync(MovementRequest entity)
    {
        var dto = _mapper.MapToDto(entity);
        var context = await _repository.GetOperationalContextAsync(entity.Id);
        if (context != null)
        {
            dto.TaskId = context.TaskId;
        }

        return dto;
    }

    private async Task<List<MovementRequestDto>> MapToDtosAsync(IReadOnlyCollection<MovementRequest> entities)
    {
        if (entities == null || entities.Count == 0)
        {
            return new List<MovementRequestDto>();
        }

        var result = new List<MovementRequestDto>(entities.Count);
        var requestIds = entities.Select(e => e.Id).Distinct().ToList();
        
        // Batch query to solve N+1 problem
        var contextMap = await _repository.GetOperationalContextsAsync(requestIds);

        foreach (var entity in entities)
        {
            var dto = _mapper.MapToDto(entity);
            if (contextMap.TryGetValue(entity.Id, out var context))
            {
                dto.TaskId = context.TaskId;
            }
            result.Add(dto);
        }

        return result;
    }
}


