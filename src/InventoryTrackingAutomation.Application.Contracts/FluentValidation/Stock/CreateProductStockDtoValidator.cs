using FluentValidation;
using InventoryTrackingAutomation.Dtos.Stock;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// CreateProductStockDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class CreateProductStockDtoValidator : AbstractValidator<CreateProductStockDto>
{
    public CreateProductStockDtoValidator()
    {
    }
}
