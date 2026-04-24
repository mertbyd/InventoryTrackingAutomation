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
/// Sevkiyat application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class ShipmentAppService : InventoryTrackingAutomationAppService, IShipmentAppService
{
    private readonly IShipmentRepository _repository;
    private readonly ShipmentManager _manager;

    public ShipmentAppService(
        IShipmentRepository repository,
        ShipmentManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre sevkiyat getirir. </summary>
    public async Task<ShipmentDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);
        return ObjectMapper.Map<Shipment, ShipmentDto>(entity);
    }

    /// <summary> Sevkiyatları sayfalı listeler. </summary>
    public async Task<PagedResultDto<ShipmentDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ShipmentDto>(
            totalCount, ObjectMapper.Map<List<Shipment>, List<ShipmentDto>>(entities));
    }

    /// <summary> Yeni sevkiyat oluşturur. </summary>
    [UnitOfWork]
    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto input)
    {
        var model = ObjectMapper.Map<CreateShipmentDto, CreateShipmentModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Shipment, ShipmentDto>(inserted);
    }

    /// <summary> Birden fazla sevkiyatı toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<ShipmentDto>> CreateManyAsync(List<CreateShipmentDto> inputs)
    {
        var entities = new List<Shipment>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateShipmentDto, CreateShipmentModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Shipment>, List<ShipmentDto>>(inserted);
    }

    /// <summary> Sevkiyatı günceller. </summary>
    [UnitOfWork]
    public async Task<ShipmentDto> UpdateAsync(Guid id, UpdateShipmentDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);
        var model = ObjectMapper.Map<UpdateShipmentDto, UpdateShipmentModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Shipment, ShipmentDto>(saved);
    }

    /// <summary> Sevkiyatı soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
