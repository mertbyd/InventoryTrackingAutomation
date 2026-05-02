using System;
using System.Threading.Tasks;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.FluentValidation.Tasks;
using Xunit;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Validations.Tasks;

/*
 * FLUENT VALIDATION TESTLERİ — Envanter görevi DTO'ları için.
 *
 * CreateInventoryTaskDto validasyonunu test eder:
 * - Code ve Name zorunlu mu?
 * - Type ve Status geçerli enum değerleri mi?
 * - Maksimum karakter sınırları aşılmış mı?
 */
public abstract class InventoryTaskValidation_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    /*
     * SENARYO 1: Tüm alanları geçerli bir görev DTO'su, hatasız geçmelidir.
     */
    [Fact]
    public async Task CreateInventoryTask_Valid_Should_Pass()
    {
        var validator = new CreateInventoryTaskDtoValidator();

        var dto = new CreateInventoryTaskDto
        {
            Code = "TSK-001",
            Name = "Saha Operasyonu",
            Type = Enums.Tasks.InventoryTaskTypeEnum.FieldOperation,
            Status = Enums.Tasks.TaskStatusEnum.Draft,
            SourceWarehouseId = Guid.NewGuid(),
            Region = "Kadıköy Bölgesi",
            Description = "Test sahası kontrolü"
        };

        var result = await validator.ValidateAsync(dto);
        Assert.True(result.IsValid);
    }

    /*
     * SENARYO 2: Code alanı boşsa HATA vermeli.
     */
    [Fact]
    public async Task CreateInventoryTask_EmptyCode_Should_Fail()
    {
        var validator = new CreateInventoryTaskDtoValidator();

        var dto = new CreateInventoryTaskDto
        {
            Code = "", // BOŞ — HATA!
            Name = "Test Task",
            Type = Enums.Tasks.InventoryTaskTypeEnum.FieldOperation,
            Status = Enums.Tasks.TaskStatusEnum.Draft
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Code");
    }

    /*
     * SENARYO 3: Name alanı boşsa HATA vermeli.
     */
    [Fact]
    public async Task CreateInventoryTask_EmptyName_Should_Fail()
    {
        var validator = new CreateInventoryTaskDtoValidator();

        var dto = new CreateInventoryTaskDto
        {
            Code = "TSK-001",
            Name = "", // BOŞ — HATA!
            Type = Enums.Tasks.InventoryTaskTypeEnum.WarehouseTransfer,
            Status = Enums.Tasks.TaskStatusEnum.Draft
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    /*
     * SENARYO 4: Type alanına geçersiz bir enum değeri gönderildiğinde HATA vermeli.
     * Kural: IsInEnum()
     */
    [Fact]
    public async Task CreateInventoryTask_InvalidType_Should_Fail()
    {
        var validator = new CreateInventoryTaskDtoValidator();

        var dto = new CreateInventoryTaskDto
        {
            Code = "TSK-001",
            Name = "Test Task",
            Type = (Enums.Tasks.InventoryTaskTypeEnum)999, // GEÇERSİZ ENUM — HATA!
            Status = Enums.Tasks.TaskStatusEnum.Draft
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Type");
    }
}
