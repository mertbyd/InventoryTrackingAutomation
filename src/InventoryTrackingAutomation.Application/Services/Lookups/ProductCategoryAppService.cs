using AutoMapper;
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

// Ürün kategorisi application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ProductCategoryManager'da.
public class ProductCategoryAppService : InventoryTrackingAutomationAppService, IProductCategoryAppService
{
    // Read/list/persist için ana repository.
    private readonly IProductCategoryRepository _repository;
    // Domain manager — Code uniqueness ve ParentId varlık kontrolü.
    private readonly ProductCategoryManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public ProductCategoryAppService(
        IProductCategoryRepository repository,
        ProductCategoryManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile ürün kategorisini getirir; yoksa EntityNotFoundException.
    public async Task<ProductCategoryDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<ProductCategory, ProductCategoryDto>(entity);
    }

    // Ürün kategorilerini sayfalı listeler.
    public async Task<PagedResultDto<ProductCategoryDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductCategoryDto>(
            totalCount,
            _mapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(entities));
    }

    // Yeni ürün kategorisi oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductCategoryDto> CreateAsync(CreateProductCategoryDto input)
    {
        var model = _mapper.Map<CreateProductCategoryDto, CreateProductCategoryModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<ProductCategory, ProductCategoryDto>(inserted);
    }

    // Birden fazla ürün kategorisini toplu oluşturur.
    [UnitOfWork]
    public async Task<List<ProductCategoryDto>> CreateManyAsync(List<CreateProductCategoryDto> inputs)
    {
        var entities = new List<ProductCategory>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateProductCategoryDto, CreateProductCategoryModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<ProductCategory>, List<ProductCategoryDto>>(inserted);
    }

    // Ürün kategorisini günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductCategoryDto> UpdateAsync(Guid id, UpdateProductCategoryDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateProductCategoryDto, UpdateProductCategoryModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<ProductCategory, ProductCategoryDto>(saved);
    }

    // Ürün kategorisini soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
