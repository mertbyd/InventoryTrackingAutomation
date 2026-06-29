using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// islevi: VehicleType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Arac tipi referans verisi icin validation, manager ve repository akisini tek AppService yuzeyinde toplar.
public class VehicleTypeAppService : InventoryTrackingAutomationAppService, IVehicleTypeAppService
{
    public VehicleTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTypeRepository _repository => LazyGetRequiredService<IVehicleTypeRepository>();
    private VehicleTypeManager _manager => LazyGetRequiredService<VehicleTypeManager>();
    private IValidator<CreateVehicleTypeDto> _createValidator => LazyGetRequiredService<IValidator<CreateVehicleTypeDto>>();
    private IValidator<UpdateVehicleTypeDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateVehicleTypeDto>>();
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public async Task<VehicleTypeDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<VehicleType, VehicleTypeDto>(entity);
    }

    public async Task<PagedResultDto<VehicleTypeDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleTypeDto>(totalCount, _mapper.Map<List<VehicleType>, List<VehicleTypeDto>>(entities));
    }

    public async Task<VehicleTypeDto> CreateAsync(CreateVehicleTypeDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateVehicleTypeDto, CreateVehicleTypeModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<VehicleType, VehicleTypeDto>(inserted);
    }

    public async Task<List<VehicleTypeDto>> CreateManyAsync(List<CreateVehicleTypeDto> inputs)
    {
        var entities = new List<VehicleType>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateVehicleTypeDto, CreateVehicleTypeModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<VehicleType>, List<VehicleTypeDto>>(inserted);
    }

    public async Task<VehicleTypeDto> UpdateAsync(Guid id, UpdateVehicleTypeDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateVehicleTypeDto, UpdateVehicleTypeModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<VehicleType, VehicleTypeDto>(saved);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        // islevi: Lookup entity soft-delete tasimadigi icin fiziksel delete uygulanir.
        // sistemdeki gorevi: Kullanilan arac tiplerini FK ile korur, kullanilmayan referanslari temizler.
        await _repository.DeleteAsync(existing, autoSave: true);
    }
}

