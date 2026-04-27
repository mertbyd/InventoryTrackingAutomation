using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation.Application.Services.Movements;

// Hareket talebi application servisi — HTTP endpoint'leri için ince orkestra katmanı.
// İş kuralları manager'da, persist sorumluluğu manager + repository'de.
public class MovementRequestAppService : InventoryTrackingAutomationAppService, IMovementRequestAppService
{
    // Read/list ve update/delete persist için ana repository.
    private readonly IMovementRequestRepository _repository;
    // Domain manager — iş kuralları, validasyon, workflow tetikleme.
    private readonly MovementRequestManager _manager;
    // Current user → Worker eşlemesi için.
    private readonly IWorkerRepository _workerRepository;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public MovementRequestAppService(
        IMovementRequestRepository repository,
        MovementRequestManager manager,
        IWorkerRepository workerRepository,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
        _workerRepository = workerRepository;
    }

    // Id ile hareket talebini getirir; yoksa EntityNotFoundException.
    public async Task<MovementRequestDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<MovementRequest, MovementRequestDto>(entity);
    }

    // Hareket taleplerini sayfalı listeler.
    public async Task<PagedResultDto<MovementRequestDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<MovementRequestDto>(
            totalCount,
            _mapper.Map<List<MovementRequest>, List<MovementRequestDto>>(entities));
    }

    // Yeni hareket talebi oluşturur — RequestedByWorkerId CurrentUser'dan çözümlenir, manager workflow'u tetikler.
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input)
    {
        var currentUserId = CurrentUserId;
        var model = _mapper.Map<CreateMovementRequestDto, CreateMovementRequestModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();

        // Manager Create + Workflow assignment + Insert akışını birlikte yürütür.
        var inserted = await _manager.CreateWithWorkflowAsync(model, currentUserId);
        return _mapper.Map<MovementRequest, MovementRequestDto>(inserted);
    }

    // Birden fazla hareket talebini toplu oluşturur — her biri için workflow tetiklenir.
    [UnitOfWork]
    public async Task<List<MovementRequestDto>> CreateManyAsync(List<CreateMovementRequestDto> inputs)
    {
        var currentUserId = CurrentUserId;
        var currentWorkerId = await ResolveCurrentWorkerIdAsync();

        // DTO listesini Model listesine map'le ve current worker bilgisini set et.
        var models = new List<CreateMovementRequestModel>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateMovementRequestDto, CreateMovementRequestModel>(dto);
            model.RequestedByWorkerId = currentWorkerId;
            models.Add(model);
        }

        // Manager toplu Create + Workflow + Insert akışını yürütür.
        var inserted = await _manager.CreateManyWithWorkflowAsync(models, currentUserId);

        return _mapper.Map<List<MovementRequest>, List<MovementRequestDto>>(inserted);
    }

    // Hareket talebini günceller — manager iş kurallarını işler, sonra repository persist eder.
    [UnitOfWork]
    public async Task<MovementRequestDto> UpdateAsync(Guid id, UpdateMovementRequestDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateMovementRequestDto, UpdateMovementRequestModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();

        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<MovementRequest, MovementRequestDto>(saved);
    }

    // Talebi + satırları + workflow'u tek atomik UoW içinde oluşturur.
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateWithLinesAsync(CreateMovementRequestWithLinesDto input)
    {
        var currentUserId = CurrentUserId;

        var model = _mapper.Map<CreateMovementRequestWithLinesDto, CreateMovementRequestWithLinesModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();

        var inserted = await _manager.CreateWithLinesAndWorkflowAsync(model, currentUserId);

        return _mapper.Map<MovementRequest, MovementRequestDto>(inserted);
    }

    // Hareket talebini soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }

    // Mevcut kullanıcının Worker Id'sini bulur; yoksa Workers.NotFound hatası fırlatır.
    private async Task<Guid> ResolveCurrentWorkerIdAsync()
    {
        var userId = CurrentUserId;
        var worker = await _workerRepository.FindAsync(w => w.UserId == userId);
        if (worker == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workers.NotFound)
                .WithData("UserId", userId);
        }
        return worker.Id;
    }

    // Authenticated kullanıcının Id'sine kısa erişim helper'ı.
    private Guid CurrentUserId => CurrentUser.GetId();
}
