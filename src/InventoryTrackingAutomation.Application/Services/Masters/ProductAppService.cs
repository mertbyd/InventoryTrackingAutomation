using InventoryTrackingAutomation.Application.Mappers.Masters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Caching;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Services.Masters;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Ürün application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ProductManager'da.
//işlevi: Product iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class ProductAppService : InventoryTrackingAutomationAppService, IProductAppService
{
    public ProductAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list/persist için ana repository.
    private IProductRepository _repository => LazyGetRequiredService<IProductRepository>();
    // Domain manager — Code uniqueness, CategoryId FK ve BaseUnit enum validasyonu.
    private ProductManager _manager => LazyGetRequiredService<ProductManager>();
    // PITON stok gorunurlugu okuma kurallari.
    private InventoryQueryManager _inventoryQueryManager => LazyGetRequiredService<InventoryQueryManager>();
    private IValidator<CreateProductDto> _createValidator => LazyGetRequiredService<IValidator<CreateProductDto>>();
    private IValidator<UpdateProductDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateProductDto>>();

    // Tüm bağımlılıkları DI ile alır.
    private static readonly ProductMapper _mapper = new ProductMapper();


    // Id ile ürünü getirir; yoksa EntityNotFoundException.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<ProductDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    // Ürünleri sayfalı listeler.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<ProductDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductDto>(
            totalCount,
            _mapper.MapToDto(entities));
    }

    // Urunun lokasyon bazli stok ozetini getirir; cache okuma/yazma InventoryCacheInterceptor tarafindan yapilir.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    // CacheKeys.ProductStockSummaryTemplate invalidation tarafindaki ProductStockSummary key'i ile ayni sozlesmeyi kullanir.
    [InventoryCache(CacheKeys.ProductStockSummaryTemplate, 10)]
    public async Task<ProductStockSummaryDto> GetStockSummaryAsync(Guid id)
    {
        var summary = await _inventoryQueryManager.GetProductStockSummaryAsync(id);
        return _mapper.MapToDto(summary);
    }

    // Yeni ürün oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<ProductDto> CreateAsync(CreateProductDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new Product(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    // Birden fazla ürünü toplu oluşturur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<ProductDto>> CreateManyAsync(List<CreateProductDto> inputs)
    {
        var entities = new List<Product>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.MapToModel(dto);
            var validatedModel = await _manager.CreateAsync(model);
            var entity = new Product(GuidGenerator.Create());
            _mapper.MapToEntity(validatedModel, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    // Ürünü günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    // Urunu silmek yerine pasife alir; master veri gecmisi ve FK butunlugu korunur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var passivated = await _manager.PassivateAsync(existing);
        await _repository.UpdateAsync(passivated, autoSave: true);
    }
}
