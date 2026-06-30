using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;


namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// CreateMovementRequestDto icin hareket rotasi validation kurallari.
/// </summary>
public class CreateMovementRequestDtoValidator : AbstractValidator<CreateMovementRequestDto>
{
    public CreateMovementRequestDtoValidator()
    {
        RuleFor(x => x.RequestNumber)
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
