using FluentValidation;
using InventoryTrackingAutomation.Dtos.Stock;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// UpdateStockMovementDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateStockMovementDtoValidator : AbstractValidator<UpdateStockMovementDto>
{
    public UpdateStockMovementDtoValidator()
    {
        RuleFor(x => x.MovementType)
            .IsInEnum();

        RuleFor(x => x.ReferenceType)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Note)
            .MaximumLength(500);
    }
}
