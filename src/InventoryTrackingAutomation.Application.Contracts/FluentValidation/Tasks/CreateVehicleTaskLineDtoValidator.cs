using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// CreateVehicleTaskLineDto icin validation kurallari.
/// </summary>
public class CreateVehicleTaskLineDtoValidator : AbstractValidator<CreateVehicleTaskLineDto>
{
    public CreateVehicleTaskLineDtoValidator()
    {
        RuleFor(x => x.TaskLineId).NotEmpty();
        RuleFor(x => x.AllocatedQuantity).GreaterThan(0);
    }
}
