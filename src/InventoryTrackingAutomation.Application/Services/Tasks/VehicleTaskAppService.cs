using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

// Arac-gorev atamasi application servisi - is kurallari VehicleTaskManager'da kalir.
//işlevi: VehicleTask iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class VehicleTaskAppService : InventoryTrackingAutomationAppService, IVehicleTaskAppService
{
    private readonly IVehicleTaskRepository _repository;
    private readonly VehicleTaskManager _manager;
    private readonly IValidator<CreateVehicleTaskDto> _createValidator;
    private readonly IValidator<UpdateVehicleTaskDto> _updateValidator;
    private readonly IMapper _mapper;

    public VehicleTaskAppService(
        IVehicleTaskRepository repository,
        VehicleTaskManager manager,
        IValidator<CreateVehicleTaskDto> createValidator,
        IValidator<UpdateVehicleTaskDto> updateValidator,
        IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<VehicleTask, VehicleTaskDto>(entity);
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<VehicleTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleTaskDto>(totalCount, _mapper.Map<List<VehicleTask>, List<VehicleTaskDto>>(entities));
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleTaskDto> CreateAsync(CreateVehicleTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateVehicleTaskDto, CreateVehicleTaskModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<VehicleTask, VehicleTaskDto>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<VehicleTaskDto>> CreateManyAsync(List<CreateVehicleTaskDto> inputs)
    {
        var entities = new List<VehicleTask>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateVehicleTaskDto, CreateVehicleTaskModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<VehicleTask>, List<VehicleTaskDto>>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<VehicleTaskDto> UpdateAsync(Guid id, UpdateVehicleTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateVehicleTaskDto, UpdateVehicleTaskModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<VehicleTask, VehicleTaskDto>(saved);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
