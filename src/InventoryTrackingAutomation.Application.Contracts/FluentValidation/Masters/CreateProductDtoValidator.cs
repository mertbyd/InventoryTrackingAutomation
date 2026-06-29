using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// CreateProductDto için sade validation kuralları.
/// </summary>
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .MaximumLength(50);

        // islevi: UnitType artik lookup FK oldugu icin bos Guid gonderilmesini engeller.
        RuleFor(x => x.UnitTypeId)
            .NotEmpty();
    }
}
