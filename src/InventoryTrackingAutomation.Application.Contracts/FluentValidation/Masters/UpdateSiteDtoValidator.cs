using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// UpdateSiteDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateSiteDtoValidator : AbstractValidator<UpdateSiteDto>
{
    public UpdateSiteDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.SiteType)
            .IsInEnum();

        RuleFor(x => x.Address)
            .MaximumLength(500);
    }
}
