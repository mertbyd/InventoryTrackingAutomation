using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Managers.Search;
using InventoryTrackingAutomation.Services.Search;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Search;

// Urun arama application servisi — ince orkestra katmani; arama/index isleri ProductSearchManager'da.
//işlevi: Urun metin aramasi ve index bakim operasyonlarini koordine eder.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class ProductSearchAppService : InventoryTrackingAutomationAppService, IProductSearchAppService
{
    public ProductSearchAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Elasticsearch urun index'inin tek sahibi olan domain manager.
    private ProductSearchManager _manager => LazyGetRequiredService<ProductSearchManager>();

    // Keyword'u urun adinda fuzzy, urun kodunda birebir arar.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<ProductIndexDto>> SearchAsync(ProductSearchInputDto input)
    {
        return await _manager.SearchAsync(input.Keyword, input.SkipCount, input.MaxResultCount);
    }

    // Urun index'ini PostgreSQL'deki guncel veriden bastan kurar.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<long> ReindexAsync()
    {
        return await _manager.ReindexAsync();
    }
}
