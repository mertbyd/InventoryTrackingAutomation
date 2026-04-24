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
            .GreaterThan(0).WithMessage("Adım sırası 0'dan büyük olmalıdır.");

        RuleFor(x => x.RequiredRoleName)
            .NotEmpty().WithMessage("Amir onayı gerekmiyorsa Rol Adı zorunludur.")
            .When(x => !x.IsManagerApprovalRequired)
            .MaximumLength(50).WithMessage("Rol adı en fazla 50 karakter olabilir.");
    }
}
