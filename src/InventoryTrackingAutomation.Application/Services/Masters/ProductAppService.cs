using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Services.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Masters;

/// <summary>
/// Ürün application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class ProductAppService : InventoryTrackingAutomationAppService, IProductAppService
{
    private readonly IProductRepository _repository;
    private readonly ProductManager _manager;

    public ProductAppService(
        IProductRepository repository,
        ProductManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre ürün getirir. </summary>
    public async Task<ProductDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);
        return ObjectMapper.Map<Product, ProductDto>(entity);
    }

    /// <summary> Ürünleri sayfalı listeler. </summary>
    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductDto>(
            totalCount, ObjectMapper.Map<List<Product>, List<ProductDto>>(entities));
    }

    /// <summary> Yeni ürün oluşturur. </summary>
    [UnitOfWork]
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var model = ObjectMapper.Map<CreateProductDto, CreateProductModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Product, ProductDto>(inserted);
    }

    /// <summary> Birden fazla ürünü toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<ProductDto>> CreateManyAsync(List<CreateProductDto> inputs)
    {
        var entities = new List<Product>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateProductDto, CreateProductModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Product>, List<ProductDto>>(inserted);
    }

    /// <summary> Ürünü günceller. </summary>
    [UnitOfWork]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);
        var model = ObjectMapper.Map<UpdateProductDto, UpdateProductModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Product, ProductDto>(saved);
    }

    /// <summary> Ürünü soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Products.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
