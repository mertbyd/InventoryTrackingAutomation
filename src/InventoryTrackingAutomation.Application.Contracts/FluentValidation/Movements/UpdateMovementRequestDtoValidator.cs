using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// UpdateMovementRequestDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
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

        RuleFor(x => x.RequestedVehicleId)
            .NotEmpty();
    }
}
