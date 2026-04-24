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

/// <summary>
/// Stok hareketi application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class StockMovementAppService : InventoryTrackingAutomationAppService, IStockMovementAppService
{
    private readonly IStockMovementRepository _repository;
    private readonly StockMovementManager _manager;

    public StockMovementAppService(
        IStockMovementRepository repository,
        StockMovementManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre stok hareketi getirir. </summary>
    public async Task<StockMovementDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.StockMovements.NotFound);
        return ObjectMapper.Map<StockMovement, StockMovementDto>(entity);
    }

    /// <summary> Stok hareketlerini sayfalı listeler. </summary>
    public async Task<PagedResultDto<StockMovementDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<StockMovementDto>(
            totalCount, ObjectMapper.Map<List<StockMovement>, List<StockMovementDto>>(entities));
    }

    /// <summary> Yeni stok hareketi oluşturur. </summary>
    [UnitOfWork]
    public async Task<StockMovementDto> CreateAsync(CreateStockMovementDto input)
    {
        var model = ObjectMapper.Map<CreateStockMovementDto, CreateStockMovementModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<StockMovement, StockMovementDto>(inserted);
    }

    /// <summary> Birden fazla stok hareketini toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<StockMovementDto>> CreateManyAsync(List<CreateStockMovementDto> inputs)
    {
        var entities = new List<StockMovement>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateStockMovementDto, CreateStockMovementModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<StockMovement>, List<StockMovementDto>>(inserted);
    }

    /// <summary> Stok hareketini günceller. </summary>
    [UnitOfWork]
    public async Task<StockMovementDto> UpdateAsync(Guid id, UpdateStockMovementDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.StockMovements.NotFound);
        var model = ObjectMapper.Map<UpdateStockMovementDto, UpdateStockMovementModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<StockMovement, StockMovementDto>(saved);
    }

    /// <summary> Stok hareketini soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.StockMovements.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
