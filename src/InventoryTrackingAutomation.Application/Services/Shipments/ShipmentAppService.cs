using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Shipments;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Managers.Shipments;
using InventoryTrackingAutomation.Models.Shipments;
using InventoryTrackingAutomation.Services.Shipments;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Shipments;

// Sevkiyat application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ShipmentManager'da.
public class ShipmentAppService : InventoryTrackingAutomationAppService, IShipmentAppService
{
    // Read/list/persist için ana repository.
    private readonly IShipmentRepository _repository;
    // Domain manager — ShipmentNumber uniqueness, Vehicle/Driver FK ve Status enum validasyonu.
    private readonly ShipmentManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public ShipmentAppService(
        IShipmentRepository repository,
        ShipmentManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile sevkiyatı getirir; yoksa EntityNotFoundException.
    public async Task<ShipmentDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Shipment, ShipmentDto>(entity);
    }

    // Sevkiyatları sayfalı listeler.
    public async Task<PagedResultDto<ShipmentDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ShipmentDto>(
            totalCount,
            _mapper.Map<List<Shipment>, List<ShipmentDto>>(entities));
    }

    // Yeni sevkiyat oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto input)
    {
        var model = _mapper.Map<CreateShipmentDto, CreateShipmentModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Shipment, ShipmentDto>(inserted);
    }

    // Birden fazla sevkiyatı toplu oluşturur.
    [UnitOfWork]
    public async Task<List<ShipmentDto>> CreateManyAsync(List<CreateShipmentDto> inputs)
    {
        var entities = new List<Shipment>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateShipmentDto, CreateShipmentModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Shipment>, List<ShipmentDto>>(inserted);
    }

    // Sevkiyatı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ShipmentDto> UpdateAsync(Guid id, UpdateShipmentDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateShipmentDto, UpdateShipmentModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Shipment, ShipmentDto>(saved);
    }

    // Sevkiyatı soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
