using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// CreateMovementRequestWithLinesDto için validation kuralları.
/// </summary>
public class CreateMovementRequestWithLinesDtoValidator : AbstractValidator<CreateMovementRequestWithLinesDto>
{
    public CreateMovementRequestWithLinesDtoValidator()
    {
        RuleFor(x => x.RequestNumber).MaximumLength(50);
        RuleFor(x => x.RequestNote).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Priority).IsInEnum();

        RuleFor(x => x.SourceSiteId).NotEmpty();
        RuleFor(x => x.TargetSiteId)
            .NotEmpty()
            .NotEqual(x => x.SourceSiteId)
            .WithMessage("Hedef lokasyon kaynak lokasyondan farklı olmalıdır.");
        RuleFor(x => x.RequestedVehicleId).NotEmpty();

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("En az bir talep satırı gereklidir.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
