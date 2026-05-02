using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/// <summary>
/// CreateTaskLineDto icin validation kurallari.
/// </summary>
public class CreateTaskLineDtoValidator : AbstractValidator<CreateTaskLineDto>
{
    public CreateTaskLineDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
