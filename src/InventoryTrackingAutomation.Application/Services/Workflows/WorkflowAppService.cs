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

namespace InventoryTrackingAutomation.Services.Workflows;

/// <summary>
/// Dinamik iş akışlarını dış dünyaya açan uygulama servisinin implementasyonu.
/// </summary>
public class WorkflowAppService : InventoryTrackingAutomationAppService, IWorkflowAppService
{
    private readonly WorkflowManager _workflowManager;
    private readonly IWorkerRepository _workerRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;

    public WorkflowAppService(
        WorkflowManager workflowManager,
        IWorkerRepository workerRepository,
        IWorkflowInstanceRepository workflowInstanceRepository,
        IWorkflowInstanceStepRepository workflowInstanceStepRepository)
    {
        _workflowManager = workflowManager;
        _workerRepository = workerRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
        _workflowInstanceStepRepository = workflowInstanceStepRepository;
    }

    /// <summary>
    /// Yeni bir iş akışı süreci başlatır.
    /// </summary>
    [UnitOfWork]
    public async Task<WorkflowInstanceDto> StartAsync(StartWorkflowDto input)
    {
        var currentUserId = CurrentUser.Id;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("Bu işlemi yapmak için giriş yapmalısınız.");
        }

        Guid? managerId = null;
        var worker = await _workerRepository.FindAsync(w => w.UserId == currentUserId.Value);

        if (worker != null)
        {
            if (worker.ManagerId.HasValue)
            {
                var managerWorker = await _workerRepository.FindAsync(w => w.Id == worker.ManagerId.Value);
                if (managerWorker != null && managerWorker.UserId != Guid.Empty)
                {
                    managerId = managerWorker.UserId;
                }
            }
        }

        var model = ObjectMapper.Map<StartWorkflowDto, StartWorkflowModel>(input);
        model.InitiatorUserId = currentUserId.Value;
        model.InitiatorsManagerUserId = managerId;

        var entity = await _workflowManager.StartWorkflowAsync(model);

        var inserted = await _workflowInstanceRepository.InsertAsync(entity, autoSave: true);

        return ObjectMapper.Map<WorkflowInstance, WorkflowInstanceDto>(inserted);
    }

    /// <summary>
    /// Belirtilen iş akışı adımında onay veya ret aksiyonunu işler.
    /// </summary>
    [UnitOfWork]
    public async Task<WorkflowInstanceStepDto> ProcessApprovalAsync(ProcessApprovalDto input)
    {
        var currentUserId = CurrentUser.Id;
        if (currentUserId == null)
        {
            throw new UnauthorizedAccessException("Bu işlemi yapmak için giriş yapmalısınız.");
        }

        var roles = CurrentUser.Roles != null ? CurrentUser.Roles.ToList() : new List<string>();

        var model = ObjectMapper.Map<ProcessApprovalDto, ProcessApprovalModel>(input);
        model.CurrentUserId = currentUserId.Value;
        model.CurrentUserRoles = roles;

        var entityStep = await _workflowManager.ProcessApprovalAsync(model);

        var updated = await _workflowInstanceStepRepository.UpdateAsync(entityStep, autoSave: true);

        return ObjectMapper.Map<WorkflowInstanceStep, WorkflowInstanceStepDto>(updated);
    }
}
