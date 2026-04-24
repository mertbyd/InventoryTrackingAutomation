using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.FluentValidation.Masters;

/// <summary>
/// UpdateWorkerDto için validation kuralları — güncelleme işleminde tüm alanlar zorunlu tutulur.
/// </summary>
public class UpdateWorkerDtoValidator : AbstractValidator<UpdateWorkerDto>
{
    public UpdateWorkerDtoValidator()
    {
        RuleFor(x => x.RegistrationNumber)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.WorkerType)
            .IsInEnum();
    }
}
