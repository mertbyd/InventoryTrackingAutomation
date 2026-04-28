using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// CreateInventoryTaskDto icin validation kurallari.
/// </summary>
public class CreateInventoryTaskDtoValidator : AbstractValidator<CreateInventoryTaskDto>
{
    public CreateInventoryTaskDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Region).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(1000);
    }
}
