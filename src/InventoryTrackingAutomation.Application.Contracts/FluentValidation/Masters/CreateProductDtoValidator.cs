using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage(ProductExceptionCodes.ValidationExceptions.Name.CannotEmpty)
            .MaximumLength(200).WithMessage(ProductExceptionCodes.ValidationExceptions.Name.MaxLength);

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage(ProductExceptionCodes.ValidationExceptions.Code.MaxLength);

        RuleFor(x => x.UnitTypeId)
            .NotEmpty().WithMessage(ProductExceptionCodes.ValidationExceptions.UnitType.CannotEmpty);
    }
}
