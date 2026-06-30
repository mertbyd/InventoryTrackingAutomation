using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Enums.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers.Workflows;
using InventoryTrackingAutomation.Models.Workflows;
using InventoryTrackingAutomation.Workflows;
using Shouldly;
using Volo.Abp.Modularity;
using Xunit;

namespace InventoryTrackingAutomation.Workflows;

public abstract class WorkflowManager_Tests<TStartupModule> : InventoryTrackingAutomationDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly WorkflowManager _workflowManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;
    private readonly IWorkflowInstanceStepRepository _workflowInstanceStepRepository;

    protected WorkflowManager_Tests()
    {
        _workflowManager = GetRequiredService<WorkflowManager>();
        _workflowDefinitionRepository = GetRequiredService<IWorkflowDefinitionRepository>();
        _workflowInstanceRepository = GetRequiredService<IWorkflowInstanceRepository>();
        _workflowInstanceStepRepository = GetRequiredService<IWorkflowInstanceStepRepository>();
    }

    [Fact]
    public async Task ProcessApproval_Should_Terminate_Workflow_When_Rejected()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var definitionId = Guid.NewGuid();
            var stepDefId = Guid.NewGuid();
            var currentUserId = Guid.NewGuid();

            var definition = new WorkflowDefinition(
                definitionId,
                name: "Test Workflow",
                description: "Test workflow for rejection transition",
                isActive: true);

            definition.Steps.Add(new WorkflowStepDefinition(
                stepDefId,
                workflowDefinitionId: definitionId,
                stepOrder: 1,
                requiredRoleName: null));
            await _workflowDefinitionRepository.InsertAsync(definition, autoSave: true);

            var instance = await _workflowManager.StartWorkflowAsync(new StartWorkflowModel
            {
                WorkflowDefinitionId = definitionId,
                EntityId = Guid.NewGuid(),
                EntityType = "TestEntity",
                InitiatorUserId = Guid.NewGuid()
            });
            await _workflowInstanceRepository.InsertAsync(instance, autoSave: true);

            var step = instance.Steps.First();

            // Act
            await _workflowManager.ProcessApprovalAsync(new ProcessApprovalModel
            {
                InstanceStepId = step.Id,
                IsApproved = false,
                Note = "Test reject",
                CurrentUserId = currentUserId,
                CurrentUserRoles = new List<string>()
            });

            // Assert
            var updatedInstance = await _workflowInstanceRepository.GetAsync(instance.Id);
            var updatedStep = await _workflowInstanceStepRepository.GetAsync(step.Id);

            updatedInstance.State.ShouldBe(WorkflowState.Rejected);
            updatedStep.ActionTaken.ShouldBe(WorkflowActionType.Rejected);
            updatedStep.Note.ShouldBe("Test reject");
        });
    }

    [Fact]
    public async Task ProcessApproval_Should_Move_To_Next_Step_When_Approved()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            // Arrange
            var definitionId = Guid.NewGuid();
            var step1DefId = Guid.NewGuid();
            var step2DefId = Guid.NewGuid();
            var initiatorId = Guid.NewGuid();
            var entityId = Guid.NewGuid();

            var definition = new WorkflowDefinition(
                definitionId,
                name: "Test Success Workflow",
                description: "Success path test",
                isActive: true);

            definition.Steps.Add(new WorkflowStepDefinition(
                step1DefId,
                workflowDefinitionId: definitionId,
                stepOrder: 1,
                requiredRoleName: "Role1"));

            definition.Steps.Add(new WorkflowStepDefinition(
                step2DefId,
                workflowDefinitionId: definitionId,
                stepOrder: 2,
                requiredRoleName: "Role2"));

            await _workflowDefinitionRepository.InsertAsync(definition, autoSave: true);

            var instance = await _workflowManager.StartWorkflowAsync(new StartWorkflowModel
            {
                WorkflowDefinitionId = definitionId,
                EntityId = entityId,
                EntityType = "TestEntity",
                InitiatorUserId = initiatorId
            });
            await _workflowInstanceRepository.InsertAsync(instance, autoSave: true);

            var step1 = instance.Steps.First();

            // Act
            await _workflowManager.ProcessApprovalAsync(new ProcessApprovalModel
            {
                InstanceStepId = step1.Id,
                IsApproved = true,
                Note = "Approved Step 1",
                CurrentUserId = Guid.NewGuid(),
                CurrentUserRoles = new List<string> { "Role1" }
            });

            // Assert
            var updatedInstance = await _workflowInstanceRepository.GetAsync(instance.Id);
            updatedInstance.State.ShouldBe(WorkflowState.Active);

            var steps = await _workflowInstanceStepRepository.GetListAsync(x => x.WorkflowInstanceId == instance.Id);
            steps.Count.ShouldBe(2);

            var s1 = steps.First(x => x.WorkflowStepDefinitionId == step1DefId);
            var s2 = steps.First(x => x.WorkflowStepDefinitionId == step2DefId);

            s1.ActionTaken.ShouldBe(WorkflowActionType.Approved);
            s1.ActionDate.ShouldNotBeNull();

            s2.ActionTaken.ShouldBe(WorkflowActionType.Pending);
            s2.ActionDate.ShouldBeNull();
        });
    }
}
