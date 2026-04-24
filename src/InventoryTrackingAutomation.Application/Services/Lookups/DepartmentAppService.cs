using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

/// <summary>
/// Departman application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class DepartmentAppService : InventoryTrackingAutomationAppService, IDepartmentAppService
{
    private readonly IDepartmentRepository _repository;
    private readonly DepartmentManager _manager;

    public DepartmentAppService(
        IDepartmentRepository repository,
        DepartmentManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre departman getirir. </summary>
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Departments.NotFound);
        return ObjectMapper.Map<Department, DepartmentDto>(entity);
    }

    /// <summary> Departmanları sayfalı listeler. </summary>
    public async Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<DepartmentDto>(
            totalCount, ObjectMapper.Map<List<Department>, List<DepartmentDto>>(entities));
    }

    /// <summary> Yeni departman oluşturur. </summary>
    [UnitOfWork]
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        var model = ObjectMapper.Map<CreateDepartmentDto, CreateDepartmentModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Department, DepartmentDto>(inserted);
    }

    /// <summary> Birden fazla departmanı toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<DepartmentDto>> CreateManyAsync(List<CreateDepartmentDto> inputs)
    {
        var entities = new List<Department>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateDepartmentDto, CreateDepartmentModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Department>, List<DepartmentDto>>(inserted);
    }

    /// <summary> Departmanı günceller. </summary>
    [UnitOfWork]
    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Departments.NotFound);
        var model = ObjectMapper.Map<UpdateDepartmentDto, UpdateDepartmentModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Department, DepartmentDto>(saved);
    }

    /// <summary> Departmanı soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Departments.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
