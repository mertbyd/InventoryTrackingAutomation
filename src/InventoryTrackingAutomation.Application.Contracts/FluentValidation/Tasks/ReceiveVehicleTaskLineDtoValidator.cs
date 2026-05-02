using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// ReceiveVehicleTaskLineDto icin validation kurallari.
/// </summary>
public class ReceiveVehicleTaskLineDtoValidator : AbstractValidator<ReceiveVehicleTaskLineDto>
{
    public ReceiveVehicleTaskLineDtoValidator()
    {
        RuleFor(x => x.ReceivedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.DamagedQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.LostQuantity).GreaterThanOrEqualTo(0);
        RuleFor(x => x.ConsumedQuantity).GreaterThanOrEqualTo(0);
    }
}
