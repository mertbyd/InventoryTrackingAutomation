using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

/// <summary>
/// CreateProductCategoryDto için sade validation kuralları.
/// </summary>
public class CreateProductCategoryDtoValidator : AbstractValidator<CreateProductCategoryDto>
{
    public CreateProductCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .MaximumLength(50);
    }
}
