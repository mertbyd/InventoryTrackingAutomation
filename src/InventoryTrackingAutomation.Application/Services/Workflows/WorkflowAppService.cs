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
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
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
    private readonly IRepository<WorkflowDefinition, Guid> _workflowDefinitionRepository;
    private readonly IIdentityUserRepository _identityUserRepository;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public WorkflowAppService(
        WorkflowManager workflowManager,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowInstanceStepRepository workflowInstanceStepRepository,
        IRepository<WorkflowStepDefinition, Guid> stepDefinitionRepository,
        IRepository<WorkflowDefinition, Guid> workflowDefinitionRepository,
        IIdentityUserRepository identityUserRepository,
        IMapper mapper)
    {
        _mapper = mapper;
        _workflowManager = workflowManager;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
        _stepDefinitionRepository = stepDefinitionRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _identityUserRepository = identityUserRepository;
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

    // Belirtilen workflow instance içindeki mevcut bekleyen adımı bulur ve generic onay akışına yollar.
    [UnitOfWork]
    public async Task<WorkflowInstanceStepDto> ProcessInstanceApprovalAsync(
        Guid instanceId,
        ProcessWorkflowInstanceApprovalDto input)
    {
        var instance = await _workflowInstanceRepository.FindAsync(instanceId);
        if (instance == null)
        {
            throw new EntityNotFoundException(typeof(WorkflowInstance), instanceId);
        }

        var pendingSteps = await _workflowInstanceStepRepository.GetListAsync(
            x => x.WorkflowInstanceId == instanceId &&
                 x.ActionTaken == InventoryTrackingAutomation.Enums.Workflows.WorkflowActionType.Pending);

        if (!pendingSteps.Any())
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Workflows.StepNotFound)
                .WithData("WorkflowInstanceId", instanceId);
        }

        var orderedPendingSteps = new List<(WorkflowInstanceStep Step, int StepOrder)>();
        foreach (var pendingStep in pendingSteps)
        {
            var stepDefinition = await _stepDefinitionRepository.GetAsync(pendingStep.WorkflowStepDefinitionId);
            orderedPendingSteps.Add((pendingStep, stepDefinition.StepOrder));
        }

        var currentStep = orderedPendingSteps.OrderBy(x => x.StepOrder).First().Step;

        return await ProcessApprovalAsync(new ProcessApprovalDto
        {
            InstanceStepId = currentStep.Id,
            IsApproved = input.IsApproved,
            Note = input.Note
        });
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

    // Bir iş akışı süreci için tam tarihçeyi döner — kim başlattı, geçmiş/mevcut/gelecek adımlar tek yapıda.
    public async Task<WorkflowHistoryDto> GetHistoryAsync(Guid instanceId)
    {
        var instance = await _workflowInstanceRepository.FindAsync(instanceId);
        if (instance == null)
        {
            throw new EntityNotFoundException(typeof(WorkflowInstance), instanceId);
        }

        var definition = await _workflowDefinitionRepository.FindAsync(instance.WorkflowDefinitionId);
        var stepDefinitions = (await _stepDefinitionRepository.GetListAsync(
                x => x.WorkflowDefinitionId == instance.WorkflowDefinitionId))
            .OrderBy(x => x.StepOrder)
            .ToList();

        var instanceSteps = (await _workflowInstanceStepRepository.GetListAsync(
            x => x.WorkflowInstanceId == instanceId)).ToList();
        var instanceStepByDefId = instanceSteps.ToDictionary(x => x.WorkflowStepDefinitionId);

        // User'ları batch çek — initiator + tüm AssignedUserId'ler.
        var userIds = new HashSet<Guid> { instance.InitiatorUserId };
        foreach (var step in instanceSteps)
        {
            if (step.AssignedUserId.HasValue)
            {
                userIds.Add(step.AssignedUserId.Value);
            }
        }

        var users = await _identityUserRepository.GetListAsync();
        var userMap = users
            .Where(user => userIds.Contains(user.Id))
            .ToDictionary(user => user.Id);

        string? GetUserName(Guid id) => userMap.TryGetValue(id, out var user) ? user.UserName : null;
        string? GetFullName(Guid id) => userMap.TryGetValue(id, out var user) ? user.Name : null;

        var dto = new WorkflowHistoryDto
        {
            WorkflowInstanceId = instance.Id,
            WorkflowDefinitionName = definition?.Name ?? string.Empty,
            WorkflowDescription = definition?.Description,
            EntityType = instance.EntityType,
            EntityId = instance.EntityId,
            State = instance.State.ToString(),
            InitiatorUserId = instance.InitiatorUserId,
            InitiatorUserName = GetUserName(instance.InitiatorUserId),
            InitiatorFullName = GetFullName(instance.InitiatorUserId),
            CreatedDate = instance.CreationTime,
            Steps = new List<WorkflowHistoryStepDto>()
        };

        foreach (var stepDefinition in stepDefinitions)
        {
            var stepDto = new WorkflowHistoryStepDto
            {
                StepOrder = stepDefinition.StepOrder,
                StepName = !string.IsNullOrEmpty(stepDefinition.RequiredRoleName)
                    ? stepDefinition.RequiredRoleName
                    : (stepDefinition.ResolverKey ?? string.Empty),
                RequiredRoleName = stepDefinition.RequiredRoleName,
                ResolverKey = stepDefinition.ResolverKey
            };

            if (instanceStepByDefId.TryGetValue(stepDefinition.Id, out var instanceStep))
            {
                stepDto.StepStatus = instanceStep.ActionTaken.ToString();
                stepDto.CreatedDate = instanceStep.CreationTime;

                if (instanceStep.AssignedUserId.HasValue)
                {
                    var userId = instanceStep.AssignedUserId.Value;
                    stepDto.Approvers.Add(new WorkflowHistoryApproverDto
                    {
                        UserId = userId,
                        UserName = GetUserName(userId),
                        FullName = GetFullName(userId),
                        ActionTaken = instanceStep.ActionTaken.ToString(),
                        Note = instanceStep.Note,
                        ActionDate = instanceStep.ActionDate
                    });
                }
            }
            else
            {
                stepDto.StepStatus = "NotStarted";
                stepDto.CreatedDate = null;
            }

            dto.Steps.Add(stepDto);
        }

        return dto;
    }
}
