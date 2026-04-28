using FluentValidation;
using InventoryTrackingAutomation.Dtos.Inventory;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// UpdateStockLocationDto icin validation kurallari.
/// </summary>
public class UpdateStockLocationDtoValidator : AbstractValidator<UpdateStockLocationDto>
{
    public UpdateStockLocationDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.LocationType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ReservedQuantity).GreaterThanOrEqualTo(0);
    }
}
