using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// CreateWorkerDto için sade validation kuralları.
/// </summary>
public class CreateWorkerDtoValidator : AbstractValidator<CreateWorkerDto>
{
    public CreateWorkerDtoValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(50);

        RuleFor(x => x.WorkerType)
            .IsInEnum();
    }
}
