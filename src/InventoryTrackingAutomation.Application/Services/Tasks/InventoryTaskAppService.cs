using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
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

namespace InventoryTrackingAutomation.Application.Services.Tasks;

// Envanter gorevi application servisi - is kurallari InventoryTaskManager'da kalir.
//işlevi: InventoryTask iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class InventoryTaskAppService : InventoryTrackingAutomationAppService, IInventoryTaskAppService
{
    private readonly IInventoryTaskRepository _repository;
    private readonly InventoryTaskManager _manager;
    private readonly InventoryQueryManager _inventoryQueryManager;
    private readonly ILocalEventBus _localEventBus;
    private readonly IValidator<CreateInventoryTaskDto> _createValidator;
    private readonly IValidator<UpdateInventoryTaskDto> _updateValidator;
    private readonly IMapper _mapper;

    public InventoryTaskAppService(
        IInventoryTaskRepository repository,
        InventoryTaskManager manager,
        InventoryQueryManager inventoryQueryManager,
        ILocalEventBus localEventBus,
        IValidator<CreateInventoryTaskDto> createValidator,
        IValidator<UpdateInventoryTaskDto> updateValidator,
        IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _inventoryQueryManager = inventoryQueryManager;
        _localEventBus = localEventBus;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(entity);
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<InventoryTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTaskDto>(totalCount, _mapper.Map<List<InventoryTask>, List<InventoryTaskDto>>(entities));
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<TaskVehicleDto>> GetVehiclesAsync(Guid id)
    {
        var vehicles = await _inventoryQueryManager.GetTaskVehiclesAsync(id);
        return _mapper.Map<List<TaskVehicleModel>, List<TaskVehicleDto>>(vehicles);
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<TaskInventoryDto>> GetInventoryAsync(Guid id)
    {
        var inventory = await _inventoryQueryManager.GetTaskInventoryAsync(id);
        return _mapper.Map<List<TaskInventoryModel>, List<TaskInventoryDto>>(inventory);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTaskDto> CreateAsync(CreateInventoryTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<InventoryTaskDto>> CreateManyAsync(List<CreateInventoryTaskDto> inputs)
    {
        var entities = new List<InventoryTask>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<InventoryTask>, List<InventoryTaskDto>>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTaskDto> UpdateAsync(Guid id, UpdateInventoryTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var targetStatus = input.Status;
        var model = _mapper.Map<UpdateInventoryTaskDto, UpdateInventoryTaskModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);

        if (updated.Status != targetStatus)
        {
            await _manager.TransitionStatusAsync(
                updated,
                targetStatus,
                _localEventBus,
                CurrentUser.GetId(),
                await ResolveCurrentWorkerIdAsync());
        }

        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(saved);
    }

    [UnitOfWork]
    public async Task<InventoryTaskDto> CompleteAsync(Guid id)
    {
        var task = await _manager.EnsureExistsAsync(id);
        await _manager.TransitionStatusAsync(
            task,
            TaskStatusEnum.Completed,
            _localEventBus,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        var saved = await _repository.UpdateAsync(task, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(saved);
    }

    [UnitOfWork]
    public async Task<InventoryTaskDto> CancelAsync(Guid id)
    {
        var task = await _manager.EnsureExistsAsync(id);
        await _manager.TransitionStatusAsync(
            task,
            TaskStatusEnum.Cancelled,
            _localEventBus,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        var saved = await _repository.UpdateAsync(task, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(saved);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }

}
