using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// CreateMovementRequestLineDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class CreateMovementRequestLineDtoValidator : AbstractValidator<CreateMovementRequestLineDto>
{
    public CreateMovementRequestLineDtoValidator()
    {
    }
}
