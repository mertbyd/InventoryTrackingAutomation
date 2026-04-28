using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// UpdateVehicleTaskDto icin validation kurallari.
/// </summary>
public class UpdateVehicleTaskDtoValidator : AbstractValidator<UpdateVehicleTaskDto>
{
    public UpdateVehicleTaskDtoValidator()
    {
        RuleFor(x => x.VehicleId).NotEmpty();
        RuleFor(x => x.InventoryTaskId).NotEmpty();
        RuleFor(x => x.AssignedAt).NotEmpty();
    }
}
