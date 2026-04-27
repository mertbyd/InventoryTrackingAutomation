using AutoMapper;
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

// Ürün application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ProductManager'da.
public class ProductAppService : InventoryTrackingAutomationAppService, IProductAppService
{
    // Read/list/persist için ana repository.
    private readonly IProductRepository _repository;
    // Domain manager — Code uniqueness, CategoryId FK ve BaseUnit enum validasyonu.
    private readonly ProductManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public ProductAppService(
        IProductRepository repository,
        ProductManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile ürünü getirir; yoksa EntityNotFoundException.
    public async Task<ProductDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Product, ProductDto>(entity);
    }

    // Ürünleri sayfalı listeler.
    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductDto>(
            totalCount,
            _mapper.Map<List<Product>, List<ProductDto>>(entities));
    }

    // Yeni ürün oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        var model = _mapper.Map<CreateProductDto, CreateProductModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Product, ProductDto>(inserted);
    }

    // Birden fazla ürünü toplu oluşturur.
    [UnitOfWork]
    public async Task<List<ProductDto>> CreateManyAsync(List<CreateProductDto> inputs)
    {
        var entities = new List<Product>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateProductDto, CreateProductModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Product>, List<ProductDto>>(inserted);
    }

    // Ürünü günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateProductDto, UpdateProductModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Product, ProductDto>(saved);
    }

    // Ürünü soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
