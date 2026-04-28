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
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Çalışan application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları WorkerManager'da.
//işlevi: Worker iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class WorkerAppService : InventoryTrackingAutomationAppService, IWorkerAppService
{
    // Read/list/persist için ana repository.
    private readonly IWorkerRepository _repository;
    // Domain manager — Department/Warehouse/Manager FK kontrolleri ve self-assignment kuralları.
    private readonly WorkerManager _manager;
    private readonly IValidator<CreateWorkerDto> _createValidator;
    private readonly IValidator<UpdateWorkerDto> _updateValidator;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public WorkerAppService(
        IWorkerRepository repository,
        WorkerManager manager,
        IValidator<CreateWorkerDto> createValidator,
        IValidator<UpdateWorkerDto> updateValidator,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
    }

    // Id ile çalışanı getirir; yoksa EntityNotFoundException.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<Worker, WorkerDto>(entity);
    }

    // Çalışanları sayfalı listeler.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<WorkerDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<WorkerDto>(
            totalCount,
            _mapper.Map<List<Worker>, List<WorkerDto>>(entities));
    }

    // Yeni çalışan oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> CreateAsync(CreateWorkerDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateWorkerDto, CreateWorkerModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<Worker, WorkerDto>(inserted);
    }

    // Birden fazla çalışanı toplu oluşturur.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<WorkerDto>> CreateManyAsync(List<CreateWorkerDto> inputs)
    {
        var entities = new List<Worker>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateWorkerDto, CreateWorkerModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<Worker>, List<WorkerDto>>(inserted);
    }

    // Çalışanı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateWorkerDto, UpdateWorkerModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<Worker, WorkerDto>(saved);
    }

    // Çalışanı soft delete ile siler.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
