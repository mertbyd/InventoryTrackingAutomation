using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
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
//işlevi: MovementRequest iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class MovementRequestAppService : InventoryTrackingAutomationAppService, IMovementRequestAppService
{
    // Read/list ve update/delete persist için ana repository.
    private readonly IMovementRequestRepository _repository;
    // Domain manager — iş kuralları, validasyon, workflow tetikleme.
    private readonly MovementRequestManager _manager;
    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public MovementRequestAppService(
        IMovementRequestRepository repository,
        MovementRequestManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    /// Hareket talebi verisini getirmek için kullanılır.
    public async Task<MovementRequestDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<MovementRequest, MovementRequestDto>(entity);
    }

    /// Hareket talebi listesini getirmek için kullanılır.
    public async Task<PagedResultDto<MovementRequestDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<MovementRequestDto>(
            totalCount,
            _mapper.Map<List<MovementRequest>, List<MovementRequestDto>>(entities));
    }

    /// Yeni bir hareket talebi oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input)
    {
        var currentUserId = CurrentUser.GetId();
        var model = _mapper.Map<CreateMovementRequestDto, CreateMovementRequestModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();

        // Manager Create + Workflow assignment + Insert akışını birlikte yürütür.
        var inserted = await _manager.CreateWithWorkflowAsync(model, currentUserId);
        return _mapper.Map<MovementRequest, MovementRequestDto>(inserted);
    }

    /// Birden fazla hareket talebini toplu olarak oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task<List<MovementRequestDto>> CreateManyAsync(List<CreateMovementRequestDto> inputs)
    {
        var currentUserId = CurrentUser.GetId();
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

    /// Mevcut bir hareket talebini güncellemek için kullanılır.
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

    /// Hareket talebi sevkiyatını gerçekleştirmek için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> DispatchAsync(Guid id, DispatchMovementRequestDto input)
    {
        var dispatched = await _manager.DispatchAsync(
            id,
            input.DispatchNote,
            CurrentUser.GetId(),
            await ResolveCurrentWorkerIdAsync());

        return _mapper.Map<MovementRequest, MovementRequestDto>(dispatched);
    }

    /// Hareket talebini teslim almak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> ReceiveAsync(Guid id, ReceiveMovementRequestDto input)
    {
        var model = _mapper.Map<ReceiveMovementRequestDto, ReceiveMovementRequestModel>(input);
        var received = await _manager.ReceiveAsync(
            id,
            model,
            CurrentUser.GetId());

        return _mapper.Map<MovementRequest, MovementRequestDto>(received);
    }

    /// Hareket talebini satırları ile birlikte oluşturmak için kullanılır.
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateWithLinesAsync(CreateMovementRequestWithLinesDto input)
    {
        var currentUserId = CurrentUser.GetId();

        var model = _mapper.Map<CreateMovementRequestWithLinesDto, CreateMovementRequestWithLinesModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();

        var inserted = await _manager.CreateWithLinesAndWorkflowAsync(model, currentUserId);

        return _mapper.Map<MovementRequest, MovementRequestDto>(inserted);
    }

    /// Hareket talebini silmek için kullanılır.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        var request = await _manager.EnsureExistsAsync(id);
        if (request.Status != MovementStatusEnum.Pending)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.InvalidStateTransition)
                .WithData("MovementRequestId", id)
                .WithData("CurrentStatus", request.Status)
                .WithData("AllowedStatus", MovementStatusEnum.Pending);
        }

        await _repository.SoftDeleteAsync(id);
    }

    }
