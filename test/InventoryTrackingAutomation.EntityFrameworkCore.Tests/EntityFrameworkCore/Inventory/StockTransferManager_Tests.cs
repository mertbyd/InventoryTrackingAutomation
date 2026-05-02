using InventoryTrackingAutomation.EntityFrameworkCore;
using InventoryTrackingAutomation.Inventory; // Domain.Tests katmanındaki abstract sınıfa erişim

namespace InventoryTrackingAutomation.EntityFrameworkCore.Inventory;

/*
 * KURAL: Domain.Tests katmanındaki abstract sınıf, burada (EF Core'da) miras alınır.
 * 
 * Neden? Domain sınıfı iş kuralını (Yetersiz stok hatası) yazdı. Ama o kuralı test edebilmek için 
 * verileri bir yere yazıp okumamız (Repository) lazım. SQLite in-memory sadece bu katmanda var.
 */
public class StockTransferManager_Tests : InventoryTrackingAutomation.Inventory.StockTransferManager_Tests<InventoryTrackingAutomationEntityFrameworkCoreTestModule>
{
    // Test içeriği boş bırakılır. Bütün senaryolar üst sınıftan çalıştırılır.
}
