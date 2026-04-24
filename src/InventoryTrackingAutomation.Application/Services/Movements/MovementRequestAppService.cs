using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using InventoryTrackingAutomation.Services.Workflows;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation.Application.Services.Movements;

/// <summary>
/// Hareket talebi application servisi — HTTP endpoint'leri için orkestra katmanı.
/// </summary>
public class MovementRequestAppService : InventoryTrackingAutomationAppService, IMovementRequestAppService
{
    private readonly IMovementRequestRepository _repository;
    private readonly MovementRequestManager _manager;
    private readonly IWorkflowAppService _workflowAppService;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkerRepository _workerRepository;
    private const string MovementRequestWorkflowName = "MovementRequest";

    public MovementRequestAppService(
        IMovementRequestRepository repository,
        MovementRequestManager manager,
        IWorkflowAppService workflowAppService,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkerRepository workerRepository)
    {
        _repository = repository;
        _manager = manager;
        _workflowAppService = workflowAppService;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workerRepository = workerRepository;
    }

    /// <summary> Id'ye göre hareket talebi getirir. </summary>
    public async Task<MovementRequestDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequests.NotFound);
        return ObjectMapper.Map<MovementRequest, MovementRequestDto>(entity);
    }

    /// <summary> Hareket taleplerini sayfalı listeler. </summary>
    public async Task<PagedResultDto<MovementRequestDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<MovementRequestDto>(
            totalCount, ObjectMapper.Map<List<MovementRequest>, List<MovementRequestDto>>(entities));
    }

    /// <summary> Yeni hareket talebi oluşturur — talep eden worker CurrentUser'dan çözümlenir. </summary>
    [UnitOfWork]
    public async Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input)
    {
        var currentUserId = CurrentUserId;
        var model = ObjectMapper.Map<CreateMovementRequestDto, CreateMovementRequestModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();
        var inserted = await _manager.CreateWithWorkflowAsync(model, currentUserId);
        return ObjectMapper.Map<MovementRequest, MovementRequestDto>(inserted);
    }

    /// <summary> Birden fazla hareket talebini toplu oluşturur — her biri için workflow tetiklenir. </summary>
    [UnitOfWork]
    public async Task<List<MovementRequestDto>> CreateManyAsync(List<CreateMovementRequestDto> inputs)
    {
        var currentUserId = CurrentUserId;
        var currentWorkerId = await ResolveCurrentWorkerIdAsync();

        var models = new List<CreateMovementRequestModel>();
        foreach (var dto in inputs)
        {
            var model = ObjectMapper.Map<CreateMovementRequestDto, CreateMovementRequestModel>(dto);
            model.RequestedByWorkerId = currentWorkerId;
            models.Add(model);
        }

        var inserted = await _manager.CreateManyWithWorkflowAsync(models, currentUserId);

        return ObjectMapper.Map<List<MovementRequest>, List<MovementRequestDto>>(inserted);
    }

    /// <summary> Hareket talebini günceller. </summary>
    [UnitOfWork]
    public async Task<MovementRequestDto> UpdateAsync(Guid id, UpdateMovementRequestDto input)
    {
        var existing = await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequests.NotFound);
        var model = ObjectMapper.Map<UpdateMovementRequestDto, UpdateMovementRequestModel>(input);
        model.RequestedByWorkerId = await ResolveCurrentWorkerIdAsync();
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return ObjectMapper.Map<MovementRequest, MovementRequestDto>(saved);
    }

    /// <summary> Hareket talebini soft delete ile siler. </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(
            id, InventoryTrackingAutomationDomainErrorCodes.MovementRequests.NotFound);
        await _repository.SoftDeleteAsync(id);
    }

    private async Task<Guid> ResolveCurrentWorkerIdAsync()
    {
        var userId = CurrentUserId;
        var worker = await _workerRepository.FindAsync(w => w.UserId == userId);
        if (worker == null)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound)
                .WithData("UserId", userId);
        }
        return worker.Id;
    }

    private Guid CurrentUserId => CurrentUser.GetId();
}
