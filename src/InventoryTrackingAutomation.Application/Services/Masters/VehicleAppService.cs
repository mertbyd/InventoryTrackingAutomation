using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Services.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Masters;

/// <summary>
/// Araç application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class VehicleAppService : InventoryTrackingAutomationAppService, IVehicleAppService
{
    private readonly IVehicleRepository _repository;
    private readonly VehicleManager _manager;

    public VehicleAppService(
        IVehicleRepository repository,
        VehicleManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre araç getirir. </summary>
    public async Task<VehicleDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);
        return ObjectMapper.Map<Vehicle, VehicleDto>(entity);
    }

    /// <summary> Araçları sayfalı listeler. </summary>
    public async Task<PagedResultDto<VehicleDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleDto>(
            totalCount, ObjectMapper.Map<List<Vehicle>, List<VehicleDto>>(entities));
    }

    /// <summary> Yeni araç oluşturur. </summary>
    [UnitOfWork]
    public async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
    {
        var model = ObjectMapper.Map<CreateVehicleDto, CreateVehicleModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Vehicle, VehicleDto>(inserted);
    }

    /// <summary> Birden fazla aracı toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<VehicleDto>> CreateManyAsync(List<CreateVehicleDto> inputs)
    {
        var entities = new List<Vehicle>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateVehicleDto, CreateVehicleModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Vehicle>, List<VehicleDto>>(inserted);
    }

    /// <summary> Aracı günceller. </summary>
    [UnitOfWork]
    public async Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);
        var model = ObjectMapper.Map<UpdateVehicleDto, UpdateVehicleModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Vehicle, VehicleDto>(saved);
    }

    /// <summary> Aracı soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Vehicles.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
