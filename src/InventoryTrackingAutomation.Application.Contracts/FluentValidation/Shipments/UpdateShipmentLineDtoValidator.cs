using FluentValidation;
using InventoryTrackingAutomation.Dtos.Shipments;

namespace InventoryTrackingAutomation.FluentValidation.Shipments;

/// <summary>
/// UpdateShipmentLineDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class UpdateShipmentLineDtoValidator : AbstractValidator<UpdateShipmentLineDto>
{
    public UpdateShipmentLineDtoValidator()
    {
    }
}
