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
            .NotEmpty().WithMessage("Adım Id boş olamaz.")
            .NotEqual(Guid.Empty).WithMessage("Geçersiz Adım Id.");

        RuleFor(x => x.Note)
            .MaximumLength(500).WithMessage("Not en fazla 500 karakter olabilir.");
    }
}
