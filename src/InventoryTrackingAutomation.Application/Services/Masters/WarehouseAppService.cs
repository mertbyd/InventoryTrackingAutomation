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
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Lokasyon application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları WarehouseManager'da.
public class WarehouseAppService : InventoryTrackingAutomationAppService, IWarehouseAppService
{
    // Read/list/persist için ana repository.
    private readonly IWarehouseRepository _repository;
    // Domain manager — Code uniqueness, LinkedVehicle/Worker FK ve WarehouseType enum validasyonu.
    private readonly WarehouseManager _manager;
    private readonly IValidator<CreateWarehouseDto> _createValidator;
    private readonly IValidator<UpdateWarehouseDto> _updateValidator;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public WarehouseAppService(
        IWarehouseRepository repository,
        WarehouseManager manager,
        IValidator<CreateWarehouseDto> createValidator,
        IValidator<UpdateWarehouseDto> updateValidator,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // Id ile lokasyonu getirir; yoksa EntityNotFoundException.
    public async Task<WarehouseDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Warehouse, WarehouseDto>(entity);
    }

    // Lokasyonları sayfalı listeler.
    public async Task<PagedResultDto<WarehouseDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<WarehouseDto>(
            totalCount,
            _mapper.Map<List<Warehouse>, List<WarehouseDto>>(entities));
    }

    // Yeni lokasyon oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateWarehouseDto, CreateWarehouseModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Warehouse, WarehouseDto>(inserted);
    }

    // Birden fazla lokasyonu toplu oluşturur.
    [UnitOfWork]
    public async Task<List<WarehouseDto>> CreateManyAsync(List<CreateWarehouseDto> inputs)
    {
        var entities = new List<Warehouse>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateWarehouseDto, CreateWarehouseModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Warehouse>, List<WarehouseDto>>(inserted);
    }

    // Lokasyonu günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<WarehouseDto> UpdateAsync(Guid id, UpdateWarehouseDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateWarehouseDto, UpdateWarehouseModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Warehouse, WarehouseDto>(saved);
    }

    // Lokasyonu soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
