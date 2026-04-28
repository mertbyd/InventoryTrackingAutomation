using System;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// CreateMovementRequestWithLinesDto icin hareket rotasi ve satir validation kurallari.
/// </summary>
public class CreateMovementRequestWithLinesDtoValidator : AbstractValidator<CreateMovementRequestWithLinesDto>
{
    public CreateMovementRequestWithLinesDtoValidator()
    {
        RuleFor(x => x.RequestNumber).MaximumLength(50);
        RuleFor(x => x.RequestNote).NotEmpty().MaximumLength(2000);
        RuleFor(x => x.Priority).IsInEnum();

        RuleFor(x => x.SourceWarehouseId).NotEmpty();

        RuleFor(x => x.RequestedVehicleId)
            .NotEmpty()
            .WithMessage("Sevkiyat araci zorunludur.");

        RuleFor(x => x.TargetWarehouseId)
            .NotEmpty()
            .When(x => !x.AssignedTaskId.HasValue || x.AssignedTaskId == Guid.Empty)
            .WithMessage("Depo-depo transferde hedef depo zorunludur.");

        RuleFor(x => x.TargetWarehouseId)
            .NotEqual(x => x.SourceWarehouseId)
            .When(x => x.TargetWarehouseId.HasValue && x.TargetWarehouseId != Guid.Empty)
            .WithMessage("Hedef depo kaynak depodan farkli olmalidir.");

        RuleFor(x => x.Lines)
            .NotEmpty()
            .WithMessage("En az bir talep satiri gereklidir.");

        RuleForEach(x => x.Lines).ChildRules(line =>
        {
            line.RuleFor(l => l.ProductId).NotEmpty();
            line.RuleFor(l => l.Quantity).GreaterThan(0);
        });
    }
}
