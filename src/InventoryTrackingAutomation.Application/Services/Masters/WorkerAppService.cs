using InventoryTrackingAutomation.Application.Mappers.Masters;
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
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Masters;

// Çalışan application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları WorkerManager'da.
//işlevi: Worker iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class WorkerAppService : InventoryTrackingAutomationAppService, IWorkerAppService
{
    public WorkerAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list/persist için ana repository.
    private IWorkerRepository _repository => LazyGetRequiredService<IWorkerRepository>();
    // Domain manager — Department/Warehouse/Manager FK kontrolleri ve self-assignment kuralları.
    private WorkerManager _manager => LazyGetRequiredService<WorkerManager>();
    private IValidator<CreateWorkerDto> _createValidator => LazyGetRequiredService<IValidator<CreateWorkerDto>>();
    private IValidator<UpdateWorkerDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateWorkerDto>>();

    // Tüm bağımlılıkları DI ile alır.
    private static readonly WorkerMapper _mapper = new WorkerMapper();


    // Id ile çalışanı getirir; yoksa EntityNotFoundException.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    // Çalışanları sayfalı listeler.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<WorkerDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty, includeDetails: true);
        return new PagedResultDto<WorkerDto>(
            totalCount,
            _mapper.MapToDto(entities));
    }

    // Yeni çalışan oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> CreateAsync(CreateWorkerDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new Worker(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    // Birden fazla çalışanı toplu oluşturur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<WorkerDto>> CreateManyAsync(List<CreateWorkerDto> inputs)
    {
        var models = new List<CreateWorkerModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<Worker>();
        foreach (var model in validatedModels)
        {
            var entity = new Worker(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    // Çalışanı günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    // Calisani silmek yerine pasife alir; gorev, talep ve onay gecmisi korunur.
    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var passivated = await _manager.PassivateAsync(existing);
        await _repository.UpdateAsync(passivated, autoSave: true);
    }
}
