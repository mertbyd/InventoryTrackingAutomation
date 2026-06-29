using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Validation:Product:NameRequired")
            .MaximumLength(200).WithMessage("Validation:Product:NameMaxLength");

        RuleFor(x => x.Code)
            .MaximumLength(50).WithMessage("Validation:Product:CodeMaxLength");

        RuleFor(x => x.UnitTypeId)
            .NotEmpty().WithMessage("Validation:Product:UnitTypeRequired");
    }
}