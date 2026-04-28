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

// Envanter hareketi application servisi - is kurallari InventoryTransactionManager'da kalir.
public class InventoryTransactionAppService : InventoryTrackingAutomationAppService, IInventoryTransactionAppService
{
    private readonly IInventoryTransactionRepository _repository;
    private readonly InventoryTransactionManager _manager;
    private readonly IMapper _mapper;

    public InventoryTransactionAppService(IInventoryTransactionRepository repository, InventoryTransactionManager manager, IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _mapper = mapper;
    }

    public async Task<InventoryTransactionDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(entity);
    }

    public async Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTransactionDto>(totalCount, _mapper.Map<List<InventoryTransaction>, List<InventoryTransactionDto>>(entities));
    }

    [UnitOfWork]
    public async Task<InventoryTransactionDto> CreateAsync(CreateInventoryTransactionDto input)
    {
        var model = _mapper.Map<CreateInventoryTransactionDto, CreateInventoryTransactionModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(inserted);
    }

    [UnitOfWork]
    public async Task<List<InventoryTransactionDto>> CreateManyAsync(List<CreateInventoryTransactionDto> inputs)
    {
        var entities = new List<InventoryTransaction>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateInventoryTransactionDto, CreateInventoryTransactionModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<InventoryTransaction>, List<InventoryTransactionDto>>(inserted);
    }

    [UnitOfWork]
    public async Task<InventoryTransactionDto> UpdateAsync(Guid id, UpdateInventoryTransactionDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateInventoryTransactionDto, UpdateInventoryTransactionModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(saved);
    }

    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
