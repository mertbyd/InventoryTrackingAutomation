using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;


namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// UpdateMovementRequestDto icin hareket rotasi validation kurallari.
/// </summary>
public class UpdateMovementRequestDtoValidator : AbstractValidator<UpdateMovementRequestDto>
{
    public UpdateMovementRequestDtoValidator()
    {
        RuleFor(x => x.RequestNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.RequestNote)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.VehicleTaskId)
            .NotEmpty()
            .WithMessage(MovementRequestExceptionCodes.ValidationExceptions.VehicleTaskId.CannotEmpty);

    }
}
