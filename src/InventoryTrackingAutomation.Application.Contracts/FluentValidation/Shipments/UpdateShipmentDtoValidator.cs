using FluentValidation;
using InventoryTrackingAutomation.Dtos.Shipments;

namespace InventoryTrackingAutomation.FluentValidation.Shipments;

/// <summary>
/// UpdateShipmentDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateShipmentDtoValidator : AbstractValidator<UpdateShipmentDto>
{
    public UpdateShipmentDtoValidator()
    {
        RuleFor(x => x.ShipmentNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Note)
            .MaximumLength(500);
    }
}
