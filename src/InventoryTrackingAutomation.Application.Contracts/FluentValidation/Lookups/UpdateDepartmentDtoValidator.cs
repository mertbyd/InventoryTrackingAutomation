using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.ExceptionCodes.Departments;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

public class UpdateDepartmentDtoValidator : AbstractValidator<UpdateDepartmentDto>
{
    public UpdateDepartmentDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().WithMessage(DepartmentExceptionCodes.ValidationExceptions.Code.CannotEmpty)
                            .MaximumLength(50).WithMessage(DepartmentExceptionCodes.ValidationExceptions.Code.MaxLength);
        RuleFor(x => x.Name).NotEmpty().WithMessage(DepartmentExceptionCodes.ValidationExceptions.Name.CannotEmpty)
                            .MaximumLength(100).WithMessage(DepartmentExceptionCodes.ValidationExceptions.Name.MaxLength);
    }
}
