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

    [Fact]
    public void Should_Use_Creation_Audit_Only()
    {
        // ARRANGE
        var transaction = new InventoryTransaction(Guid.NewGuid());

        // ACT
        var transactionType = transaction.GetType();

        // ASSERT
        // islevi: Ledger kaydinin sadece olusturma audit bilgisi tasidigini dogrular.
        // sistemdeki gorevi: InventoryTransaction'in yanlislikla tekrar FullAudited/SoftDelete yapilmasini engeller.
        typeof(CreationAuditedEntity<Guid>).IsAssignableFrom(transactionType).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(transactionType).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(transactionType).ShouldBeFalse();
    }
}
