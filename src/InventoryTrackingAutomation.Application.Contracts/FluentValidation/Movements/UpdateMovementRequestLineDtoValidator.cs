using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// UpdateMovementRequestLineDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class UpdateMovementRequestLineDtoValidator : AbstractValidator<UpdateMovementRequestLineDto>
{
    public UpdateMovementRequestLineDtoValidator()
    {
    }
}
