using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// UpdateInventoryTaskDto icin validation kurallari.
/// </summary>
public class UpdateInventoryTaskDtoValidator : AbstractValidator<UpdateInventoryTaskDto>
{
    public UpdateInventoryTaskDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Region).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
