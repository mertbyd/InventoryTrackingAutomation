using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Workflows;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Managers.Workflows;
using InventoryTrackingAutomation.Models.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using Volo.Abp;
using Volo.Abp.Users;
using Volo.Abp.Uow;
using Volo.Abp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace InventoryTrackingAutomation.Services.Workflows;

// Dinamik iş akışlarını dış dünyaya açan application servisi — başlatma ve onay/red yönlendirmesi.
public class WorkflowAppService : InventoryTrackingAutomationAppService, IWorkflowAppService
{
    // Domain manager — iş akışı state machine ve onaycı çözümleme.
    private readonly WorkflowManager _workflowManager;
    // Yeni instance persist için repository.
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    // Onay/red sonrası adım güncellemesi için repository.
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;
    // Step definition bilgilerini okumak için repository.
    private readonly IRepository<WorkflowStepDefinition, Guid> _stepDefinitionRepository;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public WorkflowAppService(
        WorkflowManager workflowManager,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowInstanceStepRepository workflowInstanceStepRepository,
        IRepository<WorkflowStepDefinition, Guid> stepDefinitionRepository,
        IMapper mapper)
    {
        _mapper = mapper;
        _workflowManager = workflowManager;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
        _stepDefinitionRepository = stepDefinitionRepository;
    }

    // Yeni iş akışı süreci başlatır — initiator CurrentUser'dan çözülür, manager state machine'i kurar, instance persist edilir.
    [UnitOfWork]
    public async Task<WorkflowInstanceDto> StartAsync(StartWorkflowDto input)
    {
        var currentUserId = CurrentUser.Id.Value;
        var model = _mapper.Map<StartWorkflowDto, StartWorkflowModel>(input);
        model.InitiatorUserId = currentUserId;
        var entity = await _workflowManager.StartWorkflowAsync(model);
        var inserted = await _workflowInstanceRepository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<WorkflowInstance, WorkflowInstanceDto>(inserted);
    }

    // Belirtilen iş akışı adımında onay/red aksiyonunu işler — yetki kontrolünü manager yapar, kararı persist eder.
    [UnitOfWork]
    public async Task<WorkflowInstanceStepDto> ProcessApprovalAsync(ProcessApprovalDto input)
    {
        var currentUserId = CurrentUser.Id.Value;
        var roles = CurrentUser.Roles != null ? CurrentUser.Roles.ToList() : new List<string>();
        var model = _mapper.Map<ProcessApprovalDto, ProcessApprovalModel>(input);
        model.CurrentUserId = currentUserId;
        model.CurrentUserRoles = roles;
        var entityStep = await _workflowManager.ProcessApprovalAsync(model);
        var updated = await _workflowInstanceStepRepository.UpdateAsync(entityStep, autoSave: true);
        return _mapper.Map<WorkflowInstanceStep, WorkflowInstanceStepDto>(updated);
    }

    // Mevcut kullanıcıya atanmış tüm bekleyen iş akışı adımlarını entity-agnostic olarak döner.
    public async Task<List<PendingWorkflowStepDto>> GetMyPendingApprovalsAsync()
    {
        var currentUserId = CurrentUser.Id.Value;

        // Kullanıcıya atanmış ve henüz karar verilmemiş step'leri bul
        var pendingSteps = await _workflowInstanceStepRepository.GetListAsync(
            x => x.AssignedUserId == currentUserId &&
                 x.ActionTaken == InventoryTrackingAutomation.Enums.Workflows.WorkflowActionType.Pending);

        var result = new List<PendingWorkflowStepDto>();

        foreach (var step in pendingSteps)
        {
            var instance = await _workflowInstanceRepository.FindAsync(step.WorkflowInstanceId);
            if (instance == null) continue;

            var stepDef = await _stepDefinitionRepository.FindAsync(step.WorkflowStepDefinitionId);

            result.Add(new PendingWorkflowStepDto
            {
                WorkflowInstanceStepId = step.Id,
                WorkflowInstanceId = instance.Id,
                EntityType = instance.EntityType,
                EntityId = instance.EntityId,
                StepOrder = stepDef?.StepOrder ?? 0,
                StepName = stepDef?.RequiredRoleName ?? stepDef?.ResolverKey ?? string.Empty,
                InitiatorUserId = instance.InitiatorUserId,
                CreatedAt = step.CreationTime
            });
        }

        return result;
    }
}
