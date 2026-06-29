using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

// islevi: WorkerType guncelleme isteginin temel alan kurallarini dogrular.
// sistemdeki gorevi: Lookup referans verisinin DB kolon limitlerine uygun kalmasini saglar.
public class UpdateWorkerTypeDtoValidator : AbstractValidator<UpdateWorkerTypeDto>
{
    public UpdateWorkerTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
