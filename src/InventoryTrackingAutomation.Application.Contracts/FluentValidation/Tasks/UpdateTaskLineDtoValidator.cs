using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// UpdateTaskLineDto icin validation kurallari.
/// </summary>
public class UpdateTaskLineDtoValidator : AbstractValidator<UpdateTaskLineDto>
{
    public UpdateTaskLineDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
