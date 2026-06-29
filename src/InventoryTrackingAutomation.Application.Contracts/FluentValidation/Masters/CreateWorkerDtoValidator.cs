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

        // islevi: WorkerType artik lookup FK oldugu icin bos Guid gonderilmesini engeller.
        RuleFor(x => x.WorkerTypeId)
            .NotEmpty();
    }
}
