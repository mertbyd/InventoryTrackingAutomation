using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

/// <summary>
/// Ürün kategorisi domain manager'ı — ProductCategory entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: ProductCategory etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class ProductCategoryManager : BaseManager<ProductCategory>
{
    /// <summary>
    /// ProductCategoryManager constructor'ı.
    /// </summary>

    public ProductCategoryManager(IProductCategoryRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni ürün kategorisi oluşturur — Code unique ve ParentId varlık kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<CreateProductCategoryModel> CreateAsync(CreateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(x => x.Code == model.Code);
        }
        return model;
    }

    /// <summary>
    /// Ürün kategorisini günceller — Code unique (self hariç) ve ParentId varlık kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<UpdateProductCategoryModel> UpdateAsync(ProductCategory existing, UpdateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }
        if (model.ParentId.HasValue && existing.ParentId != model.ParentId)
        {
            await EnsureExistsAsync(model.ParentId.Value);
        }
        return model;
    }
}
