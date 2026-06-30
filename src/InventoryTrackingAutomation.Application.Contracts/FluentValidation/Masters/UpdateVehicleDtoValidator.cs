using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

public class UpdateVehicleDtoValidator : AbstractValidator<UpdateVehicleDto>
{
    public UpdateVehicleDtoValidator()
    {
        RuleFor(x => x.PlateNumber)
            .NotEmpty().WithMessage(VehicleExceptionCodes.ValidationExceptions.PlateNumber.CannotEmpty)
            .MaximumLength(20).WithMessage(VehicleExceptionCodes.ValidationExceptions.PlateNumber.MaxLength);

        RuleFor(x => x.VehicleTypeId)
            .NotEmpty().WithMessage(VehicleExceptionCodes.ValidationExceptions.VehicleType.CannotEmpty);
    }
}
