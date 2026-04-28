using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// Departman application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları DepartmentManager'da.
public class DepartmentAppService : InventoryTrackingAutomationAppService, IDepartmentAppService
{
    // Read/list/persist için ana repository.
    private readonly IDepartmentRepository _repository;
    // Domain manager — Code uniqueness ve diğer iş kuralları.
    private readonly DepartmentManager _manager;
    private readonly IValidator<CreateDepartmentDto> _createValidator;
    private readonly IValidator<UpdateDepartmentDto> _updateValidator;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public DepartmentAppService(
        IDepartmentRepository repository,
        DepartmentManager manager,
        IValidator<CreateDepartmentDto> createValidator,
        IValidator<UpdateDepartmentDto> updateValidator,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // Id ile departmanı getirir; yoksa EntityNotFoundException.
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Department, DepartmentDto>(entity);
    }

    // Departmanları sayfalı listeler.
    public async Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<DepartmentDto>(
            totalCount,
            _mapper.Map<List<Department>, List<DepartmentDto>>(entities));
    }

    // Yeni departman oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateDepartmentDto, CreateDepartmentModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Department, DepartmentDto>(inserted);
    }

    // Birden fazla departmanı toplu oluşturur.
    [UnitOfWork]
    public async Task<List<DepartmentDto>> CreateManyAsync(List<CreateDepartmentDto> inputs)
    {
        var entities = new List<Department>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateDepartmentDto, CreateDepartmentModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Department>, List<DepartmentDto>>(inserted);
    }

    // Departmanı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateDepartmentDto, UpdateDepartmentModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Department, DepartmentDto>(saved);
    }

    // Departmanı soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
