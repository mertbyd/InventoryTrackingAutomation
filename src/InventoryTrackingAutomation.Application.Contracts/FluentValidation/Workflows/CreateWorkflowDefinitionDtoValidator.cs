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
            .NotEmpty().WithMessage("İş akışı adı boş olamaz.")
            .MaximumLength(100).WithMessage("İş akışı adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.Version)
            .GreaterThan(0).WithMessage("Versiyon 0'dan büyük olmalıdır.");

        RuleForEach(x => x.Steps).SetValidator(new CreateWorkflowStepDefinitionDtoValidator());
    }
}
