using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Models.Movements;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace InventoryTrackingAutomation.Managers.Movements;

// Hareket talebi onay süreci domain manager'ı — onay/red iş kuralları, validasyon ve state machine.
public class MovementApprovalManager : DomainService
{
    // MovementRequest okuma/yazma için repository.
    private readonly IRepository<MovementRequest, Guid> _movementRequestRepository;
    // Onay/red kararlarını saklayan repository.
    private readonly IRepository<MovementApproval, Guid> _movementApprovalRepository;
    // İş akışı instance adımları repository'si.
    private readonly IRepository<WorkflowInstanceStep, Guid> _workflowInstanceStepRepository;
    // İş akışı instance'ları repository'si.
    private readonly IRepository<WorkflowInstance, Guid> _workflowInstanceRepository;
    // Adım sırası (StepOrder) gibi tanım bilgileri için.
    private readonly IRepository<WorkflowStepDefinition, Guid> _workflowStepDefinitionRepository;
    // Onaylayan kullanıcının Worker kaydını çözmek için.
    private readonly IRepository<Worker, Guid> _workerRepository;
    // Pending listesinde site adlarını çözmek için.
    private readonly IRepository<Site, Guid> _siteRepository;
    // Rol bazlı yetki kontrolü için.
    private readonly IdentityUserManager _identityUserManager;
    // Yeni MovementApproval kayıtlarına Id atamak için.
    private readonly IGuidGenerator _guidGenerator;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public MovementApprovalManager(
        IRepository<MovementRequest, Guid> movementRequestRepository,
        IRepository<MovementApproval, Guid> movementApprovalRepository,
        IRepository<WorkflowInstanceStep, Guid> workflowInstanceStepRepository,
        IRepository<WorkflowInstance, Guid> workflowInstanceRepository,
        IRepository<WorkflowStepDefinition, Guid> workflowStepDefinitionRepository,
        IRepository<Worker, Guid> workerRepository,
        IRepository<Site, Guid> siteRepository,
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator,
        IMapper mapper)
    {
        _mapper = mapper;
        _movementRequestRepository = movementRequestRepository;
        _movementApprovalRepository = movementApprovalRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowStepDefinitionRepository = workflowStepDefinitionRepository;
        _workerRepository = workerRepository;
        _siteRepository = siteRepository;
        _identityUserManager = identityUserManager;
        _guidGenerator = guidGenerator;
    }

    // Hareket talebini onaylar — yetki kontrolü yapar, approval kaydı oluşturur, sonraki adıma routing.
    public async Task<MovementApproval> ApproveAsync(Guid movementRequestId, Guid approvingUserId, string comment)
    {
        // Hareket talebini doğrula (var, InReview durumunda)
        var movementRequest = await ValidateMovementRequestAsync(movementRequestId);
        // Workflow instance'ı doğrula (var, Active)
        var workflowInstance = await ValidateWorkflowInstanceAsync(movementRequest);
        // Bekleyen step'i bul
        var currentStep = await ValidatePendingStepExistsAsync(workflowInstance.Id);
        // Kullanıcının onay yetkisi var mı kontrol et
        await ValidateApprovalAuthorizationAsync(currentStep, approvingUserId);

        // Approval kaydı oluştur
        var approval = await CreateApprovalRecordAsync(
            movementRequestId, approvingUserId, currentStep, ApprovalStatusEnum.Approved, comment);

        // Step'i Approved olarak işaretle
        await MarkStepAsDecidedAsync(currentStep, WorkflowActionType.Approved);

        // Tüm step'ler tamamlandıysa workflow ve talep durumunu güncelle
        await TryCompleteWorkflowAsync(workflowInstance, movementRequest);

        return approval;
    }

    // Hareket talebini reddeder — workflow anında sonlanır, talep Rejected olur.
    public async Task<MovementApproval> RejectAsync(Guid movementRequestId, Guid approvingUserId, string reason)
    {
        // Hareket talebini doğrula
        var movementRequest = await ValidateMovementRequestAsync(movementRequestId);
        // Workflow instance'ı doğrula
        var workflowInstance = await ValidateWorkflowInstanceAsync(movementRequest);
        // Bekleyen step'i bul
        var currentStep = await ValidatePendingStepExistsAsync(workflowInstance.Id);
        // Yetki kontrolü
        await ValidateApprovalAuthorizationAsync(currentStep, approvingUserId);

        // Rejection kaydı oluştur
        var approval = await CreateApprovalRecordAsync(
            movementRequestId, approvingUserId, currentStep, ApprovalStatusEnum.Rejected, reason);

        // Step'i Rejected olarak işaretle
        await MarkStepAsDecidedAsync(currentStep, WorkflowActionType.Rejected);

        // Workflow ve talebi anında sonlandır
        await TerminateWorkflowAsync(workflowInstance, movementRequest);

        return approval;
    }

    // Verilen kullanıcının onaylaması gereken bekleyen talepleri liste olarak döner.
    public async Task<List<PendingApprovalModel>> GetPendingApprovalsForUserAsync(Guid userId)
    {
        // Kullanıcıya atanmış ve henüz karar verilmemiş step'leri bul
        var pendingSteps = await _workflowInstanceStepRepository.GetListAsync(
            x => x.AssignedUserId == userId && x.ActionTaken == WorkflowActionType.Pending);

        var result = new List<PendingApprovalModel>();

        // Her bekleyen step için MovementRequest ve Site bilgilerini topla
        foreach (var step in pendingSteps)
        {
            var model = await BuildPendingApprovalModelAsync(step);
            if (model != null)
                result.Add(model);
        }

        return result;
    }

    // Bir bekleyen step için PendingApprovalModel oluşturur — MovementRequest ve Site bilgilerini join'ler.
    private async Task<PendingApprovalModel> BuildPendingApprovalModelAsync(WorkflowInstanceStep step)
    {
        // Step'e bağlı MovementRequest'i bul (workflow üzerinden)
        var movementRequest = await _movementRequestRepository.FirstOrDefaultAsync(
            x => x.WorkflowInstanceId == step.WorkflowInstanceId);
        if (movementRequest == null)
            return null;

        // Step definition'dan adım sırası bilgisini al
        var stepDefinition = await _workflowStepDefinitionRepository.FindAsync(step.WorkflowStepDefinitionId);

        // Kaynak ve hedef lokasyon adlarını çöz
        var sourceSite = await _siteRepository.FindAsync(movementRequest.SourceSiteId);
        var targetSite = await _siteRepository.FindAsync(movementRequest.TargetSiteId);

        return new PendingApprovalModel
        {
            MovementRequestId = movementRequest.Id,
            WorkflowInstanceStepId = step.Id,
            RequestNumber = movementRequest.RequestNumber,
            SourceSiteName = sourceSite?.Name ?? string.Empty,
            TargetSiteName = targetSite?.Name ?? string.Empty,
            CurrentStepOrder = stepDefinition?.StepOrder ?? 0,
            CurrentStepName = stepDefinition?.RequiredRoleName ?? stepDefinition?.ResolverKey ?? string.Empty,
            CreatedAt = step.CreationTime,
            PlannedDate = movementRequest.PlannedDate,
            RequestNote = movementRequest.RequestNote,
            Priority = movementRequest.Priority
        };
    }

    // Onay/red kaydı oluşturur ve veritabanına ekler.
    private async Task<MovementApproval> CreateApprovalRecordAsync(
        Guid movementRequestId,
        Guid approvingUserId,
        WorkflowInstanceStep currentStep,
        ApprovalStatusEnum status,
        string note)
    {
        // Onaylayan kullanıcının Worker kaydını bul
        var approverWorker = await _workerRepository.FirstOrDefaultAsync(x => x.UserId == approvingUserId);

        // Step definition'ı manuel yükle — StepOrder için
        var stepDefinition = await _workflowStepDefinitionRepository.GetAsync(currentStep.WorkflowStepDefinitionId);

        var approval = new MovementApproval(_guidGenerator.Create())
        {
            MovementRequestId = movementRequestId,
            ApproverWorkerId = approverWorker?.Id ?? Guid.Empty,
            StepOrder = stepDefinition.StepOrder,
            Status = status,
            DecidedAt = DateTime.UtcNow,
            Note = note
        };

        await _movementApprovalRepository.InsertAsync(approval, autoSave: true);
        return approval;
    }

    // Step'i Approved/Rejected olarak işaretler ve kaydeder.
    private async Task MarkStepAsDecidedAsync(WorkflowInstanceStep step, WorkflowActionType action)
    {
        step.ActionTaken = action;
        step.ActionDate = DateTime.UtcNow;
        await _workflowInstanceStepRepository.UpdateAsync(step, autoSave: true);
    }

    // Tüm step'ler tamamlandıysa workflow ve talebi Approved olarak işaretler.
    private async Task TryCompleteWorkflowAsync(WorkflowInstance workflowInstance, MovementRequest movementRequest)
    {
        // Hâlâ bekleyen step var mı?
        var remainingPendingSteps = await _workflowInstanceStepRepository.GetListAsync(
            x => x.WorkflowInstanceId == workflowInstance.Id && x.ActionTaken == WorkflowActionType.Pending);

        if (remainingPendingSteps.Any())
            return;

        // Tüm step'ler tamamlandı: workflow Completed, talep Approved
        workflowInstance.State = WorkflowState.Completed;
        movementRequest.Status = MovementStatusEnum.Approved;
        await _workflowInstanceRepository.UpdateAsync(workflowInstance, autoSave: true);
        await _movementRequestRepository.UpdateAsync(movementRequest, autoSave: true);
    }

    // Workflow'u Rejected olarak sonlandırır ve talebi reddeder.
    private async Task TerminateWorkflowAsync(WorkflowInstance workflowInstance, MovementRequest movementRequest)
    {
        workflowInstance.State = WorkflowState.Rejected;
        movementRequest.Status = MovementStatusEnum.Rejected;
        await _workflowInstanceRepository.UpdateAsync(workflowInstance, autoSave: true);
        await _movementRequestRepository.UpdateAsync(movementRequest, autoSave: true);
    }

    // Onaylayan kullanıcının yetki kontrolü: assigned user, required role veya manager onayı.
    private async Task ValidateApprovalAuthorizationAsync(WorkflowInstanceStep step, Guid approvingUserId)
    {
        // Step'e atanmış kullanıcı varsa sadece o onaylayabilir
        if (step.AssignedUserId.HasValue && step.AssignedUserId != approvingUserId)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.UnauthorizedApprover);

        // Step definition'ı manuel yükle — navigation property eager load yok.
        var stepDefinition = await _workflowStepDefinitionRepository.GetAsync(step.WorkflowStepDefinitionId);

        // Step'te required role tanımlanmışsa kullanıcının o role sahip olduğunu doğrula
        if (!string.IsNullOrEmpty(stepDefinition.RequiredRoleName))
        {
            var user = await _identityUserManager.GetByIdAsync(approvingUserId);
            var roles = await _identityUserManager.GetRolesAsync(user);
            if (!roles.Contains(stepDefinition.RequiredRoleName))
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.UnauthorizedApprover);
        }
    }

    // Hareket talebinin var ve InReview durumunda olduğunu doğrular.
    private async Task<MovementRequest> ValidateMovementRequestAsync(Guid movementRequestId)
    {
        var movementRequest = await _movementRequestRepository.GetAsync(movementRequestId);
        if (movementRequest == null)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementRequests.NotFound);

        if (movementRequest.Status != MovementStatusEnum.InReview)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.InvalidMovementStatus);

        return movementRequest;
    }

    // Workflow instance'ın var ve Active durumunda olduğunu doğrular.
    private async Task<WorkflowInstance> ValidateWorkflowInstanceAsync(MovementRequest movementRequest)
    {
        if (!movementRequest.WorkflowInstanceId.HasValue)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.WorkflowNotFound);

        var workflowInstance = await _workflowInstanceRepository.GetAsync(movementRequest.WorkflowInstanceId.Value);
        if (workflowInstance == null || workflowInstance.State != WorkflowState.Active)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.WorkflowNotFound);

        return workflowInstance;
    }

    // Bekleyen step'i bulur — sıraya göre ilkini döner, yoksa hata fırlatır.
    private async Task<WorkflowInstanceStep> ValidatePendingStepExistsAsync(Guid workflowInstanceId)
    {
        var pendingSteps = await _workflowInstanceStepRepository.GetListAsync(
            x => x.WorkflowInstanceId == workflowInstanceId && x.ActionTaken == WorkflowActionType.Pending);

        if (!pendingSteps.Any())
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.MovementApprovals.NoPendingApprovalStep);

        return pendingSteps.OrderBy(x => x.WorkflowStepDefinition.StepOrder).First();
    }
}
