using InventoryTrackingAutomation.EntityFrameworkCore;
using InventoryTrackingAutomation.Movements; // Application.Tests katmanındaki abstract sınıfa erişim

namespace InventoryTrackingAutomation.EntityFrameworkCore.Movements;

/*
 * ABP KURALI 3: Tüm abstract (soyut) testler, EntityFrameworkCore katmanında miras alınır (Inherit).
 * Bu sınıf "concrete" (somut) bir sınıftır ve test çalıştırıcısı (Test Runner) 
 * sadece bu sınıftaki testleri "Run" edebilir.
 * 
 * Neden? Çünkü bu katman SQLite In-Memory altyapısına sahiptir. 
 * Application katmanında yazdığımız kurallar, burada EF Core üzerinden gerçek bir DB simülasyonunda koşturulur.
 */
public class MovementRequestAppService_Tests : InventoryTrackingAutomation.Movements.MovementRequestAppService_Tests<InventoryTrackingAutomationEntityFrameworkCoreTestModule>
{
    // İçine ekstra kod yazmamıza gerek yoktur! 
    // Tüm [Fact] metotları üst sınıftan (Application.Tests) miras gelecektir.
    // Tek amacı, testlerin EF Core bağlamında (context) ve SQLite üzerinde çalışmasını sağlamaktır.
}
