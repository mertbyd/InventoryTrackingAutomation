using FluentValidation;
using InventoryTrackingAutomation.Dtos.Stock;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// CreateStockMovementDto için sade validation kuralları.
/// </summary>
public class CreateStockMovementDtoValidator : AbstractValidator<CreateStockMovementDto>
{
    public CreateStockMovementDtoValidator()
    {
        RuleFor(x => x.MovementType)
            .IsInEnum();

        RuleFor(x => x.ReferenceType)
            .MaximumLength(100);

        RuleFor(x => x.Note)
            .MaximumLength(500);
    }
}
