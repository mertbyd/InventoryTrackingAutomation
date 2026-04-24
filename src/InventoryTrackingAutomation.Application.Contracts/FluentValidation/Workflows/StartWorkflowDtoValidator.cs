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
            .NotEmpty().WithMessage("Entity türü boş olamaz.")
            .MaximumLength(50).WithMessage("Entity türü en fazla 50 karakter olabilir.");

        RuleFor(x => x.EntityId)
            .NotEmpty().WithMessage("Entity Id boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Geçersiz Entity Id.");

        RuleFor(x => x.WorkflowDefinitionId)
            .NotEmpty().WithMessage("İş akışı tanım Id boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Geçersiz İş Akışı Tanım Id.");
    }
}
