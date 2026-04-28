using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// CreateVehicleTaskDto icin validation kurallari.
/// </summary>
public class CreateVehicleTaskDtoValidator : AbstractValidator<CreateVehicleTaskDto>
{
    public CreateVehicleTaskDtoValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.InventoryTaskId).NotEmpty();
        RuleFor(x => x.AssignedAt).NotEmpty();
    }
}
