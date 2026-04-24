using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

/// <summary>
/// CreateDepartmentDto için sade validation kuralları.
/// </summary>
public class CreateDepartmentDtoValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Code)
            .MaximumLength(50);
    }
}
