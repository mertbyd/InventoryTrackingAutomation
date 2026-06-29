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

// islevi: UnitType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Olcu birimi referans verisi icin validation, manager ve repository akisini tek AppService yuzeyinde toplar.
public class UnitTypeAppService : InventoryTrackingAutomationAppService, IUnitTypeAppService
{
    public UnitTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IUnitTypeRepository _repository => LazyGetRequiredService<IUnitTypeRepository>();
    private UnitTypeManager _manager => LazyGetRequiredService<UnitTypeManager>();
    private IValidator<CreateUnitTypeDto> _createValidator => LazyGetRequiredService<IValidator<CreateUnitTypeDto>>();
    private IValidator<UpdateUnitTypeDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateUnitTypeDto>>();
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public async Task<UnitTypeDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<UnitType, UnitTypeDto>(entity);
    }

    public async Task<PagedResultDto<UnitTypeDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<UnitTypeDto>(totalCount, _mapper.Map<List<UnitType>, List<UnitTypeDto>>(entities));
    }

    public async Task<UnitTypeDto> CreateAsync(CreateUnitTypeDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateUnitTypeDto, CreateUnitTypeModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<UnitType, UnitTypeDto>(inserted);
    }

    public async Task<List<UnitTypeDto>> CreateManyAsync(List<CreateUnitTypeDto> inputs)
    {
        var entities = new List<UnitType>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateUnitTypeDto, CreateUnitTypeModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<UnitType>, List<UnitTypeDto>>(inserted);
    }

    public async Task<UnitTypeDto> UpdateAsync(Guid id, UpdateUnitTypeDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateUnitTypeDto, UpdateUnitTypeModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<UnitType, UnitTypeDto>(saved);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        // islevi: Lookup entity soft-delete tasimadigi icin fiziksel delete uygulanir.
        // sistemdeki gorevi: Kullanilan olcu birimlerini FK ile korur, kullanilmayan referanslari temizler.
        await _repository.DeleteAsync(existing, autoSave: true);
    }
}

