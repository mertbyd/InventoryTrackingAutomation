using FluentValidation;
using InventoryTrackingAutomation.Dtos.Workflows;

namespace InventoryTrackingAutomation.FluentValidation.Workflows;

/// <summary>
/// CreateWorkflowStepDefinitionDto için doğrulama kuralları.
/// </summary>
public class CreateWorkflowStepDefinitionDtoValidator : AbstractValidator<CreateWorkflowStepDefinitionDto>
{
    public CreateWorkflowStepDefinitionDtoValidator()
    {
        RuleFor(x => x.StepOrder)
            .GreaterThan(0).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StepDefinition.OrderInvalid);

        RuleFor(x => x.RequiredRoleName)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.StepDefinition.RoleNameCannotEmpty)
            .When(x => string.IsNullOrWhiteSpace(x.ResolverKey))
            .MaximumLength(50).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StepDefinition.RoleNameMaxLength);

        RuleFor(x => x.ResolverKey)
            .MaximumLength(100).WithMessage(WorkflowExceptionCodes.ValidationExceptions.StepDefinition.ResolverKeyMaxLength);
    }
}
