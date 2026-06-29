using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleDtoValidator()
    {
        RuleFor(x => x.PlateNumber)
            .NotEmpty().WithMessage("Validation:Vehicle:PlateNumberRequired")
            .MaximumLength(20).WithMessage("Validation:Vehicle:PlateNumberMaxLength");

        RuleFor(x => x.VehicleTypeId)
            .NotEmpty().WithMessage("Validation:Vehicle:VehicleTypeRequired");
    }
}