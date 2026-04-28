using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Masters;
using InventoryTrackingAutomation.Managers.Stock;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Models.Stock;
using InventoryTrackingAutomation.Services.Masters;
using FluentValidation;
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
    // PITON arac stok gorunurlugu okuma kurallari.
    private readonly InventoryQueryManager _inventoryQueryManager;
    private readonly IValidator<CreateVehicleDto> _createValidator;
    private readonly IValidator<UpdateVehicleDto> _updateValidator;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public VehicleAppService(
        IVehicleRepository repository,
        VehicleManager manager,
        InventoryQueryManager inventoryQueryManager,
        IValidator<CreateVehicleDto> createValidator,
        IValidator<UpdateVehicleDto> updateValidator,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
        _inventoryQueryManager = inventoryQueryManager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
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

    // Arac uzerindeki envanterleri getirir; aktif gorev baglami manager tarafinda cozulur.
    public async Task<List<VehicleInventoryDto>> GetInventoriesAsync(Guid id)
    {
        var inventories = await _inventoryQueryManager.GetVehicleInventoriesAsync(id);
        return _mapper.Map<List<VehicleInventoryModel>, List<VehicleInventoryDto>>(inventories);
    }

    // Yeni araç oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<VehicleDto> CreateAsync(CreateVehicleDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
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
            await _createValidator.ValidateAndThrowAsync(dto);
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
        await _updateValidator.ValidateAndThrowAsync(input);
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
