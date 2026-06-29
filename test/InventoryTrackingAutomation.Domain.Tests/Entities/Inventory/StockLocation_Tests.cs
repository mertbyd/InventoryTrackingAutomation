using System;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Inventory;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Inventory/
 * ACIKLAMA: 'StockLocation' mevcut stok bakiyesidir; guncellenir ama soft-delete ile silinmez.
 */
public class StockLocation_Tests
{
    [Fact]
    public void Should_Initialize_Stock_Location()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var locationId = Guid.NewGuid();

        // ACT
        var stockLocation = new StockLocation(id)
        {
            ProductId = productId,
            LocationType = StockLocationTypeEnum.Warehouse,
            LocationId = locationId,
            Quantity = 20,
            ReservedQuantity = 5
        };

        // ASSERT
        stockLocation.Id.ShouldBe(id);
        stockLocation.ProductId.ShouldBe(productId);
        stockLocation.LocationId.ShouldBe(locationId);
        stockLocation.Quantity.ShouldBe(20);
        stockLocation.ReservedQuantity.ShouldBe(5);
    }

    [Fact]
    public void Should_Use_Update_Audit_Without_Soft_Delete()
    {
        // ARRANGE
        var stockLocation = new StockLocation(Guid.NewGuid());

        // ACT
        var stockLocationType = stockLocation.GetType();

        // ASSERT
        // islevi: Stok bakiyesinin update audit tasidigini ama soft-delete tasimadigini dogrular.
        // sistemdeki gorevi: StockLocation'in yanlislikla tekrar FullAudited/SoftDelete yapilmasini engeller.
        typeof(AuditedEntity<Guid>).IsAssignableFrom(stockLocationType).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(stockLocationType).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(stockLocationType).ShouldBeFalse();
    }
}
