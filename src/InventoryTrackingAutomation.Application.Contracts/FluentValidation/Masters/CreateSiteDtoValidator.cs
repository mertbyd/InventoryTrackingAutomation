using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// CreateSiteDto için sade validation kuralları.
/// </summary>
public class CreateSiteDtoValidator : AbstractValidator<CreateSiteDto>
{
    public CreateSiteDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .MaximumLength(50);

        RuleFor(x => x.SiteType)
            .IsInEnum();

        RuleFor(x => x.Address)
            .MaximumLength(500);
    }
}
