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

/// <summary>
/// Sevkiyat satırı application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class ShipmentLineAppService : InventoryTrackingAutomationAppService, IShipmentLineAppService
{
    private readonly IShipmentLineRepository _repository;
    private readonly ShipmentLineManager _manager;

    public ShipmentLineAppService(
        IShipmentLineRepository repository,
        ShipmentLineManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre sevkiyat satırı getirir. </summary>
    public async Task<ShipmentLineDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ShipmentLines.NotFound);
        return ObjectMapper.Map<ShipmentLine, ShipmentLineDto>(entity);
    }

    /// <summary> Sevkiyat satırlarını sayfalı listeler. </summary>
    public async Task<PagedResultDto<ShipmentLineDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ShipmentLineDto>(
            totalCount, ObjectMapper.Map<List<ShipmentLine>, List<ShipmentLineDto>>(entities));
    }

    /// <summary> Yeni sevkiyat satırı oluşturur. </summary>
    [UnitOfWork]
    public async Task<ShipmentLineDto> CreateAsync(CreateShipmentLineDto input)
    {
        var model = ObjectMapper.Map<CreateShipmentLineDto, CreateShipmentLineModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<ShipmentLine, ShipmentLineDto>(inserted);
    }

    /// <summary> Birden fazla sevkiyat satırını toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<ShipmentLineDto>> CreateManyAsync(List<CreateShipmentLineDto> inputs)
    {
        var entities = new List<ShipmentLine>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateShipmentLineDto, CreateShipmentLineModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<ShipmentLine>, List<ShipmentLineDto>>(inserted);
    }

    /// <summary> Sevkiyat satırını günceller. </summary>
    [UnitOfWork]
    public async Task<ShipmentLineDto> UpdateAsync(Guid id, UpdateShipmentLineDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ShipmentLines.NotFound);
        var model = ObjectMapper.Map<UpdateShipmentLineDto, UpdateShipmentLineModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<ShipmentLine, ShipmentLineDto>(saved);
    }

    /// <summary> Sevkiyat satırını soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.ShipmentLines.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
