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
/// Ürün stok application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class ProductStockAppService : InventoryTrackingAutomationAppService, IProductStockAppService
{
    private readonly IProductStockRepository _repository;
    private readonly ProductStockManager _manager;

    public ProductStockAppService(
        IProductStockRepository repository,
        ProductStockManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre stok kaydı getirir. </summary>
    public async Task<ProductStockDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductStocks.NotFound);
        return ObjectMapper.Map<ProductStock, ProductStockDto>(entity);
    }

    /// <summary> Stok kayıtlarını sayfalı listeler. </summary>
    public async Task<PagedResultDto<ProductStockDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductStockDto>(
            totalCount, ObjectMapper.Map<List<ProductStock>, List<ProductStockDto>>(entities));
    }

    /// <summary> Yeni stok kaydı oluşturur. </summary>
    [UnitOfWork]
    public async Task<ProductStockDto> CreateAsync(CreateProductStockDto input)
    {
        var model = ObjectMapper.Map<CreateProductStockDto, CreateProductStockModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<ProductStock, ProductStockDto>(inserted);
    }

    /// <summary> Birden fazla stok kaydını toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<ProductStockDto>> CreateManyAsync(List<CreateProductStockDto> inputs)
    {
        var entities = new List<ProductStock>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateProductStockDto, CreateProductStockModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<ProductStock>, List<ProductStockDto>>(inserted);
    }

    /// <summary> Stok kaydını günceller. </summary>
    [UnitOfWork]
    public async Task<ProductStockDto> UpdateAsync(Guid id, UpdateProductStockDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductStocks.NotFound);
        var model = ObjectMapper.Map<UpdateProductStockDto, UpdateProductStockModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<ProductStock, ProductStockDto>(saved);
    }

    /// <summary> Stok kaydını soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductStocks.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
