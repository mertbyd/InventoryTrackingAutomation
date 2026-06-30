using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;


namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class CreateWorkerTypeDtoValidator : AbstractValidator<CreateWorkerTypeDto>
{
    public CreateWorkerTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(WorkerTypeExceptionCodes.ValidationExceptions.Code.CannotEmpty)
                            .MaximumLength(50).WithMessage(WorkerTypeExceptionCodes.ValidationExceptions.Code.MaxLength);
        RuleFor(x => x.Name).NotEmpty().WithMessage(WorkerTypeExceptionCodes.ValidationExceptions.Name.CannotEmpty)
                            .MaximumLength(100).WithMessage(WorkerTypeExceptionCodes.ValidationExceptions.Name.MaxLength);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(WorkerTypeExceptionCodes.ValidationExceptions.Description.MaxLength);
    }
}
