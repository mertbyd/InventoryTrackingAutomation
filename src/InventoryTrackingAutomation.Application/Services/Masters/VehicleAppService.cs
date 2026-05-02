using AutoMapper;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
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
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Araç application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları VehicleManager'da.
//işlevi: Vehicle iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class VehicleAppService : InventoryTrackingAutomationAppService, IVehicleAppService
{
    public VehicleAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list/persist için ana repository.
    private IVehicleRepository _repository => LazyGetRequiredService<IVehicleRepository>();
    // Domain manager — PlateNumber uniqueness ve VehicleType enum validasyonu.
    private VehicleManager _manager => LazyGetRequiredService<VehicleManager>();
    // PITON arac stok gorunurlugu okuma kurallari.
    private InventoryQueryManager _inventoryQueryManager => LazyGetRequiredService<InventoryQueryManager>();
    private IValidator<CreateVehicleDto> _createValidator => LazyGetRequiredService<IValidator<CreateVehicleDto>>();
    private IValidator<UpdateVehicleDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateVehicleDto>>();
    private IDistributedCache _cache => LazyGetRequiredService<IDistributedCache>();

    // Tüm bağımlılıkları DI ile alır.
    private IMapper _mapper => LazyGetRequiredService<IMapper>();


    // Id ile aracı getirir; yoksa EntityNotFoundException.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Vehicle, VehicleDto>(entity);
    }

    // Araçları sayfalı listeler.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<VehicleDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleDto>(
            totalCount,
            _mapper.Map<List<Vehicle>, List<VehicleDto>>(entities));
    }

    // Arac uzerindeki envanterleri getirir; cache-aside ile Redis'ten okur, yoksa DB'den ceker.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<VehicleInventoryDto>> GetInventoriesAsync(Guid id)
    {
        var cacheKey = CacheKeys.VehicleInventories(id);
        var cached = await _cache.GetStringAsync(cacheKey);
        if (cached is not null)
            return JsonSerializer.Deserialize<List<VehicleInventoryDto>>(cached)!;

        var inventories = await _inventoryQueryManager.GetVehicleInventoriesAsync(id);
        var dto = _mapper.Map<List<VehicleInventoryModel>, List<VehicleInventoryDto>>(inventories);

        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(dto),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10) });

        return dto;
    }

    // Yeni araç oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateVehicleDto, CreateVehicleModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Vehicle, VehicleDto>(inserted);
    }

    // Birden fazla aracı toplu oluşturur.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<VehicleDto>> CreateManyAsync(List<CreateVehicleDto> inputs)
    {
        var entities = new List<Vehicle>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateVehicleDto, CreateVehicleModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Vehicle>, List<VehicleDto>>(inserted);
    }

    // Aracı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateVehicleDto, UpdateVehicleModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Vehicle, VehicleDto>(saved);
    }

    // Aracı soft delete ile siler.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
