using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Permissions;
using InventoryTrackingAutomation.Services.Search;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Search;

/// <summary>
/// Ürün metin arama endpoint'leri (Elasticsearch okuma-tarafı).
/// </summary>
[Route("api/products/search")]
[ApiExplorerSettings(GroupName = "Masters")]
public class ProductSearchController : InventoryTrackingAutomationController
{
    public ProductSearchController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IProductSearchAppService _appService => LazyGetRequiredService<IProductSearchAppService>();

    /// <summary>
    /// Keyword'ü ürün adında fuzzy, ürün kodunda birebir arar; sayfalı sonuç döner.
    /// </summary>
    /// <param name="input">Arama ve sayfalama parametreleri.</param>
    /// <remarks>
    /// Request {
    ///   Keyword        (string?)      → Aranacak serbest metin; boş ise tüm kayıtlar sayfalı döner
    /// }
    /// Response {
    ///   Code           (string)       → Ürün kodu
    ///   Name           (string)       → Ürün adı
    ///   CategoryId     (Guid?)        → Bağlı kategori Id'si (Guid lookup Id)
    ///   CategoryName   (string?)      → Kategori adı (okuma-tarafı değer)
    ///   UnitTypeId     (Guid)         → Ölçü birimi lookup Id'si (Guid lookup Id)
    ///   UnitTypeName   (string?)      → Ölçü birimi adı (okuma-tarafı değer)
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<PagedResultDto<ProductIndexDto>>> Search([FromQuery] ProductSearchInputDto input)
    {
        var result = await _appService.SearchAsync(input);
        return result;
    }

    /// <summary>
    /// Ürün arama index'ini PostgreSQL'deki güncel veriden baştan kurar.
    /// </summary>
    /// <remarks>
    /// Response: index'lenen doküman sayısı.
    /// </remarks>
    [HttpPost("reindex")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<long>> Reindex()
    {
        var result = await _appService.ReindexAsync();
        return result;
    }
}
