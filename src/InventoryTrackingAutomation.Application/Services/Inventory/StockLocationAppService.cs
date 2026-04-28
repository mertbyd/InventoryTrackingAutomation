using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Services.Inventory;
using FluentValidation;
using InventoryTrackingAutomation.Managers.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Lokasyon bazli stok application servisi - is kurallari StockLocationManager'da kalir.
public class StockLocationAppService : InventoryTrackingAutomationAppService, IStockLocationAppService
{
    private readonly IStockLocationRepository _repository;
    private readonly StockLocationManager _manager;
    private readonly IValidator<CreateStockLocationDto> _createValidator;
    private readonly IValidator<UpdateStockLocationDto> _updateValidator;
    private readonly IMapper _mapper;

    public StockLocationAppService(
        IStockLocationRepository repository,
        StockLocationManager manager,
        IValidator<CreateStockLocationDto> createValidator,
        IValidator<UpdateStockLocationDto> updateValidator,
        IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

    public async Task<StockLocationDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<StockLocation, StockLocationDto>(entity);
    }

    public async Task<PagedResultDto<StockLocationDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<StockLocationDto>(totalCount, _mapper.Map<List<StockLocation>, List<StockLocationDto>>(entities));
    }

    [UnitOfWork]
    public async Task<StockLocationDto> CreateAsync(CreateStockLocationDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateStockLocationDto, CreateStockLocationModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<StockLocation, StockLocationDto>(inserted);
    }

    [UnitOfWork]
    public async Task<List<StockLocationDto>> CreateManyAsync(List<CreateStockLocationDto> inputs)
    {
        var entities = new List<StockLocation>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateStockLocationDto, CreateStockLocationModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<StockLocation>, List<StockLocationDto>>(inserted);
    }

    [UnitOfWork]
    public async Task<StockLocationDto> UpdateAsync(Guid id, UpdateStockLocationDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateStockLocationDto, UpdateStockLocationModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<StockLocation, StockLocationDto>(saved);
    }

    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
