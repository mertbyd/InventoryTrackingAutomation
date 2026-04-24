using FluentValidation;
using InventoryTrackingAutomation.Dtos.Shipments;

namespace InventoryTrackingAutomation.FluentValidation.Shipments;

/// <summary>
/// CreateShipmentLineDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class CreateShipmentLineDtoValidator : AbstractValidator<CreateShipmentLineDto>
{
    public CreateShipmentLineDtoValidator()
    {
    }
}
