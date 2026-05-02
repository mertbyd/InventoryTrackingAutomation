using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;
using Xunit;

namespace InventoryTrackingAutomation.Inventory;

public abstract class StockTransferManager_Tests<TStartupModule> : InventoryTrackingAutomationDomainTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly StockTransferManager _stockTransferManager;
    private readonly IStockLocationRepository _stockLocationRepository;
    private readonly IRepository<Product, Guid> _productRepository;

    protected StockTransferManager_Tests()
    {
        _stockTransferManager = GetRequiredService<StockTransferManager>();
        _stockLocationRepository = GetRequiredService<IStockLocationRepository>();
        _productRepository = GetRequiredService<IRepository<Product, Guid>>();
    }

    [Fact]
    public async Task Should_Throw_Exception_When_Source_Stock_Is_Insufficient()
    {
        await WithUnitOfWorkAsync(async () =>
        {
            var product = await _productRepository.InsertAsync(new Product(Guid.NewGuid())
            {
                Code = $"STK-PRD-{Guid.NewGuid():N}"[..30],
                Name = "Stok transfer test urunu",
                BaseUnit = UnitTypeEnum.Piece,
                IsActive = true,
                IsSerializable = false
            }, autoSave: true);

            var sourceWarehouseId = Guid.NewGuid();
            var targetVehicleId = Guid.NewGuid();

            await _stockLocationRepository.InsertAsync(new StockLocation(Guid.NewGuid())
            {
                ProductId = product.Id,
                LocationType = StockLocationTypeEnum.Warehouse,
                LocationId = sourceWarehouseId,
                Quantity = 5,
                ReservedQuantity = 0
            }, autoSave: true);

            var transferModel = new StockTransferModel
            {
                ProductId = product.Id,
                Quantity = 10,
                SourceLocationType = StockLocationTypeEnum.Warehouse,
                SourceLocationId = sourceWarehouseId,
                DestinationLocationType = StockLocationTypeEnum.Vehicle,
                DestinationLocationId = targetVehicleId,
                TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
                PerformedByUserId = Guid.NewGuid()
            };

            var exception = await Assert.ThrowsAsync<BusinessException>(async () =>
            {
                await _stockTransferManager.ExecuteAsync(transferModel);
            });

            Assert.Equal(InventoryTrackingAutomationErrorCodes.StockLocations.InsufficientStock, exception.Code);
        });
    }
}
