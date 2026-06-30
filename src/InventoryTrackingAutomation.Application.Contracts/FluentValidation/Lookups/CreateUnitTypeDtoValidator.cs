using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.ExceptionCodes.UnitTypes;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class CreateUnitTypeDtoValidator : AbstractValidator<CreateUnitTypeDto>
{
    public CreateUnitTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(UnitTypeExceptionCodes.ValidationExceptions.Code.CannotEmpty)
                            .MaximumLength(50).WithMessage(UnitTypeExceptionCodes.ValidationExceptions.Code.MaxLength);
        RuleFor(x => x.Name).NotEmpty().WithMessage(UnitTypeExceptionCodes.ValidationExceptions.Name.CannotEmpty)
                            .MaximumLength(100).WithMessage(UnitTypeExceptionCodes.ValidationExceptions.Name.MaxLength);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(UnitTypeExceptionCodes.ValidationExceptions.Description.MaxLength);
    }
}