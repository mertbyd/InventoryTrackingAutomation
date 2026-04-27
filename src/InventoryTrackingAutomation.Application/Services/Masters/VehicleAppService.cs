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

// Araç application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları VehicleManager'da.
public class VehicleAppService : InventoryTrackingAutomationAppService, IVehicleAppService
{
    // Read/list/persist için ana repository.
    private readonly IVehicleRepository _repository;
    // Domain manager — PlateNumber uniqueness ve VehicleType enum validasyonu.
    private readonly VehicleManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public VehicleAppService(
        IVehicleRepository repository,
        VehicleManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile aracı getirir; yoksa EntityNotFoundException.
    public async Task<VehicleDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Vehicle, VehicleDto>(entity);
    }

    // Araçları sayfalı listeler.
    public async Task<PagedResultDto<VehicleDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleDto>(
            totalCount,
            _mapper.Map<List<Vehicle>, List<VehicleDto>>(entities));
    }

    // Yeni araç oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
    {
        var model = _mapper.Map<CreateVehicleDto, CreateVehicleModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Vehicle, VehicleDto>(inserted);
    }

    // Birden fazla aracı toplu oluşturur.
    [UnitOfWork]
    public async Task<List<VehicleDto>> CreateManyAsync(List<CreateVehicleDto> inputs)
    {
        var entities = new List<Vehicle>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateVehicleDto, CreateVehicleModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Vehicle>, List<VehicleDto>>(inserted);
    }

    // Aracı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<VehicleDto> UpdateAsync(Guid id, UpdateVehicleDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateVehicleDto, UpdateVehicleModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Vehicle, VehicleDto>(saved);
    }

    // Aracı soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
