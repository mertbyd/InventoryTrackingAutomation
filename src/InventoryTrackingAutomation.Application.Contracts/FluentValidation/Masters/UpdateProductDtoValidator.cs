using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ProductExceptionCodes.ValidationExceptions.Name.CannotEmpty)
            .MaximumLength(200).WithMessage(ProductExceptionCodes.ValidationExceptions.Name.MaxLength);

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage(ProductExceptionCodes.ValidationExceptions.Code.CannotEmpty)
            .MaximumLength(50).WithMessage(ProductExceptionCodes.ValidationExceptions.Code.MaxLength);

        RuleFor(x => x.UnitTypeId)
            .NotEmpty().WithMessage(ProductExceptionCodes.ValidationExceptions.UnitType.CannotEmpty);
    }
}
