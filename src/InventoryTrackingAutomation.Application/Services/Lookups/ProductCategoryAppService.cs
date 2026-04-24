using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

/// <summary>
/// Ürün kategorisi application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class ProductCategoryAppService : InventoryTrackingAutomationAppService, IProductCategoryAppService
{
    private readonly IProductCategoryRepository _repository;
    private readonly ProductCategoryManager _manager;

    public ProductCategoryAppService(
        IProductCategoryRepository repository,
        ProductCategoryManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre ürün kategorisi getirir. </summary>
    public async Task<ProductCategoryDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(entity);
    }

    /// <summary> Ürün kategorilerini sayfalı listeler. </summary>
    public async Task<PagedResultDto<ProductCategoryDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductCategoryDto>(
            totalCount, ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(entities));
    }

    /// <summary> Yeni ürün kategorisi oluşturur. </summary>
    [UnitOfWork]
    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input)
    {
        var model = ObjectMapper.Map<CreateProductCategoryDto, CreateProductCategoryModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(inserted);
    }

    /// <summary> Birden fazla ürün kategorisini toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<ProductCategoryDto>> CreateManyAsync(List<CreateProductCategoryDto> inputs)
    {
        var entities = new List<ProductCategory>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateProductCategoryDto, CreateProductCategoryModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(inserted);
    }

    /// <summary> Ürün kategorisini günceller. </summary>
    [UnitOfWork]
    public async Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        var model = ObjectMapper.Map<UpdateProductCategoryDto, UpdateProductCategoryModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<ProductCategory, ProductCategoryDto>(saved);
    }

    /// <summary> Ürün kategorisini soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
