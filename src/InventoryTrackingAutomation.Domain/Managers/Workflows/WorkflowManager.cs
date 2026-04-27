using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers.Workflows.Approvers;
using InventoryTrackingAutomation.Models.Workflows;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace InventoryTrackingAutomation.Managers.Workflows;

// Dinamik iş akışı yürütme manager'ı — instance başlatır, çok adımlı state machine ile onayları yönlendirir.
// Onaycı çözümleme tamamen IWorkflowApproverResolver'a delege edilir; manager initiator/site mantığı bilmez.
public class WorkflowManager : DomainService
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;
    private readonly ILocalEventBus _localEventBus;
    private readonly IWorkflowApproverResolver _workflowApproverResolver;

    private readonly IMapper _mapper;
    public WorkflowManager(
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowInstanceStepRepository workflowInstanceStepRepository,
        ILocalEventBus localEventBus,
        IWorkflowApproverResolver workflowApproverResolver,
        IMapper mapper)
    {
        _mapper = mapper;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
        _localEventBus = localEventBus;
        _workflowApproverResolver = workflowApproverResolver;
    }

    // Yeni iş akışı başlatır: instance oluşturur, ilk adımın tanımını yükler, ilk onaycıyı çözer ve adımı kuyruğa alır.
    public async Task<WorkflowInstance> StartWorkflowAsync(StartWorkflowModel model)
    {
        var definition = await FindAndValidateDefinitionAsync(model.WorkflowDefinitionId);

        var instance = new WorkflowInstance(
            id: GuidGenerator.Create(),
            workflowDefinitionId: definition.Id,
            entityType: model.EntityType,
            entityId: model.EntityId,
            state: WorkflowState.Active,
            initiatorUserId: model.InitiatorUserId
        );

        var firstStepDef = definition.Steps.OrderBy(x => x.StepOrder).First();
        var context = new ApproverContext(model.EntityType, model.EntityId, model.InitiatorUserId);
        var firstApprover = await _workflowApproverResolver.ResolveApproverAsync(context, firstStepDef);

        instance.Steps.Add(new WorkflowInstanceStep(
            id: GuidGenerator.Create(),
            workflowInstanceId: instance.Id,
            workflowStepDefinitionId: firstStepDef.Id,
            assignedUserId: firstApprover
        ));

        return instance;
    }

    // Onay/red kararını işler. Onaylandıysa sonraki adıma yönlendirir, reddedildiyse iş akışını sonlandırır.
    public async Task<WorkflowInstanceStep> ProcessApprovalAsync(ProcessApprovalModel model)
    {
        var step = await FindAndValidateStepAsync(model.InstanceStepId);
        var instance = step.WorkflowInstance;

        ValidateWorkflowIsActive(instance);
        ValidateStepIsPending(step);
        ValidateApprovalAuthorization(step, model.CurrentUserId, model.CurrentUserRoles);

        step.ActionTaken = model.IsApproved ? WorkflowActionType.Approved : WorkflowActionType.Rejected;
        step.Note = model.Note;
        step.ActionDate = Clock.Now;

        await _workflowInstanceStepRepository.UpdateAsync(step, autoSave: true);

        if (!model.IsApproved)
        {
            await TerminateWorkflowAsync(instance, WorkflowState.Rejected);
        }
        else
        {
            await RouteToNextStepAsync(step, instance);
        }

        return step;
    }

    // Sıradaki adım tanımını bulur, onaycısını çözer ve adımı oluşturur.
    // Sıradaki adım yoksa iş akışını Completed olarak sonlandırır.
    private async Task RouteToNextStepAsync(WorkflowInstanceStep processedStep, WorkflowInstance instance)
    {
        var definition = await FindAndValidateDefinitionAsync(instance.WorkflowDefinitionId);

        var currentOrder = processedStep.WorkflowStepDefinition.StepOrder;
        var nextStepDef = definition.Steps
            .OrderBy(x => x.StepOrder)
            .FirstOrDefault(x => x.StepOrder > currentOrder);

        if (nextStepDef != null)
        {
            var context = new ApproverContext(instance.EntityType, instance.EntityId, instance.InitiatorUserId);
            var nextApprover = await _workflowApproverResolver.ResolveApproverAsync(context, nextStepDef);

            var nextStep = new WorkflowInstanceStep(
                id: GuidGenerator.Create(),
                workflowInstanceId: instance.Id,
                workflowStepDefinitionId: nextStepDef.Id,
                assignedUserId: nextApprover
            );

            await _workflowInstanceStepRepository.InsertAsync(nextStep, autoSave: true);
            await PublishStepAssignedAsync(instance, nextStep);
        }
        else
        {
            await TerminateWorkflowAsync(instance, WorkflowState.Completed);
        }
    }

    private async Task<WorkflowDefinition> FindAndValidateDefinitionAsync(Guid definitionId)
    {
        var query = await _workflowDefinitionRepository.WithDetailsAsync(x => x.Steps);
        var definition = query.FirstOrDefault(x => x.Id == definitionId);

        if (definition == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.DefinitionNotFound)
                .WithData("DefinitionId", definitionId);
        }

        if (!definition.IsActive)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }

        if (!definition.Steps.Any())
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }

        return definition;
    }

    private async Task<WorkflowInstanceStep> FindAndValidateStepAsync(Guid stepId)
    {
        var query = await _workflowInstanceStepRepository.WithDetailsAsync(
            x => x.WorkflowInstance,
            x => x.WorkflowStepDefinition);

        var step = query.FirstOrDefault(x => x.Id == stepId);

        if (step == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.StepNotFound);
        }

        return step;
    }

    private void ValidateWorkflowIsActive(WorkflowInstance instance)
    {
        if (instance.State != WorkflowState.Active)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.InstanceNotActive);
    }

    private void ValidateStepIsPending(WorkflowInstanceStep step)
    {
        if (step.ActionTaken != WorkflowActionType.Pending)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.AlreadyDecided);
    }

    // Mevcut kullanıcının bu adımı onaylama yetkisinin olup olmadığını doğrular.
    // Öncelik: (1) atanmış kullanıcı eşleşmesi, (2) gerekli rol eşleşmesi, (3) ikisi de yoksa serbest.
    private void ValidateApprovalAuthorization(WorkflowInstanceStep step, Guid currentUserId, List<string> currentUserRoles)
    {
        if (step.AssignedUserId.HasValue)
        {
            if (step.AssignedUserId.Value != currentUserId)
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.UnauthorizedApproval);
        }
        else
        {
            var requiredRole = step.WorkflowStepDefinition.RequiredRoleName;
            if (!string.IsNullOrEmpty(requiredRole) && !currentUserRoles.Contains(requiredRole))
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.UnauthorizedApproval);
            
        }
    }

    private async Task TerminateWorkflowAsync(WorkflowInstance instance, WorkflowState finalState)
    {
        instance.State = finalState;
        await _workflowInstanceRepository.UpdateAsync(instance, autoSave: true);
        await _localEventBus.PublishAsync(new WorkflowCompletedEto
        {
            WorkflowInstanceId = instance.Id,
            EntityType = instance.EntityType,
            EntityId = instance.EntityId,
            FinalState = finalState
        });
    }

    private Task PublishStepAssignedAsync(WorkflowInstance instance, WorkflowInstanceStep step)
    {
        return _localEventBus.PublishAsync(new WorkflowStepAssignedEto
        {
            WorkflowInstanceId = instance.Id,
            WorkflowInstanceStepId = step.Id,
            WorkflowStepDefinitionId = step.WorkflowStepDefinitionId,
            EntityType = instance.EntityType,
            EntityId = instance.EntityId,
            AssignedUserId = step.AssignedUserId
        });
    }
}
