using System;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Workflows;

namespace InventoryTrackingAutomation.FluentValidation.Workflows;

/// <summary>
/// StartWorkflowDto için doğrulama kuralları.
/// </summary>
public class StartWorkflowDtoValidator : AbstractValidator<StartWorkflowDto>
{
    public StartWorkflowDtoValidator()
    {
        RuleFor(x => x.EntityType)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.EntityTypeCannotEmpty)
            .MaximumLength(50).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.EntityTypeMaxLength);

        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.EntityIdCannotEmpty)
            .NotEqual(Guid.Empty).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.EntityIdInvalid);

        RuleFor(x => x.WorkflowDefinitionId)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.DefinitionIdCannotEmpty)
            .NotEqual(Guid.Empty).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StartWorkflow.DefinitionIdInvalid);
    }
}
