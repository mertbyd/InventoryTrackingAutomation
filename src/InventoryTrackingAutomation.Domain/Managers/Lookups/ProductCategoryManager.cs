using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

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
    private readonly IMapper _mapper;
    public ProductCategoryManager(
        IProductCategoryRepository repository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Yeni ürün kategorisi oluşturur — Code unique ve ParentId varlık kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<ProductCategory> CreateAsync(CreateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code);
        }
        if (model.ParentId.HasValue)
        {
            await EnsureExistsInAsync(
                Repository,
                model.ParentId.Value);
        }
        var entity = new ProductCategory(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Ürün kategorisini günceller — Code unique (self hariç) ve ParentId varlık kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<ProductCategory> UpdateAsync(ProductCategory existing, UpdateProductCategoryModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }
        if (model.ParentId.HasValue && existing.ParentId != model.ParentId)
        {
            await EnsureExistsInAsync(
                Repository,
                model.ParentId.Value);
        }
        _mapper.Map(model, existing);
        return existing;
    }
}

