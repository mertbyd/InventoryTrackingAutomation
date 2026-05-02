using InventoryTrackingAutomation.EntityFrameworkCore;
using InventoryTrackingAutomation.Mappings.Movements;

namespace InventoryTrackingAutomation.EntityFrameworkCore.Mappings.Movements;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/EntityFrameworkCore/Mappings/Movements/
 * ACIKLAMA: Soyut (abstract) olarak tanimlanmis MovementRequestMapping_Tests sinifini 
 * EF Core altyapisi ile calistirilabilir hale getirir.
 */
public class EfCoreMovementRequestMapping_Tests : MovementRequestMapping_Tests<InventoryTrackingAutomationEntityFrameworkCoreTestModule>
{
}
