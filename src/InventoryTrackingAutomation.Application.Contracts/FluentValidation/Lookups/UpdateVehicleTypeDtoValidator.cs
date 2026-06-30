using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;



namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class UpdateVehicleTypeDtoValidator : AbstractValidator<UpdateVehicleTypeDto>
{
    public UpdateVehicleTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(VehicleTypeExceptionCodes.ValidationExceptions.Code.CannotEmpty)
                            .MaximumLength(50).WithMessage(VehicleTypeExceptionCodes.ValidationExceptions.Code.MaxLength);
        RuleFor(x => x.Name).NotEmpty().WithMessage(VehicleTypeExceptionCodes.ValidationExceptions.Name.CannotEmpty)
                            .MaximumLength(100).WithMessage(VehicleTypeExceptionCodes.ValidationExceptions.Name.MaxLength);
        RuleFor(x => x.Description).MaximumLength(500).WithMessage(VehicleTypeExceptionCodes.ValidationExceptions.Description.MaxLength);
    }
}
