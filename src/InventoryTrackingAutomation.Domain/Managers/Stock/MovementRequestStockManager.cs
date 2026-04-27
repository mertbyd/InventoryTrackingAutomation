using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Stock;
using Volo.Abp;
using Volo.Abp.Domain.Services;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Hareket talebi stok kurallarini yoneten domain manager.
/// </summary>
public class MovementRequestStockManager : DomainService
{
    private readonly IProductStockRepository _productStockRepository;

    public MovementRequestStockManager(IProductStockRepository productStockRepository)
    {
        _productStockRepository = productStockRepository;
    }

    public async Task DecreaseSourceStockAsync(MovementRequest request, IReadOnlyList<MovementRequestLine> lines)
    {
        foreach (var line in lines)
        {
            // Her talep satiri icin kaynak lokasyondaki mevcut stok bulunur.
            var stock = await _productStockRepository.FindAsync(x =>
                x.ProductId == line.ProductId &&
                x.SiteId == request.SourceSiteId);

            if (stock == null || stock.TotalQuantity < line.Quantity)
            {
                // Yetersiz stokta exception firlatilir; UnitOfWork tum onay islemini geri alir.
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.ProductStocks.InsufficientStock)
                    .WithData("ProductId", line.ProductId)
                    .WithData("RequestedQuantity", line.Quantity)
                    .WithData("AvailableQuantity", stock?.TotalQuantity ?? 0);
            }

            stock.TotalQuantity -= line.Quantity;
            await _productStockRepository.UpdateAsync(stock, autoSave: true);
        }
    }
}
