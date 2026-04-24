using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Events.Workflows;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Models.Workflows;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.EventBus.Local;

namespace InventoryTrackingAutomation.Managers.Workflows;

/// <summary>
/// Dinamik iş akışlarını başlatan ve onay süreçlerini (State Machine) yöneten Domain Service.
/// Side-effect barındırmaz — sadece durum günceller ve event fırlatır.
/// </summary>
public class WorkflowManager : DomainService
{
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;
    private readonly ILocalEventBus _localEventBus;
    private readonly IWorkflowApproverResolver _workflowApproverResolver;
    private readonly IWorkerRepository _workerRepository;

    public WorkflowManager(
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowInstanceStepRepository workflowInstanceStepRepository,
        ILocalEventBus localEventBus,
        IWorkflowApproverResolver workflowApproverResolver,
        IWorkerRepository workerRepository)
    {
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
        _localEventBus = localEventBus;
        _workflowApproverResolver = workflowApproverResolver;
        _workerRepository = workerRepository;
    }

    /// <summary>
    /// Yeni bir iş akışı süreci başlatır. Sadece birinci adımı oluşturur;
    /// sonraki adımlar her onay sonrasında dinamik olarak (Routing) üretilir.
    /// Entity'leri döndürür, kaydetme işi AppService'e aittir.
    /// </summary>
    public async Task<WorkflowInstance> StartWorkflowAsync(StartWorkflowModel model)
    {
        var defQuery = await _workflowDefinitionRepository.WithDetailsAsync(x => x.Steps);
        var definition = defQuery.FirstOrDefault(x => x.Id == model.WorkflowDefinitionId);

        if (definition == null)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.DefinitionNotFound)
                .WithData("DefinitionId", model.WorkflowDefinitionId);
        }

        if (!definition.IsActive)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.General.InvalidOperation)
                .WithData("Message", "İş akışı tanımı aktif değil.");
        }

        if (!definition.Steps.Any())
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.General.InvalidOperation)
                .WithData("Message", "İş akışı tanımında hiç adım bulunmuyor.");
        }

        var instance = new WorkflowInstance(
            id: GuidGenerator.Create(),
            workflowDefinitionId: definition.Id,
            entityType: model.EntityType,
            entityId: model.EntityId,
            state: WorkflowState.Active,
            initiatorUserId: model.InitiatorUserId
        );

        // Yalnızca birinci adımı oluştur — devamı her onaydan sonra routing ile üretilir.
        var firstStepDef = definition.Steps.OrderBy(x => x.StepOrder).First();
        var assignedUserId = await ResolveApproverAsync(
            firstStepDef,
            model.EntityType,
            model.EntityId,
            model.InitiatorsManagerUserId);

        instance.Steps.Add(new WorkflowInstanceStep(
            id: GuidGenerator.Create(),
            workflowInstanceId: instance.Id,
            workflowStepDefinitionId: firstStepDef.Id,
            assignedUserId: assignedUserId
        ));

        return instance;
    }

    /// <summary>
    /// Bir iş akışı adımında onay veya ret kararını işler; Routing (Yönlendirme) Zekasını çalıştırır.
    /// Onay varsa sonraki adımı dinamik olarak oluşturur; son adımsa süreci tamamlar.
    /// Entity'leri günceller / insert'e kuyruğa alır, kaydetme işi AppService'e aittir.
    /// </summary>
    public async Task<WorkflowInstanceStep> ProcessApprovalAsync(ProcessApprovalModel model)
    {
        // --- 1. VERİ YÜKLEME ---
        var stepQuery = await _workflowInstanceStepRepository.WithDetailsAsync(
            x => x.WorkflowInstance,
            x => x.WorkflowStepDefinition);

        var step = stepQuery.FirstOrDefault(x => x.Id == model.InstanceStepId);

        if (step == null)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.StepNotFound)
                .WithData("InstanceStepId", model.InstanceStepId);
        }

        var instance = step.WorkflowInstance;

        // --- 2. DURUM KONTROLÜ ---
        if (instance.State != WorkflowState.Active)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.InstanceNotActive)
                .WithData("State", instance.State.ToString());
        }

        if (step.ActionTaken != WorkflowActionType.Pending)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.MovementApprovals.AlreadyDecided);
        }

        // --- 3. YETKİ KONTROLÜ ---
        if (step.AssignedUserId.HasValue)
        {
            // Spesifik kullanıcı atanmışsa yalnızca o onaylayabilir.
            if (step.AssignedUserId.Value != model.CurrentUserId)
            {
                throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.UnauthorizedApproval);
            }
        }
        else
        {
            // Kullanıcı atanmamışsa rol bazlı kontrol yapılır.
            var requiredRole = step.WorkflowStepDefinition.RequiredRoleName;
            if (!string.IsNullOrEmpty(requiredRole) &&
                !model.CurrentUserRoles.Contains(requiredRole))
            {
                throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.UnauthorizedApproval);
            }
        }

        // --- 4. KARAR UYGULAMA ---
        step.ActionTaken = model.IsApproved ? WorkflowActionType.Approved : WorkflowActionType.Rejected;
        step.Note = model.Note;
        step.ActionDate = Clock.Now;

        // --- 5. DURUM MAKİNESİ (STATE MACHINE) ---
        if (!model.IsApproved)
        {
            // Herhangi bir adım reddedilirse tüm süreç anında sonlanır.
            instance.State = WorkflowState.Rejected;

            await _localEventBus.PublishAsync(new WorkflowCompletedEto
            {
                WorkflowInstanceId = instance.Id,
                EntityType = instance.EntityType,
                EntityId = instance.EntityId,
                FinalState = WorkflowState.Rejected
            });
        }
        else
        {
            // Onay durumunda Routing Zekası devreye girer.
            await RouteToNextStepAsync(step, instance);
        }

        return step;
    }

    /// <summary>
    /// Onaylanan adımdan sonra şablona bakarak sonraki adımı oluşturur
    /// veya sürecin tamamlandığını ilan eder.
    /// </summary>
    private async Task RouteToNextStepAsync(WorkflowInstanceStep processedStep, WorkflowInstance instance)
    {
        var defQuery = await _workflowDefinitionRepository.WithDetailsAsync(x => x.Steps);
        var definition = defQuery.FirstOrDefault(x => x.Id == instance.WorkflowDefinitionId);

        if (definition == null)
        {
            throw new BusinessException(InventoryTrackingAutomationDomainErrorCodes.Workflows.DefinitionNotFound)
                .WithData("DefinitionId", instance.WorkflowDefinitionId);
        }

        var currentOrder = processedStep.WorkflowStepDefinition.StepOrder;

        // Şablondaki bir sonraki adım tanımı (StepOrder > mevcut, küçükten büyüğe sıralı ilk)
        var nextStepDef = definition.Steps
            .OrderBy(x => x.StepOrder)
            .FirstOrDefault(x => x.StepOrder > currentOrder);

        if (nextStepDef != null)
        {
            // --- Sonraki adım var: Onaycıyı çöz ve yeni WorkflowInstanceStep oluştur ---
            Guid? managerUserId = null;
            if (nextStepDef.IsManagerApprovalRequired)
            {
                // "Talebi başlatan kişinin yöneticisi" — instance'taki InitiatorUserId üzerinden çözülür.
                managerUserId = await FindManagerUserIdAsync(instance.InitiatorUserId);
            }

            var assignedUserId = await ResolveApproverAsync(
                nextStepDef,
                instance.EntityType,
                instance.EntityId,
                managerUserId);

            var nextStep = new WorkflowInstanceStep(
                id: GuidGenerator.Create(),
                workflowInstanceId: instance.Id,
                workflowStepDefinitionId: nextStepDef.Id,
                assignedUserId: assignedUserId
            );

            // Routing'in parçası olduğundan Manager doğrudan kuyruklar; UnitOfWork commit eder.
            await _workflowInstanceStepRepository.InsertAsync(nextStep);
        }
        else
        {
            // --- Son adım onaylandı: Süreci tamamla ve event fırlat ---
            instance.State = WorkflowState.Completed;

            await _localEventBus.PublishAsync(new WorkflowCompletedEto
            {
                WorkflowInstanceId = instance.Id,
                EntityType = instance.EntityType,
                EntityId = instance.EntityId,
                FinalState = WorkflowState.Completed
            });
        }
    }

    /// <summary>
    /// Bir adım tanımı için onaycı kullanıcı Id'sini çözer.
    /// Öncelik sırası: IsManagerApprovalRequired → ResolverKey → null (rol bazlı).
    /// </summary>
    private async Task<Guid?> ResolveApproverAsync(
        WorkflowStepDefinition stepDef,
        string entityType,
        Guid entityId,
        Guid? managerUserId)
    {
        if (stepDef.IsManagerApprovalRequired)
        {
            return managerUserId;
        }

        if (!string.IsNullOrEmpty(stepDef.ResolverKey))
        {
            return await _workflowApproverResolver.ResolveApproverAsync(entityType, entityId, stepDef);
        }

        return null; // Atanmış kullanıcı yok; rol bazlı yetki ProcessApprovalAsync'te kontrol edilir.
    }

    /// <summary>
    /// Verilen UserId'ye sahip Worker'ın yöneticisinin (ManagerId) UserId'sini döner.
    /// </summary>
    private async Task<Guid?> FindManagerUserIdAsync(Guid workerUserId)
    {
        var worker = await _workerRepository.FindAsync(w => w.UserId == workerUserId);

        if (worker?.ManagerId == null)
        {
            return null;
        }

        var manager = await _workerRepository.FindAsync(worker.ManagerId.Value);
        return manager?.UserId;
    }
}
