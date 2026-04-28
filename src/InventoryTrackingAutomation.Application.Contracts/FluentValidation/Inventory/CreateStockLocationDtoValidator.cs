using FluentValidation;
using InventoryTrackingAutomation.Dtos.Inventory;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// CreateStockLocationDto icin validation kurallari.
/// </summary>
public class CreateStockLocationDtoValidator : AbstractValidator<CreateStockLocationDto>
{
    public CreateStockLocationDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.LocationType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0);
    }
}
