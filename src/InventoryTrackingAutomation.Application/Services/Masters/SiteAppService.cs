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
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Lokasyon application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları SiteManager'da.
public class SiteAppService : InventoryTrackingAutomationAppService, ISiteAppService
{
    // Read/list/persist için ana repository.
    private readonly ISiteRepository _repository;
    // Domain manager — Code uniqueness, LinkedVehicle/Worker FK ve SiteType enum validasyonu.
    private readonly SiteManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public SiteAppService(
        ISiteRepository repository,
        SiteManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile lokasyonu getirir; yoksa EntityNotFoundException.
    public async Task<SiteDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Site, SiteDto>(entity);
    }

    // Lokasyonları sayfalı listeler.
    public async Task<PagedResultDto<SiteDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<SiteDto>(
            totalCount,
            _mapper.Map<List<Site>, List<SiteDto>>(entities));
    }

    // Yeni lokasyon oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<SiteDto> CreateAsync(CreateSiteDto input)
    {
        var model = _mapper.Map<CreateSiteDto, CreateSiteModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Site, SiteDto>(inserted);
    }

    // Birden fazla lokasyonu toplu oluşturur.
    [UnitOfWork]
    public async Task<List<SiteDto>> CreateManyAsync(List<CreateSiteDto> inputs)
    {
        var entities = new List<Site>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateSiteDto, CreateSiteModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Site>, List<SiteDto>>(inserted);
    }

    // Lokasyonu günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<SiteDto> UpdateAsync(Guid id, UpdateSiteDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateSiteDto, UpdateSiteModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Site, SiteDto>(saved);
    }

    // Lokasyonu soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
