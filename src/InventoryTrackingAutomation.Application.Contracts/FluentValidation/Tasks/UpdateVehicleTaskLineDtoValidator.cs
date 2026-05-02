using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// UpdateVehicleTaskLineDto için validation kuralları.
/// </summary>
public class UpdateVehicleTaskLineDtoValidator : AbstractValidator<UpdateVehicleTaskLineDto>
{
    public UpdateVehicleTaskLineDtoValidator()
    {
        RuleFor(x => x.AllocatedQuantity).GreaterThan(0);
    }
}
