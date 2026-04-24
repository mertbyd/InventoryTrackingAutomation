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
/// Çalışan application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class WorkerAppService : InventoryTrackingAutomationAppService, IWorkerAppService
{
    private readonly IWorkerRepository _repository;
    private readonly WorkerManager _manager;

    public WorkerAppService(
        IWorkerRepository repository,
        WorkerManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre çalışan getirir. </summary>
    public async Task<WorkerDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);
        return ObjectMapper.Map<Worker, WorkerDto>(entity);
    }

    /// <summary> Çalışanları sayfalı listeler. </summary>
    public async Task<PagedResultDto<WorkerDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<WorkerDto>(
            totalCount, ObjectMapper.Map<List<Worker>, List<WorkerDto>>(entities));
    }

    /// <summary> Yeni çalışan oluşturur. </summary>
    [UnitOfWork]
    public async Task<WorkerDto> CreateAsync(CreateWorkerDto input)
    {
        var model = ObjectMapper.Map<CreateWorkerDto, CreateWorkerModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Worker, WorkerDto>(inserted);
    }

    /// <summary> Birden fazla çalışanı toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<WorkerDto>> CreateManyAsync(List<CreateWorkerDto> inputs)
    {
        var entities = new List<Worker>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateWorkerDto, CreateWorkerModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Worker>, List<WorkerDto>>(inserted);
    }

    /// <summary> Çalışanı günceller. </summary>
    [UnitOfWork]
    public async Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);
        var model = ObjectMapper.Map<UpdateWorkerDto, UpdateWorkerModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Worker, WorkerDto>(saved);
    }

    /// <summary> Çalışanı soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
