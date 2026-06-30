using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.ExceptionCodes.ProductCategories;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class UpdateProductCategoryDtoValidator : AbstractValidator<UpdateProductCategoryDto>
{
    public UpdateProductCategoryDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(ProductCategoryExceptionCodes.ValidationExceptions.Code.CannotEmpty)
                            .MaximumLength(50).WithMessage(ProductCategoryExceptionCodes.ValidationExceptions.Code.MaxLength);
        RuleFor(x => x.Name).NotEmpty().WithMessage(ProductCategoryExceptionCodes.ValidationExceptions.Name.CannotEmpty)
                            .MaximumLength(100).WithMessage(ProductCategoryExceptionCodes.ValidationExceptions.Name.MaxLength);
    }
}
