using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Search;

/// <summary>
/// Urun metin aramasi application servis sozlesmesi.
/// </summary>
// islevi: Urun arama ve index bakim (reindex) operasyonlarini sunar.
// sistemdeki gorevi: Text-search okumalari PostgreSQL LIKE sorgusu yerine Elasticsearch'ten yapilir.
public interface IProductSearchAppService : IApplicationService
{
    /// <summary>
    /// Keyword'u urun adinda fuzzy, urun kodunda birebir arar; sayfali dokuman listesi doner.
    /// </summary>
    Task<PagedResultDto<ProductIndexDto>> SearchAsync(ProductSearchInputDto input);

    /// <summary>
    /// Urun index'ini PostgreSQL'den bastan kurar; index'lenen dokuman sayisini doner.
    /// </summary>
    Task<long> ReindexAsync();
}
