using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;

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
        RuleFor(x => x.Type).IsInEnum();
        RuleFor(x => x.Region).MaximumLength(100);
        RuleFor(x => x.Status).IsInEnum();
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.SourceWarehouseId).NotEmpty();
        RuleFor(x => x.TargetWarehouseId)
            .NotEmpty()
            .When(x => x.Type == InventoryTaskTypeEnum.WarehouseTransfer);
        RuleFor(x => x.TargetWarehouseId)
            .NotEqual(x => x.SourceWarehouseId)
            .When(x => x.TargetWarehouseId.HasValue);
    }
}
