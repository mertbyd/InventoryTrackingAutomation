using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Movements;

/// <summary>
/// Hareket talebi satırı application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class MovementRequestLineAppService : InventoryTrackingAutomationAppService, IMovementRequestLineAppService
{
    private readonly IMovementRequestLineRepository _repository;
    private readonly MovementRequestLineManager _manager;

    public MovementRequestLineAppService(
        IMovementRequestLineRepository repository,
        MovementRequestLineManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre hareket talebi satırı getirir. </summary>
    public async Task<MovementRequestLineDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequestLines.NotFound);
        return ObjectMapper.Map<MovementRequestLine, MovementRequestLineDto>(entity);
    }

    /// <summary> Hareket talebi satırlarını sayfalı listeler. </summary>
    public async Task<PagedResultDto<MovementRequestLineDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<MovementRequestLineDto>(
            totalCount, ObjectMapper.Map<List<MovementRequestLine>, List<MovementRequestLineDto>>(entities));
    }

    /// <summary> Yeni hareket talebi satırı oluşturur. </summary>
    [UnitOfWork]
    public async Task<MovementRequestLineDto> CreateAsync(CreateMovementRequestLineDto input)
    {
        var model = ObjectMapper.Map<CreateMovementRequestLineDto, CreateMovementRequestLineModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<MovementRequestLine, MovementRequestLineDto>(inserted);
    }

    /// <summary> Birden fazla hareket talebi satırını toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<MovementRequestLineDto>> CreateManyAsync(List<CreateMovementRequestLineDto> inputs)
    {
        var entities = new List<MovementRequestLine>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateMovementRequestLineDto, CreateMovementRequestLineModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<MovementRequestLine>, List<MovementRequestLineDto>>(inserted);
    }

    /// <summary> Hareket talebi satırını günceller. </summary>
    [UnitOfWork]
    public async Task<MovementRequestLineDto> UpdateAsync(Guid id, UpdateMovementRequestLineDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequestLines.NotFound);
        var model = ObjectMapper.Map<UpdateMovementRequestLineDto, UpdateMovementRequestLineModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<MovementRequestLine, MovementRequestLineDto>(saved);
    }

    /// <summary> Hareket talebi satırını soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequestLines.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
