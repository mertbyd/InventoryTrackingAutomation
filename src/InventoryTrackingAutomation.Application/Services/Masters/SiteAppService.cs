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
/// Lokasyon application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class SiteAppService : InventoryTrackingAutomationAppService, ISiteAppService
{
    private readonly ISiteRepository _repository;
    private readonly SiteManager _manager;

    public SiteAppService(
        ISiteRepository repository,
        SiteManager manager)
    {
        _repository = repository;
        _manager = manager;
    }

    /// <summary> Id'ye göre lokasyon getirir. </summary>
    public async Task<SiteDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        return ObjectMapper.Map<Site, SiteDto>(entity);
    }

    /// <summary> Lokasyonları sayfalı listeler. </summary>
    public async Task<PagedResultDto<SiteDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<SiteDto>(
            totalCount, ObjectMapper.Map<List<Site>, List<SiteDto>>(entities));
    }

    /// <summary> Yeni lokasyon oluşturur. </summary>
    [UnitOfWork]
    public async Task<SiteDto> CreateAsync(CreateSiteDto input)
    {
        var model = ObjectMapper.Map<CreateSiteDto, CreateSiteModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return ObjectMapper.Map<Site, SiteDto>(inserted);
    }

    /// <summary> Birden fazla lokasyonu toplu oluşturur. </summary>
    [UnitOfWork]
    public async Task<List<SiteDto>> CreateManyAsync(List<CreateSiteDto> inputs)
    {
        var entities = new List<Site>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateSiteDto, CreateSiteModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return ObjectMapper.Map<List<Site>, List<SiteDto>>(inserted);
    }

    /// <summary> Lokasyonu günceller. </summary>
    [UnitOfWork]
    public async Task<SiteDto> UpdateAsync(Guid id, UpdateSiteDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        var model = ObjectMapper.Map<UpdateSiteDto, UpdateSiteModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<Site, SiteDto>(saved);
    }

    /// <summary> Lokasyonu soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        await _repository.SoftDeleteAsync(id);
    }
}
