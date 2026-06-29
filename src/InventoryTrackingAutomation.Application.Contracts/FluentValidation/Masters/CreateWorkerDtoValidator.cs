using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class CreateWorkerDtoValidator : AbstractValidator<CreateWorkerDto>
{
    public CreateWorkerDtoValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(50).WithMessage("Validation:Worker:RegistrationNumberMaxLength");

        RuleFor(x => x.WorkerTypeId)
            .NotEmpty().WithMessage("Validation:Worker:WorkerTypeRequired");
    }
}