using AutoMapper;
using System;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Models.Stock;
using Volo.Abp;

namespace InventoryTrackingAutomation.Managers.Stock;

/// <summary>
/// StockLocation domain manager'i - depo/arac bazli stok kurallarini yonetir.
/// </summary>
public class StockLocationManager : BaseManager<StockLocation>
{
    private readonly IProductRepository _productRepository;
    private readonly ISiteRepository _siteRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public StockLocationManager(
        IStockLocationRepository repository,
        IProductRepository productRepository,
        ISiteRepository siteRepository,
        IVehicleRepository vehicleRepository,
        IMapper mapper)
        : base(repository)
    {
        _productRepository = productRepository;
        _siteRepository = siteRepository;
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<StockLocation> CreateAsync(CreateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.WarehouseSiteId, model.VehicleId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.WarehouseSiteId, model.VehicleId, null);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        var entity = new StockLocation(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    public async Task<StockLocation> UpdateAsync(StockLocation existing, UpdateStockLocationModel model)
    {
        await ValidateReferencesAsync(model.ProductId, model.LocationType, model.WarehouseSiteId, model.VehicleId);
        await ValidateUniqueLocationAsync(model.ProductId, model.LocationType, model.WarehouseSiteId, model.VehicleId, existing.Id);
        ValidateQuantities(model.Quantity, model.ReservedQuantity);

        _mapper.Map(model, existing);
        return existing;
    }

    private async Task ValidateReferencesAsync(
        Guid productId,
        InventoryLocationTypeEnum locationType,
        Guid? warehouseSiteId,
        Guid? vehicleId)
    {
        // Urun ve lokasyon referanslari tek noktada dogrulanir.
        await EnsureExistsInAsync(_productRepository, productId);
        if (locationType == InventoryLocationTypeEnum.Warehouse)
        {
            if (!warehouseSiteId.HasValue || vehicleId.HasValue)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.StockLocations.InvalidLocation);
            }

            await EnsureExistsInAsync<Site>(_siteRepository, warehouseSiteId.Value);
            return;
        }

        if (!vehicleId.HasValue || warehouseSiteId.HasValue)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.StockLocations.InvalidLocation);
        }

        await EnsureExistsInAsync<Vehicle>(_vehicleRepository, vehicleId.Value);
    }

    private async Task ValidateUniqueLocationAsync(
        Guid productId,
        InventoryLocationTypeEnum locationType,
        Guid? warehouseSiteId,
        Guid? vehicleId,
        Guid? excludeId)
    {
        // Ayni urun ayni fiziksel lokasyonda tek stok satiri ile tutulur.
        var existingLocations = await Repository.GetListAsync(x =>
            x.ProductId == productId &&
            x.LocationType == locationType &&
            x.WarehouseSiteId == warehouseSiteId &&
            x.VehicleId == vehicleId);

        if (existingLocations.Any(x => !excludeId.HasValue || x.Id != excludeId.Value))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }

    private static void ValidateQuantities(int quantity, int reservedQuantity)
    {
        // Rezerve miktar toplam stoktan buyuk olamaz.
        if (quantity < 0 || reservedQuantity < 0 || reservedQuantity > quantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation);
        }
    }
}
