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

// Sevkiyat satırı application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ShipmentLineManager'da.
public class ShipmentLineAppService : InventoryTrackingAutomationAppService, IShipmentLineAppService
{
    // Read/list/persist için ana repository.
    private readonly IShipmentLineRepository _repository;
    // Domain manager — Shipment/MovementRequestLine/Product FK kontrolleri.
    private readonly ShipmentLineManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public ShipmentLineAppService(
        IShipmentLineRepository repository,
        ShipmentLineManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile sevkiyat satırını getirir; yoksa EntityNotFoundException.
    public async Task<ShipmentLineDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<ShipmentLine, ShipmentLineDto>(entity);
    }

    // Sevkiyat satırlarını sayfalı listeler.
    public async Task<PagedResultDto<ShipmentLineDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ShipmentLineDto>(
            totalCount,
            _mapper.Map<List<ShipmentLine>, List<ShipmentLineDto>>(entities));
    }

    // Yeni sevkiyat satırı oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ShipmentLineDto> CreateAsync(CreateShipmentLineDto input)
    {
        var model = _mapper.Map<CreateShipmentLineDto, CreateShipmentLineModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<ShipmentLine, ShipmentLineDto>(inserted);
    }

    // Birden fazla sevkiyat satırını toplu oluşturur.
    [UnitOfWork]
    public async Task<List<ShipmentLineDto>> CreateManyAsync(List<CreateShipmentLineDto> inputs)
    {
        var entities = new List<ShipmentLine>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateShipmentLineDto, CreateShipmentLineModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<ShipmentLine>, List<ShipmentLineDto>>(inserted);
    }

    // Sevkiyat satırını günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ShipmentLineDto> UpdateAsync(Guid id, UpdateShipmentLineDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateShipmentLineDto, UpdateShipmentLineModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<ShipmentLine, ShipmentLineDto>(saved);
    }

    // Sevkiyat satırını soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
