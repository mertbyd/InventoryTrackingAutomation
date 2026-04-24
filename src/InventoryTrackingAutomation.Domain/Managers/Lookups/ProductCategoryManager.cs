using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.Managers.Lookups;

/// <summary>
/// Ürün kategorisi domain manager'ı — ProductCategory entity'si için iş kuralları ve validasyonları.
/// </summary>
public class ProductCategoryManager : BaseManager<ProductCategory>
{
    /// <summary>
    /// ProductCategoryManager constructor'ı.
    /// </summary>
    public ProductCategoryManager(
        IProductCategoryRepository repository)
        : base(repository)
    {
    }

    /// <summary>
    /// Yeni ürün kategorisi oluşturur — Code unique ve ParentId varlık kontrolü yapar.
    /// </summary>
    public async Task<ProductCategory> CreateAsync(CreateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.CodeNotUnique);
        }
        if (model.ParentId.HasValue)
        {
            await EnsureExistsInAsync(
                Repository,
                model.ParentId.Value,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        }
        return MapAndAssignId(model);
    }

    /// <summary>
    /// Ürün kategorisini günceller — Code unique (self hariç) ve ParentId varlık kontrolü yapar.
    /// </summary>
    public async Task<ProductCategory> UpdateAsync(ProductCategory existing, UpdateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.CodeNotUnique);
        }
        if (model.ParentId.HasValue && existing.ParentId != model.ParentId)
        {
            await EnsureExistsInAsync(
                Repository,
                model.ParentId.Value,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        }
        return MapForUpdate(model, existing);
    }
}
