using System;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Movements;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/// <summary>
/// UpdateMovementRequestDto icin hareket rotasi validation kurallari.
/// </summary>
public class UpdateMovementRequestDtoValidator : AbstractValidator<UpdateMovementRequestDto>
{
    public UpdateMovementRequestDtoValidator()
    {
        RuleFor(x => x.RequestNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.RequestNote)
            .NotEmpty()
            .MaximumLength(2000);

        RuleFor(x => x.Priority)
            .IsInEnum();

        RuleFor(x => x.SourceWarehouseId)
            .NotEmpty();

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
    }
}
