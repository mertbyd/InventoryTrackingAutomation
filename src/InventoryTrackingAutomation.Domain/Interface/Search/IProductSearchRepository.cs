using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Interface.Search;

/// <summary>
/// Urun arama index'i (inventory-products) icin repository arayuzu.
/// </summary>
public interface IProductSearchRepository : IElasticsearchRepository<ProductIndexDto>
{
    /// <summary>
    /// Keyword'u urun adinda fuzzy, urun kodunda birebir arar; bos keyword tum kayitlari sayfali doner.
    /// </summary>
    Task<PagedResultDto<ProductIndexDto>> SearchAsync(string? keyword, int skipCount, int maxResultCount);
}
