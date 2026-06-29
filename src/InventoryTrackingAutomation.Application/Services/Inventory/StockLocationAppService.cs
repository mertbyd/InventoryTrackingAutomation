using AutoMapper;
using System;
using System.Collections.Generic;
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
    private IMapper _mapper => LazyGetRequiredService<IMapper>();



//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<StockLocation, StockLocationDto>(entity);
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<StockLocationDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<StockLocationDto>(totalCount, _mapper.Map<List<StockLocation>, List<StockLocationDto>>(entities));
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> CreateAsync(CreateStockLocationDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateStockLocationDto, CreateStockLocationModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        await InvalidateStockCacheAsync(inserted.ProductId, inserted.LocationType, inserted.LocationId);
        return _mapper.Map<StockLocation, StockLocationDto>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<StockLocationDto>> CreateManyAsync(List<CreateStockLocationDto> inputs)
    {
        var entities = new List<StockLocation>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateStockLocationDto, CreateStockLocationModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        foreach (var e in inserted)
            await InvalidateStockCacheAsync(e.ProductId, e.LocationType, e.LocationId);
        return _mapper.Map<List<StockLocation>, List<StockLocationDto>>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<StockLocationDto> UpdateAsync(Guid id, UpdateStockLocationDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateStockLocationDto, UpdateStockLocationModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        await InvalidateStockCacheAsync(saved.ProductId, saved.LocationType, saved.LocationId);
        return _mapper.Map<StockLocation, StockLocationDto>(saved);
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
        var keys = locationType == StockLocationTypeEnum.Vehicle
            ? new[] { CacheKeys.ProductStockSummary(productId), CacheKeys.VehicleInventories(locationId) }
            : new[] { CacheKeys.ProductStockSummary(productId) };

        await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(keys));
    }
}
