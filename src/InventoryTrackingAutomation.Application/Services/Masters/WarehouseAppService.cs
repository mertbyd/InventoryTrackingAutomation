using InventoryTrackingAutomation.Application.Mappers.Masters;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Services.Masters;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Lokasyon application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları WarehouseManager'da.
//işlevi: Warehouse iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class WarehouseAppService : InventoryTrackingAutomationAppService, IWarehouseAppService
{
    public WarehouseAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list/persist için ana repository.
    private IWarehouseRepository _repository => LazyGetRequiredService<IWarehouseRepository>();
    // Domain manager — Code uniqueness, LinkedVehicle/Worker FK ve WarehouseType enum validasyonu.
    private WarehouseManager _manager => LazyGetRequiredService<WarehouseManager>();
    private IValidator<CreateWarehouseDto> _createValidator => LazyGetRequiredService<IValidator<CreateWarehouseDto>>();
    private IValidator<UpdateWarehouseDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateWarehouseDto>>();

    // Tüm bağımlılıkları DI ile alır.
    private static readonly WarehouseMapper _mapper = new WarehouseMapper();


    // Id ile lokasyonu getirir; yoksa EntityNotFoundException.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WarehouseDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    // Lokasyonları sayfalı listeler.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<WarehouseDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<WarehouseDto>(
            totalCount,
            _mapper.MapToDto(entities));
    }

    // Yeni lokasyon oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new Warehouse(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    // Birden fazla lokasyonu toplu oluşturur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<WarehouseDto>> CreateManyAsync(List<CreateWarehouseDto> inputs)
    {
        var models = new List<CreateWarehouseModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<Warehouse>();
        foreach (var model in validatedModels)
        {
            var entity = new Warehouse(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    // Lokasyonu günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    // Depoyu silmek yerine pasife alir; stok, gorev ve hareket gecmisi korunur.
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
