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
            .NotEmpty().WithMessage("ResolverKey belirtilmediyse Rol Adı zorunludur.")
            .When(x => string.IsNullOrWhiteSpace(x.ResolverKey))
            .MaximumLength(50).WithMessage("Rol adı en fazla 50 karakter olabilir.");

        RuleFor(x => x.ResolverKey)
            .MaximumLength(100).WithMessage("ResolverKey en fazla 100 karakter olabilir.");
    }
}
