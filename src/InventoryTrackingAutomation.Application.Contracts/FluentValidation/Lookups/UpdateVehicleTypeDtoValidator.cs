using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

// islevi: VehicleType guncelleme isteginin temel alan kurallarini dogrular.
// sistemdeki gorevi: Lookup referans verisinin DB kolon limitlerine uygun kalmasini saglar.
public class UpdateVehicleTypeDtoValidator : AbstractValidator<UpdateVehicleTypeDto>
{
    public UpdateVehicleTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
