using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

// Envanter gorevi application servisi - is kurallari InventoryTaskManager'da kalir.
public class InventoryTaskAppService : InventoryTrackingAutomationAppService, IInventoryTaskAppService
{
    private readonly IInventoryTaskRepository _repository;
    private readonly InventoryTaskManager _manager;
    private readonly InventoryQueryManager _inventoryQueryManager;
    private readonly IValidator<CreateInventoryTaskDto> _createValidator;
    private readonly IValidator<UpdateInventoryTaskDto> _updateValidator;
    private readonly IMapper _mapper;

    public InventoryTaskAppService(
        IInventoryTaskRepository repository,
        InventoryTaskManager manager,
        InventoryQueryManager inventoryQueryManager,
        IValidator<CreateInventoryTaskDto> createValidator,
        IValidator<UpdateInventoryTaskDto> updateValidator,
        IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _inventoryQueryManager = inventoryQueryManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

    public async Task<InventoryTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(entity);
    }

    public async Task<PagedResultDto<InventoryTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTaskDto>(totalCount, _mapper.Map<List<InventoryTask>, List<InventoryTaskDto>>(entities));
    }

    public async Task<List<TaskVehicleDto>> GetVehiclesAsync(Guid id)
    {
        var vehicles = await _inventoryQueryManager.GetTaskVehiclesAsync(id);
        return _mapper.Map<List<TaskVehicleModel>, List<TaskVehicleDto>>(vehicles);
    }

    public async Task<List<TaskInventoryDto>> GetInventoryAsync(Guid id)
    {
        var inventory = await _inventoryQueryManager.GetTaskInventoryAsync(id);
        return _mapper.Map<List<TaskInventoryModel>, List<TaskInventoryDto>>(inventory);
    }

    [UnitOfWork]
    public async Task<InventoryTaskDto> CreateAsync(CreateInventoryTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(inserted);
    }

    [UnitOfWork]
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
    public async Task<InventoryTaskDto> UpdateAsync(Guid id, UpdateInventoryTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateInventoryTaskDto, UpdateInventoryTaskModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<InventoryTask, InventoryTaskDto>(saved);
    }

    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
