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

// islevi: WorkerType lookup use-case'lerini koordine eder.
// sistemdeki gorevi: Calisan tipi referans verisi icin validation, manager ve repository akisini tek AppService yuzeyinde toplar.
public class WorkerTypeAppService : InventoryTrackingAutomationAppService, IWorkerTypeAppService
{
    public WorkerTypeAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IWorkerTypeRepository _repository => LazyGetRequiredService<IWorkerTypeRepository>();
    private WorkerTypeManager _manager => LazyGetRequiredService<WorkerTypeManager>();
    private IValidator<CreateWorkerTypeDto> _createValidator => LazyGetRequiredService<IValidator<CreateWorkerTypeDto>>();
    private IValidator<UpdateWorkerTypeDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateWorkerTypeDto>>();
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    public async Task<WorkerTypeDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<WorkerType, WorkerTypeDto>(entity);
    }

    public async Task<PagedResultDto<WorkerTypeDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<WorkerTypeDto>(totalCount, _mapper.Map<List<WorkerType>, List<WorkerTypeDto>>(entities));
    }

    public async Task<WorkerTypeDto> CreateAsync(CreateWorkerTypeDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateWorkerTypeDto, CreateWorkerTypeModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<WorkerType, WorkerTypeDto>(inserted);
    }

    public async Task<List<WorkerTypeDto>> CreateManyAsync(List<CreateWorkerTypeDto> inputs)
    {
        var entities = new List<WorkerType>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateWorkerTypeDto, CreateWorkerTypeModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<WorkerType>, List<WorkerTypeDto>>(inserted);
    }

    public async Task<WorkerTypeDto> UpdateAsync(Guid id, UpdateWorkerTypeDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateWorkerTypeDto, UpdateWorkerTypeModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<WorkerType, WorkerTypeDto>(saved);
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        // islevi: Lookup entity soft-delete tasimadigi icin fiziksel delete uygulanir.
        // sistemdeki gorevi: Kullanilan calisan tiplerini FK ile korur, kullanilmayan referanslari temizler.
        await _repository.DeleteAsync(existing, autoSave: true);
    }
}

