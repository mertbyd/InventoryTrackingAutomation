using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Managers.Masters;

/// <summary>
/// Ürün domain manager'ı — Product entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Product etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class ProductManager : BaseManager<Product>
{
    private IProductCategoryRepository _categoryRepository => LazyGetRequiredService<IProductCategoryRepository>();  // CategoryId FK validasyonu için
    private IRepository<UnitType, System.Guid> _unitTypeRepository => LazyGetRequiredService<IRepository<UnitType, System.Guid>>();

    /// <summary>
    /// ProductManager constructor'ı.
    /// </summary>
    public ProductManager(IProductRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni ürün oluşturur — Code unique ve CategoryId varlık kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<CreateProductModel> CreateAsync(CreateProductModel model)
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

        // islevi: Enum yerine gelen UnitType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_unitTypeRepository, model.UnitTypeId);

        return model;
    }

    /// <summary>
    /// Ürünü günceller — Code unique (self hariç) ve CategoryId varlık kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<UpdateProductModel> UpdateAsync(Product existing, UpdateProductModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }

        if (model.CategoryId.HasValue)
        {
            await EnsureExistsInAsync(
                _categoryRepository,
                model.CategoryId.Value);
        }

        // islevi: Enum yerine gelen UnitType lookup kaydinin DB'de var oldugunu dogrular.
        await EnsureExistsInAsync(_unitTypeRepository, model.UnitTypeId);

        return model;
    }

    /// <summary>
    /// Urunu operasyon gecmisi bozulmadan pasife almak icin kullanilir.
    /// </summary>
    // islevi: Master veriyi silmeden kullanim disi birakir.
    // sistemdeki gorevi: Product FK gecmisini koruyarak soft-delete kolonlarina olan ihtiyaci kaldirir.
    public Task<Product> PassivateAsync(Product existing)
    {
        existing.IsActive = false;
        return Task.FromResult(existing);
    }
}

