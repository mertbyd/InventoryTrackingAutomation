using FluentValidation;
using InventoryTrackingAutomation.Dtos.Workflows;

namespace InventoryTrackingAutomation.FluentValidation.Workflows;

/// <summary>
/// CreateWorkflowDefinitionDto için doğrulama kuralları.
/// </summary>
public class CreateWorkflowDefinitionDtoValidator : AbstractValidator<CreateWorkflowDefinitionDto>
{
    public CreateWorkflowDefinitionDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(WorkflowExceptionCodes.ValidationExceptions.Definition.NameCannotEmpty)
            .MaximumLength(100).WithMessage(WorkflowExceptionCodes.ValidationExceptions.Definition.NameMaxLength);

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage(WorkflowExceptionCodes.ValidationExceptions.Definition.VersionInvalid);

        RuleForEach(x => x.Steps).SetValidator(new CreateWorkflowStepDefinitionDtoValidator());
    }
}
