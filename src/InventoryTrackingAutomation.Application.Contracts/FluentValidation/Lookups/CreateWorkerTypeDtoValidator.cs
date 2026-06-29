using FluentValidation;
using InventoryTrackingAutomation.Dtos.Lookups;

namespace InventoryTrackingAutomation.FluentValidation.Lookups;

// islevi: WorkerType olusturma isteginin temel alan kurallarini dogrular.
// sistemdeki gorevi: Bos kod/ad ve kolon uzunlugu ihlallerini AppService'e girmeden yakalar.
public class CreateWorkerTypeDtoValidator : AbstractValidator<CreateWorkerTypeDto>
{
    public CreateWorkerTypeDtoValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(500);
    }
}
