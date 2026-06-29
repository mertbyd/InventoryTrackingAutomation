using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class UpdateUnitTypeDtoValidator : AbstractValidator<UpdateUnitTypeDto>
{
    public UpdateUnitTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage("Validation:Lookup:CodeRequired")
                            .MaximumLength(50).WithMessage("Validation:Lookup:CodeMaxLength");
        RuleFor(x => x.Name).NotEmpty().WithMessage("Validation:Lookup:NameRequired")
                            .MaximumLength(100).WithMessage("Validation:Lookup:NameMaxLength");
        RuleFor(x => x.Description).MaximumLength(500).WithMessage("Validation:Lookup:DescriptionMaxLength");
    }
}