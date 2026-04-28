using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// UpdateWarehouseDto icin depo kurallarini dogrular.
/// </summary>
public class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDto>
{
    public UpdateWarehouseDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Address)
            .MaximumLength(500);
    }
}
