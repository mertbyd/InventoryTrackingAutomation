using System.Threading.Tasks;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.FluentValidation.Masters;
using Xunit;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Validations.Masters;

/*
 * FLUENT VALIDATION TESTLERİ — Application.Tests katmanında abstract yazılır.
 * 
 * NEDEN BURADA? FluentValidation kuralları DTO'ları (dışarıdan gelen verileri) doğrular.
 * DTO'lar Application.Contracts katmanında yaşar, doğrulama da Application katmanı sorumluluğundadır.
 * 
 * Bu testler validator sınıfını doğrudan new'leyerek çalıştırır.
 * Veritabanına erişim gerekmez (Pure Unit Test). Yine de ABP standartlarına uygun olarak
 * abstract yazılır ve EF Core katmanında somutlaştırılır.
 */
public abstract class WarehouseValidation_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    // ───────────────────────────────────────────────
    //  CreateWarehouseDto Testleri
    // ───────────────────────────────────────────────

    /*
     * SENARYO 1: Tüm alanları geçerli olan bir DTO, validation'dan hatasız geçmelidir.
     * Bu "pozitif test" — doğru veri gönderince sistemin sorunsuz çalıştığını kanıtlar.
     */
    [Fact]
    public async Task CreateWarehouse_Valid_Should_Pass()
    {
        // Validator sınıfını oluştur (DI'ye gerek yok, direkt new'lenebilir).
        var validator = new CreateWarehouseDtoValidator();

        // Geçerli bir DTO oluştur — tüm zorunlu alanlar dolu.
        var dto = new CreateWarehouseDto
        {
            Code = "WH-001",
            Name = "Ana Depo",
            Address = "İstanbul, Türkiye",
            IsActive = true
        };

        // Validation çalıştır ve sonuçta HATA OLMADIĞINI doğrula.
        var result = await validator.ValidateAsync(dto);
        Assert.True(result.IsValid);
    }

    /*
     * SENARYO 2: Code alanı boş bırakıldığında validation HATA vermeli.
     * Bu "negatif test" — yanlış veri gönderince sistemin hata fırlatmasını kanıtlar.
     */
    [Fact]
    public async Task CreateWarehouse_EmptyCode_Should_Fail()
    {
        var validator = new CreateWarehouseDtoValidator();

        // KRİTİK: Code alanı boş bırakıldı!
        var dto = new CreateWarehouseDto
        {
            Code = "", // HATA: Boş olamaz.
            Name = "Ana Depo",
            IsActive = true
        };

        var result = await validator.ValidateAsync(dto);

        // Validation sonucunda en az 1 hata olmalı.
        Assert.False(result.IsValid);
        // Hata "Code" alanıyla ilgili olmalı.
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }

    /*
     * SENARYO 3: Name alanı boş bırakıldığında validation HATA vermeli.
     */
    [Fact]
    public async Task CreateWarehouse_EmptyName_Should_Fail()
    {
        var validator = new CreateWarehouseDtoValidator();

        var dto = new CreateWarehouseDto
        {
            Code = "WH-001",
            Name = "", // HATA: Boş olamaz.
            IsActive = true
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    /*
     * SENARYO 4: Code alanı 50 karakterden uzun olduğunda validation HATA vermeli.
     * MaximumLength(50) kuralı var.
     */
    [Fact]
    public async Task CreateWarehouse_CodeTooLong_Should_Fail()
    {
        var validator = new CreateWarehouseDtoValidator();

        var dto = new CreateWarehouseDto
        {
            Code = new string('X', 51), // 51 karakter — sınırı aşıyor!
            Name = "Ana Depo",
            IsActive = true
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }
}
