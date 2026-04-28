using FluentValidation;
using InventoryTrackingAutomation.Dtos.Inventory;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// UpdateInventoryTransactionDto icin validation kurallari.
/// </summary>
public class UpdateInventoryTransactionDtoValidator : AbstractValidator<UpdateInventoryTransactionDto>
{
    public UpdateInventoryTransactionDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.TransactionType).IsInEnum();
        RuleFor(x => x.Quantity).GreaterThan(0);
        RuleFor(x => x.Note).MaximumLength(500);
    }
}
