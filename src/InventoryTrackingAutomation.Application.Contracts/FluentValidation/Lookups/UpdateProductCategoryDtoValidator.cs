using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

/// <summary>
/// UpdateProductCategoryDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateProductCategoryDtoValidator : AbstractValidator<UpdateProductCategoryDto>
{
    public UpdateProductCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}
