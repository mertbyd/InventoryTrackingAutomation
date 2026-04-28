using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Movements;

// Hareket talebi satırı application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları MovementRequestLineManager'da.
//işlevi: MovementRequestLine iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class MovementRequestLineAppService : InventoryTrackingAutomationAppService, IMovementRequestLineAppService
{
    // Read/list/persist için ana repository.
    private readonly IMovementRequestLineRepository _repository;
    // Domain manager — MovementRequestId ve ProductId FK kontrolleri.
    private readonly MovementRequestLineManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public MovementRequestLineAppService(
        IMovementRequestLineRepository repository,
        MovementRequestLineManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile hareket talebi satırını getirir; yoksa EntityNotFoundException.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<MovementRequestLineDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<MovementRequestLine, MovementRequestLineDto>(entity);
    }

    // Hareket talebi satırlarını sayfalı listeler.
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<MovementRequestLineDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<MovementRequestLineDto>(
            totalCount,
            _mapper.Map<List<MovementRequestLine>, List<MovementRequestLineDto>>(entities));
    }

    // Yeni hareket talebi satırı oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<MovementRequestLineDto> CreateAsync(CreateMovementRequestLineDto input)
    {
        var model = _mapper.Map<CreateMovementRequestLineDto, CreateMovementRequestLineModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<MovementRequestLine, MovementRequestLineDto>(inserted);
    }

    // Birden fazla hareket talebi satırını toplu oluşturur.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<MovementRequestLineDto>> CreateManyAsync(List<CreateMovementRequestLineDto> inputs)
    {
        var entities = new List<MovementRequestLine>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateMovementRequestLineDto, CreateMovementRequestLineModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<MovementRequestLine>, List<MovementRequestLineDto>>(inserted);
    }

    // Hareket talebi satırını günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<MovementRequestLineDto> UpdateAsync(Guid id, UpdateMovementRequestLineDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateMovementRequestLineDto, UpdateMovementRequestLineModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<MovementRequestLine, MovementRequestLineDto>(saved);
    }

    // Hareket talebi satırını soft delete ile siler.
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
