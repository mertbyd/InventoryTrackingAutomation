using FluentValidation;
using InventoryTrackingAutomation.Dtos.Shipments;

namespace InventoryTrackingAutomation.FluentValidation.Shipments;

/// <summary>
/// CreateShipmentDto için sade validation kuralları.
/// </summary>
public class CreateShipmentDtoValidator : AbstractValidator<CreateShipmentDto>
{
    public CreateShipmentDtoValidator()
    {
        RuleFor(x => x.ShipmentNumber)
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Note)
            .MaximumLength(500);
    }
}
