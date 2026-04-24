using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Ürün domain manager'ı — Product entity'si için iş kuralları ve validasyonları.
/// </summary>
public class ProductManager : BaseManager<Product>
{
    private readonly IProductCategoryRepository _categoryRepository;  // CategoryId FK validasyonu için

    /// <summary>
    /// ProductManager constructor'ı.
    /// </summary>
    public ProductManager(
        IProductRepository repository,
        IProductCategoryRepository categoryRepository)
        : base(repository)
    {
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Yeni ürün oluşturur — Code unique ve CategoryId varlık kontrolü yapar.
    /// </summary>
    public async Task<Product> CreateAsync(CreateProductModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                InventoryTrackingAutomationDomainErrorCodes.Products.CodeNotUnique);
        }

        if (model.CategoryId.HasValue)
        {
            await EnsureExistsInAsync(
                _categoryRepository,
                model.CategoryId.Value,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        }

        await EnsureValidEnumAsync(model.BaseUnit, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedUnitTypes);

        return MapAndAssignId(model);
    }

    /// <summary>
    /// Ürünü günceller — Code unique (self hariç) ve CategoryId varlık kontrolü yapar.
    /// </summary>
    public async Task<Product> UpdateAsync(Product existing, UpdateProductModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.Products.CodeNotUnique);
        }

        if (model.CategoryId.HasValue && existing.CategoryId != model.CategoryId)
        {
            await EnsureExistsInAsync(
                _categoryRepository,
                model.CategoryId.Value,
                InventoryTrackingAutomationDomainErrorCodes.ProductCategories.NotFound);
        }

        await EnsureValidEnumAsync(model.BaseUnit, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedUnitTypes);

        return MapForUpdate(model, existing);
    }
}
