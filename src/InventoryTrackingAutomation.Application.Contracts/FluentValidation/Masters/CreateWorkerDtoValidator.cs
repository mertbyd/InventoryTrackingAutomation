using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class CreateWorkerDtoValidator : AbstractValidator<CreateWorkerDto>
{
    public CreateWorkerDtoValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .MaximumLength(50).WithMessage(WorkerExceptionCodes.ValidationExceptions.RegistrationNumber.MaxLength);

        RuleFor(x => x.WorkerTypeId)
            .NotEmpty().WithMessage(WorkerExceptionCodes.ValidationExceptions.WorkerType.CannotEmpty);
    }
}
