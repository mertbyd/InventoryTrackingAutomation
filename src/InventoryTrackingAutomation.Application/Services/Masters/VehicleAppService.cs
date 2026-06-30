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

    // Tüm bağımlılıkları DI ile alır.
    private static readonly VehicleMapper _mapper = new VehicleMapper();


    // Id ile aracı getirir; yoksa EntityNotFoundException.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
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
            _mapper.MapToDto(entities));
    }

    // Arac uzerindeki envanterleri getirir; cache okuma/yazma InventoryCacheInterceptor tarafindan yapilir.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    // CacheKeys.VehicleInventoriesTemplate invalidation tarafindaki VehicleInventories key'i ile ayni sozlesmeyi kullanir.
    [InventoryCache(CacheKeys.VehicleInventoriesTemplate, 10)]
    public async Task<List<VehicleInventoryDto>> GetInventoriesAsync(Guid id)
    {
        var inventories = await _inventoryQueryManager.GetVehicleInventoriesAsync(id);
        return _mapper.MapToDto(inventories);
    }

    // Yeni araç oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new Vehicle(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    // Birden fazla aracı toplu oluşturur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<VehicleDto>> CreateManyAsync(List<CreateVehicleDto> inputs)
    {
        var models = new List<CreateVehicleModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<Vehicle>();
        foreach (var model in validatedModels)
        {
            var entity = new Vehicle(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    // Aracı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    // Araci silmek yerine pasife alir; arac-gorev ve stok gecmisi korunur.
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
