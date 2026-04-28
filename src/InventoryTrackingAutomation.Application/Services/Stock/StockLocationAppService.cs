using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Managers.Stock;
using InventoryTrackingAutomation.Models.Stock;
using InventoryTrackingAutomation.Services.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Lokasyon bazli stok application servisi - is kurallari StockLocationManager'da kalir.
public class StockLocationAppService : InventoryTrackingAutomationAppService, IStockLocationAppService
{
    private readonly IStockLocationRepository _repository;
    private readonly StockLocationManager _manager;
    private readonly IMapper _mapper;

    public StockLocationAppService(IStockLocationRepository repository, StockLocationManager manager, IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
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
            var model = _mapper.Map<CreateStockLocationDto, CreateStockLocationModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<StockLocation>, List<StockLocationDto>>(inserted);
    }

    [UnitOfWork]
    public async Task<StockLocationDto> UpdateAsync(Guid id, UpdateStockLocationDto input)
    {
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
