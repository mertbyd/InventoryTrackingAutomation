using InventoryTrackingAutomation.Application.Mappers.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Services.Inventory;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Lokasyon bazli stok application servisi - is kurallari StockLocationManager'da kalir.
//işlevi: StockLocation iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class StockLocationAppService : InventoryTrackingAutomationAppService, IStockLocationAppService
{
    public StockLocationAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IStockLocationRepository _repository => LazyGetRequiredService<IStockLocationRepository>();
    private StockLocationManager _manager => LazyGetRequiredService<StockLocationManager>();
    // Stok lokasyonu degisiklikleri urun/arac stok cachelerini local event ile temizler.
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    private IValidator<CreateStockLocationDto> _createValidator => LazyGetRequiredService<IValidator<CreateStockLocationDto>>();
    private IValidator<UpdateStockLocationDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateStockLocationDto>>();
    private static readonly StockLocationMapper _mapper = new StockLocationMapper();



    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<StockLocationDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<StockLocationDto>(totalCount, _mapper.MapToDto(entities));
    }

    public async Task<List<InventoryGridItemDto>> GetInventoryGridListAsync()
    {
        var locations = await _repository.WithDetailsAsync(x => x.Product, x => x.Warehouse);
        var filteredLocations = locations
            .Where(x => x.LocationType == StockLocationTypeEnum.Warehouse)
            .ToList();

        return filteredLocations.Select(l => new InventoryGridItemDto
        {
            Id = l.Id,
            ProductId = l.ProductId,
            Name = l.Product.Name,
            CategoryId = l.Product.CategoryId ?? Guid.Empty,
            CategoryName = "MockCategory", // To be implemented with ProductCategory Include
            WarehouseId = l.Warehouse != null ? l.Warehouse.Id : Guid.Empty,
            WarehouseName = l.Warehouse != null ? l.Warehouse.Name : null,
            WarehouseLocation = l.Warehouse != null ? l.Warehouse.Code : null,
            Quantity = l.Quantity
        }).ToList();
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> CreateAsync(CreateStockLocationDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var entity = await _manager.CreateAsync(model);
        _mapper.MapToEntity(model, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        await InvalidateStockCacheAsync(inserted.ProductId, inserted.LocationType, inserted.LocationId);
        return _mapper.MapToDto(inserted);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<StockLocationDto>> CreateManyAsync(List<CreateStockLocationDto> inputs)
    {
        var models = new List<CreateStockLocationModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<StockLocation>();
        foreach (var model in validatedModels)
        {
            var entity = new StockLocation(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        await InvalidateStockCacheAsync(inserted);

        return _mapper.MapToDto(inserted);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> UpdateAsync(Guid id, UpdateStockLocationDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var updated = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(model, updated);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        await InvalidateStockCacheAsync(saved.ProductId, saved.LocationType, saved.LocationId);
        return _mapper.MapToDto(saved);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        // StockLocation silinmez; miktar degisiklikleri stok hareketi ve update akislariyla yonetilir.
        await _manager.RejectDeleteAsync(id);
    }

    private async Task InvalidateStockCacheAsync(Guid productId, StockLocationTypeEnum locationType, Guid locationId)
    {
        await PublishStockCacheInvalidationAsync(BuildStockCacheKeys(productId, locationType, locationId));
    }

    private Task InvalidateStockCacheAsync(IEnumerable<StockLocation> stockLocations)
    {
        var keys = stockLocations
            .SelectMany(x => BuildStockCacheKeys(x.ProductId, x.LocationType, x.LocationId))
            .Distinct()
            .ToArray();

        return PublishStockCacheInvalidationAsync(keys);
    }

    private static IEnumerable<string> BuildStockCacheKeys(
        Guid productId,
        StockLocationTypeEnum locationType,
        Guid locationId)
    {
        yield return CacheKeys.ProductStockSummary(productId);

        if (locationType == StockLocationTypeEnum.Vehicle)
        {
            yield return CacheKeys.VehicleInventories(locationId);
        }
    }

    private Task PublishStockCacheInvalidationAsync(IEnumerable<string> keys)
    {
        var keyArray = keys.Distinct().ToArray();
        return keyArray.Length == 0
            ? Task.CompletedTask
            : _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(keyArray));
    }
}


