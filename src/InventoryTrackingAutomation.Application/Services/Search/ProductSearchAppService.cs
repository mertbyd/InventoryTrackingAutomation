using System.Threading.Tasks;
using InventoryTrackingAutomation.Application.Mappers.Masters;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Interface.Search;
using InventoryTrackingAutomation.Managers.Search;
using InventoryTrackingAutomation.Services.Search;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Search;

// Urun arama application servisi â€” ince orkestra katmani; okuma repository'de, reindex manager'da, mapping Mapperly'de.
//iÅŸlevi: Urun metin aramasi ve index bakim operasyonlarini koordine eder.
//sistemdeki gÃ¶revi: Uygulama katmanÄ±ndaki kullanÄ±m senaryolarÄ±nÄ± (use-case) gerÃ§ekleÅŸtiren ana servis birimidir.
public class ProductSearchAppService : InventoryTrackingAutomationAppService, IProductSearchAppService
{
    public ProductSearchAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Urun arama index'ine lambda ile sorgu atan Elasticsearch repository'si.
    private IProductSearchRepository _searchRepository => LazyGetRequiredService<IProductSearchRepository>();
    // Index bakim (reindex) akisinin domain manager'i.
    private ProductSearchManager _manager => LazyGetRequiredService<ProductSearchManager>();

    private static readonly ProductMapper _mapper = new ProductMapper();

    // Keyword'u urun adinda fuzzy, urun kodunda birebir arar; bos keyword tum kayitlari sayfali doner.
    //iÅŸlevi: Ä°lgili iÅŸ senaryosunu (use-case) yÃ¼rÃ¼tÃ¼r.
    //sistemdeki gÃ¶revi: Uygulama katmanÄ±ndaki bir operasyonu atomik olarak gerÃ§ekleÅŸtirir.
    public async Task<PagedResultDto<ProductIndexDto>> SearchAsync(ProductSearchInputDto input)
    {
        var keyword = input.Keyword;
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return await _searchRepository.GetListByAsync(x => true, input.SkipCount, input.MaxResultCount);
        }

        return await _searchRepository.GetListByAsync(
            x => x.Name.Contains(keyword) || x.Code == keyword,
            input.SkipCount,
            input.MaxResultCount);
    }

    // Urun index'ini PostgreSQL'deki guncel veriden bastan kurar; donusum mapper'dan, dongu manager'dan.
    //iÅŸlevi: Ä°lgili iÅŸ senaryosunu (use-case) yÃ¼rÃ¼tÃ¼r.
    //sistemdeki gÃ¶revi: Uygulama katmanÄ±ndaki bir operasyonu atomik olarak gerÃ§ekleÅŸtirir.
    public async Task<long> ReindexAsync()
    {
        return await _manager.ReindexAsync(_mapper.MapToIndexDto);
    }
}
