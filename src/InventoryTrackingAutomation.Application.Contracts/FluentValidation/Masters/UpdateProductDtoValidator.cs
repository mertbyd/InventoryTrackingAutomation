using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Validation:Product:NameRequired")
            .MaximumLength(200).WithMessage("Validation:Product:NameMaxLength");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Validation:Product:CodeRequired")
            .MaximumLength(50).WithMessage("Validation:Product:CodeMaxLength");

        RuleFor(x => x.UnitTypeId)
            .NotEmpty().WithMessage("Validation:Product:UnitTypeRequired");
    }
}