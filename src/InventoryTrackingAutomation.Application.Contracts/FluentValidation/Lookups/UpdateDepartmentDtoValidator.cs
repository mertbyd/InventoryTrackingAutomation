using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

/// <summary>
/// UpdateDepartmentDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .NotEmpty()
            .MaximumLength(50);
    }
}
