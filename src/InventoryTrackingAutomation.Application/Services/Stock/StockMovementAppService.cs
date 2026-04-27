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

// Stok hareketi application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları StockMovementManager'da.
public class StockMovementAppService : InventoryTrackingAutomationAppService, IStockMovementAppService
{
    // Read/list/persist için ana repository.
    private readonly IStockMovementRepository _repository;
    // Domain manager — Product/Site FK ve MovementType enum validasyonu.
    private readonly StockMovementManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public StockMovementAppService(
        IStockMovementRepository repository,
        StockMovementManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile stok hareketini getirir; yoksa EntityNotFoundException.
    public async Task<StockMovementDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<StockMovement, StockMovementDto>(entity);
    }

    // Stok hareketlerini sayfalı listeler.
    public async Task<PagedResultDto<StockMovementDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<StockMovementDto>(
            totalCount,
            _mapper.Map<List<StockMovement>, List<StockMovementDto>>(entities));
    }

    // Yeni stok hareketi oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<StockMovementDto> CreateAsync(CreateStockMovementDto input)
    {
        var model = _mapper.Map<CreateStockMovementDto, CreateStockMovementModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<StockMovement, StockMovementDto>(inserted);
    }

    // Birden fazla stok hareketini toplu oluşturur.
    [UnitOfWork]
    public async Task<List<StockMovementDto>> CreateManyAsync(List<CreateStockMovementDto> inputs)
    {
        var entities = new List<StockMovement>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateStockMovementDto, CreateStockMovementModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<StockMovement>, List<StockMovementDto>>(inserted);
    }

    // Stok hareketini günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<StockMovementDto> UpdateAsync(Guid id, UpdateStockMovementDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateStockMovementDto, UpdateStockMovementModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<StockMovement, StockMovementDto>(saved);
    }

    // Stok hareketini soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
