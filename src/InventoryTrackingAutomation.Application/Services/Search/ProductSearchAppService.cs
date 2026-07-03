using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Mappers.Masters;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Managers.Search;
using InventoryTrackingAutomation.Services.Search;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Search;

// Urun arama application servisi - ince orkestra katmani; okuma repository'de, reindex manager'da, mapping Mapperly'de.
// islevi: Urun metin aramasi ve index bakim operasyonlarini koordine eder.
// sistemdeki gorevi: Uygulama katmanindaki kullanim senaryolarini (use-case) gerceklestiren ana servis birimidir.
public class ProductSearchAppService : InventoryTrackingAutomationAppService, IProductSearchAppService
{
    public ProductSearchAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Urun arama index'inin Elasticsearch repository'si.
    private IProductSearchRepository _searchRepository => LazyGetRequiredService<IProductSearchRepository>();
    // Index bakim (reindex) akisinin domain manager'i.
    private ProductSearchManager _manager => LazyGetRequiredService<ProductSearchManager>();

    private static readonly ProductMapper _mapper = new ProductMapper();

    // Keyword'u urun adinda fuzzy, urun kodunda birebir arar; bos keyword tum kayitlari sayfali doner.
    public async Task<PagedResultDto<ProductIndexDto>> SearchAsync(ProductSearchInputDto input)
    {
        return await _searchRepository.SearchAsync(input.Keyword, input.SkipCount, input.MaxResultCount);
    }

    // Urun index'ini PostgreSQL'deki guncel veriden bastan kurar; donusum mapper'dan, dongu manager'dan.
    public async Task<long> ReindexAsync()
    {
        return await _manager.ReindexAsync(_mapper.MapToIndexDto);
    }
}
