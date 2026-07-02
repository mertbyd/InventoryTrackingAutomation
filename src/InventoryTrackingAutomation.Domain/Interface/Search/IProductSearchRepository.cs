using InventoryTrackingAutomation.Dtos.Masters;

namespace InventoryTrackingAutomation.Interface.Search;

/// <summary>
/// Urun arama index'i (inventory-products) icin repository arayuzu.
/// </summary>
public interface IProductSearchRepository : IElasticsearchRepository<ProductIndexDto>
{
}
