using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Inventory;

/// <summary>
/// StockLocation domain manager'i - depo/arac bazli stok kurallarini yonetir.
/// </summary>
//işlevi: Fiziksel lokasyonlardaki (depo/araç) ürün stok bakiyelerini yönetir, validasyonları yapar.
//sistemdeki görevii: Stok kayıtlarının bütünlüğünü sağlar (negatif stok engelleme, lokasyon doğrulama vb.).
//işlevi: StockLocation etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class StockLocationManager : BaseManager<StockLocation>
{
    private IProductRepository _productRepository => LazyGetRequiredService<IProductRepository>();
    private IWarehouseRepository _warehouseRepository => LazyGetRequiredService<IWarehouseRepository>();
    private IVehicleRepository _vehicleRepository => LazyGetRequiredService<IVehicleRepository>();

    public StockLocationManager(IStockLocationRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    protected override string DeleteNotSupportedErrorCode => StockLocationExceptionCodes.DeleteNotSupported;

    /// Yeni bir stok lokasyon kaydı oluşturmak için kullanılır.
    public async Task<StockLocation> CreateAsync(CreateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.LocationId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.LocationId, null);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        var entity = new StockLocation(GuidGenerator.Create());
        return entity;
    }

    /// <summary>
    /// Birden fazla stok lokasyon kaydı oluşturmak için toplu validasyon yapar.
    /// </summary>
    public async Task<System.Collections.Generic.List<CreateStockLocationModel>> CreateManyAsync(System.Collections.Generic.List<CreateStockLocationModel> models)
    {
        var productIds = models.Select(x => x.ProductId).Distinct().ToList();
        if (productIds.Any()) await EnsureAllExistInAsync(_productRepository, productIds);

        var warehouseIds = models.Where(x => x.LocationType == StockLocationTypeEnum.Warehouse).Select(x => x.LocationId).Distinct().ToList();
        if (warehouseIds.Any()) await EnsureAllExistInAsync(_warehouseRepository, warehouseIds);

        var vehicleIds = models.Where(x => x.LocationType == StockLocationTypeEnum.Vehicle).Select(x => x.LocationId).Distinct().ToList();
        if (vehicleIds.Any()) await EnsureAllExistInAsync(_vehicleRepository, vehicleIds);

        var existingLocations = await ((IStockLocationRepository)Repository).GetListAsync(x => productIds.Contains(x.ProductId));
        
        foreach (var model in models)
        {
            ValidateQuantities(model.Quantity, model.ReservedQuantity);
            if (existingLocations.Any(x => x.ProductId == model.ProductId && x.LocationType == model.LocationType && x.LocationId == model.LocationId))
            {
                throw new BusinessException(StockLocationExceptionCodes.DuplicateLocation);
            }
        }
        
        var duplicateInInput = models.GroupBy(x => new { x.ProductId, x.LocationType, x.LocationId }).Any(g => g.Count() > 1);
        if (duplicateInInput) throw new BusinessException(StockLocationExceptionCodes.DuplicateLocation);

        return models;
    }

    /// Mevcut bir stok lokasyon kaydını güncellemek için kullanılır.
    public async Task<StockLocation> UpdateAsync(StockLocation existing, UpdateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.LocationId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.LocationId, existing.Id);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        return existing;
    }



    /// Lokasyon referanslarını doğrulamak için kullanılır.
    private async Task ValidateReferencesAsync(
        Guid productId,
        StockLocationTypeEnum locationType,
        Guid locationId)
    {
        // Urun ve lokasyon referanslari tek noktada dogrulanir.
        await EnsureExistsInAsync(_productRepository, productId);
        if (locationType == StockLocationTypeEnum.Warehouse)
        {
            await EnsureExistsInAsync<Warehouse>(_warehouseRepository, locationId);
            return;
        }

        await EnsureExistsInAsync<Vehicle>(_vehicleRepository, locationId);
    }

    /// Lokasyonun benzersizliğini doğrulamak için kullanılır.
    private async Task ValidateUniqueLocationAsync(
        Guid productId,
        StockLocationTypeEnum locationType,
        Guid locationId,
        Guid? excludeId)
    {
        // Ayni urun ayni fiziksel lokasyonda tek stok satiri ile tutulur.
        var existingLocations = await Repository.GetListAsync(x =>
            x.ProductId == productId &&
            x.LocationType == locationType &&
            x.LocationId == locationId);

        if (existingLocations.Any(x => !excludeId.HasValue || x.Id != excludeId.Value))
        {
            throw new BusinessException(StockLocationExceptionCodes.DuplicateLocation);
        }
    }

    /// Stok miktarlarını doğrulamak için kullanılır.
    private static void ValidateQuantities(int quantity, int reservedQuantity)
    {
        // Rezerve miktar toplam stoktan buyuk olamaz.
        if (quantity < 0 || reservedQuantity < 0 || reservedQuantity > quantity)
        {
            throw new BusinessException(StockLocationExceptionCodes.InvalidQuantity);
        }
    }

    /// Stok miktarını azaltmak için kullanılır.
    public async Task DecreaseAsync(StockLocationTypeEnum type, Guid locationId, Guid productId, int qty)
    {
        var stock = await ((IStockLocationRepository)Repository)
            .FindAsync(x => x.LocationType == type && x.LocationId == locationId && x.ProductId == productId);

        if (stock == null || stock.Quantity < qty)
            throw new BusinessException(StockLocationExceptionCodes.InsufficientStock);

        stock.Quantity -= qty;
        await Repository.UpdateAsync(stock, autoSave: true);
    }

    /// Stok miktarını artırmak için kullanılır.
    public async Task IncreaseAsync(StockLocationTypeEnum type, Guid locationId, Guid productId, int qty)
    {
        var stock = await ((IStockLocationRepository)Repository)
            .FindAsync(x => x.LocationType == type && x.LocationId == locationId && x.ProductId == productId);

        if (stock == null)
        {
            await EnsureLocationExistsAsync(type, locationId);
            stock = new StockLocation(GuidGenerator.Create())
            {
                LocationType = type,
                LocationId = locationId,
                ProductId = productId,
                Quantity = qty,
                ReservedQuantity = 0
            };
            await Repository.InsertAsync(stock, autoSave: true);
            return;
        }
        stock.Quantity += qty;
        await Repository.UpdateAsync(stock, autoSave: true);
    }

    /// Lokasyonun varlığını doğrulamak için kullanılır.
    private async Task EnsureLocationExistsAsync(StockLocationTypeEnum type, Guid locationId)
    {
        switch (type)
        {
            case StockLocationTypeEnum.Warehouse:
                await EnsureExistsInAsync(_warehouseRepository, locationId);
                break;
            case StockLocationTypeEnum.Vehicle:
                await EnsureExistsInAsync(_vehicleRepository, locationId);
                break;
            default:
                throw new BusinessException(StockLocationExceptionCodes.UnsupportedLocationType);
        }
    }
}

