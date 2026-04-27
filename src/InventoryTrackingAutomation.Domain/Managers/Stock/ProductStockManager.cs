using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Models.Stock;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// Ürün stok domain manager'ı — ProductStock entity'si için iş kuralları ve validasyonları.
/// </summary>
public class ProductStockManager : BaseManager<ProductStock>
{
    private readonly IProductRepository _productRepository;  // ProductId FK validasyonu için
    private readonly ISiteRepository _siteRepository;        // SiteId FK validasyonu için

    /// <summary>
    /// ProductStockManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public ProductStockManager(
        IProductStockRepository repository,
        IProductRepository productRepository,
        ISiteRepository siteRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _productRepository = productRepository;
        _siteRepository = siteRepository;
    }

    /// <summary>
    /// Yeni ürün stok kaydı oluşturur — ProductId ve SiteId varlık kontrolü, (ProductId, SiteId) unique kontrolü yapar.
    /// </summary>
    public async Task<ProductStock> CreateAsync(CreateProductStockModel model)
    {
        await EnsureExistsInAsync(
            _productRepository,
            model.ProductId);

        await EnsureExistsInAsync(
            _siteRepository,
            model.SiteId);

        await EnsureUniqueAsync(
            x => x.ProductId == model.ProductId && x.SiteId == model.SiteId);

        var entity = new ProductStock(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Ürün stok kaydını günceller — ProductId ve SiteId varlık kontrolleri yapar.
    /// </summary>
    public async Task<ProductStock> UpdateAsync(ProductStock existing, UpdateProductStockModel model)
    {
        if (existing.ProductId != model.ProductId)
        {
            await EnsureExistsInAsync(
                _productRepository,
                model.ProductId);
        }

        if (existing.SiteId != model.SiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.SiteId);
        }

        _mapper.Map(model, existing);
        return existing;
    }
}

