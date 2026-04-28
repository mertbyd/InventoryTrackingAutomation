using AutoMapper;
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
    private readonly IProductRepository _productRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public StockLocationManager(
        IStockLocationRepository repository,
        IProductRepository productRepository,
        IWarehouseRepository warehouseRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
        : base(repository)
    {
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    /// Yeni bir stok lokasyon kaydı oluşturmak için kullanılır.
    public async Task<StockLocation> CreateAsync(CreateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.LocationId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.LocationId, null);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        var entity = new StockLocation(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// Mevcut bir stok lokasyon kaydını güncellemek için kullanılır.
    public async Task<StockLocation> UpdateAsync(StockLocation existing, UpdateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.LocationId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.LocationId, existing.Id);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        _mapper.Map(model, existing);
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
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    /// Stok miktarlarını doğrulamak için kullanılır.
    private static void ValidateQuantities(int quantity, int reservedQuantity)
    {
        // Rezerve miktar toplam stoktan buyuk olamaz.
        if (quantity < 0 || reservedQuantity < 0 || reservedQuantity > quantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    /// Stok miktarını azaltmak için kullanılır.
    public async Task DecreaseAsync(StockLocationTypeEnum type, Guid locationId, Guid productId, int qty)
    {
        var stock = await ((IStockLocationRepository)Repository)
            .FindAsync(x => x.LocationType == type && x.LocationId == locationId && x.ProductId == productId);

        if (stock == null || stock.Quantity < qty)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.StockLocations.InsufficientStock)
                .WithData("LocationType", type).WithData("LocationId", locationId)
                .WithData("ProductId", productId).WithData("Requested", qty)
                .WithData("Available", stock?.Quantity ?? 0);
                
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
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }
}
