using System;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Workflows;

namespace InventoryTrackingAutomation.FluentValidation.Workflows;

/// <summary>
/// ProcessApprovalDto için doğrulama kuralları.
/// </summary>
public class ProcessApprovalDtoValidator : AbstractValidator<ProcessApprovalDto>
{
    public ProcessApprovalDtoValidator()
    {
        RuleFor(x => x.InstanceStepId)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.ProcessApproval.StepIdCannotEmpty)
            .NotEqual(Guid.Empty).WithMessage(WorkflowExceptionCodes.ValidationExceptions.ProcessApproval.StepIdInvalid);

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage(WorkflowExceptionCodes.ValidationExceptions.ProcessApproval.NoteMaxLength);
    }
}
