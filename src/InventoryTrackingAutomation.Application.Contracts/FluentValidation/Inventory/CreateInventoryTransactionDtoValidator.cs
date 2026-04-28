using FluentValidation;
using InventoryTrackingAutomation.Dtos.Inventory;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// CreateInventoryTransactionDto icin validation kurallari.
/// </summary>
public class CreateInventoryTransactionDtoValidator : AbstractValidator<CreateInventoryTransactionDto>
{
    public CreateInventoryTransactionDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.TransactionType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}
