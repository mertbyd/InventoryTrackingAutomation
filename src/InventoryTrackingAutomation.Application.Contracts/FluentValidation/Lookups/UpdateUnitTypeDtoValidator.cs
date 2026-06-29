using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

// islevi: UnitType guncelleme isteginin temel alan kurallarini dogrular.
// sistemdeki gorevi: Lookup referans verisinin DB kolon limitlerine uygun kalmasini saglar.
public class UpdateUnitTypeDtoValidator : AbstractValidator<UpdateUnitTypeDto>
{
    public UpdateUnitTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
