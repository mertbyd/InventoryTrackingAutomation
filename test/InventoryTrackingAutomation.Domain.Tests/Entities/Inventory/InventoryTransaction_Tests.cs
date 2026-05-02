using System;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Enums.Inventory;
using Shouldly;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Inventory;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Inventory/
 * ACIKLAMA: 'InventoryTransaction' sistemdeki tum stok hareketlerinin degismez (immutable) kaydidir.
 */
public class InventoryTransaction_Tests
{
    [Fact]
    public void Should_Initialize_Transaction()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var movementId = Guid.NewGuid();
        var sourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();

        // ACT
        var transaction = new InventoryTransaction(id)
        {
            ProductId = productId,
            TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
            Quantity = 50,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = sourceId,
            TargetLocationType = StockLocationTypeEnum.Vehicle,
            TargetLocationId = targetId,
            RelatedMovementRequestId = movementId,
            OccurredAt = DateTime.UtcNow
        };

        // ASSERT
        transaction.Id.ShouldBe(id);
        transaction.ProductId.ShouldBe(productId);
        transaction.Quantity.ShouldBe(50);
        transaction.RelatedMovementRequestId.ShouldBe(movementId);
    }
}
