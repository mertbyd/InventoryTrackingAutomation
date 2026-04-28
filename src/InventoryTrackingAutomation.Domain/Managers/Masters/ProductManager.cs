using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Ürün domain manager'ı — Product entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Product etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class ProductManager : BaseManager<Product>
{
    private readonly IProductCategoryRepository _categoryRepository;  // CategoryId FK validasyonu için

    /// <summary>
    /// ProductManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public ProductManager(
        IProductRepository repository,
        IProductCategoryRepository categoryRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _categoryRepository = categoryRepository;
    }

    /// <summary>
    /// Yeni ürün oluşturur — Code unique ve CategoryId varlık kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Product> CreateAsync(CreateProductModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code);
        }

        if (model.CategoryId.HasValue)
        {
            await EnsureExistsInAsync(
                _categoryRepository,
                model.CategoryId.Value);
        }

        await EnsureValidEnumAsync(model.BaseUnit, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedUnitTypes);

        var entity = new Product(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Ürünü günceller — Code unique (self hariç) ve CategoryId varlık kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Product> UpdateAsync(Product existing, UpdateProductModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }

        if (model.CategoryId.HasValue && existing.CategoryId != model.CategoryId)
        {
            await EnsureExistsInAsync(
                _categoryRepository,
                model.CategoryId.Value);
        }

        await EnsureValidEnumAsync(model.BaseUnit, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Masters.AllowedUnitTypes);

        _mapper.Map(model, existing);
        return existing;
    }
}

