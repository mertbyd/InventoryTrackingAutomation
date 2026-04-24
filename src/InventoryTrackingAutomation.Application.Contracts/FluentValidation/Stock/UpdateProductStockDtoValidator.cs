using FluentValidation;
using InventoryTrackingAutomation.Dtos.Stock;

namespace InventoryTrackingAutomation.FluentValidation.Stock;

/// <summary>
/// UpdateProductStockDto için sade validation kuralları — geliştirme aşamasında kural eklenmemiştir.
/// </summary>
public class UpdateProductStockDtoValidator : AbstractValidator<UpdateProductStockDto>
{
    public UpdateProductStockDtoValidator()
    {
    }
}
