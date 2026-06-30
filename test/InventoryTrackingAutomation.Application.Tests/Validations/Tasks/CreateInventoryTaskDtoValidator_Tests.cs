using FluentValidation.TestHelper;
using System;
using InventoryTrackingAutomation.Dtos.Tasks;
using Xunit;

namespace InventoryTrackingAutomation.FluentValidation.Tasks;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Application.Tests/Validations/Tasks/
 * ACIKLAMA: 'CreateInventoryTaskDtoValidator' sınıfını test eder.
 * NEDEN BURADA?: Yeni bir operasyon görevi oluşturulurken giriş verilerinin doğruluğunu (zorunlu alanlar, uzunluklar) kontrol etmek kritik olduğu için Application katmanında test edilir.
 */
public class CreateInventoryTaskDtoValidator_Tests
{
    private readonly CreateInventoryTaskDtoValidator _validator;

    public CreateInventoryTaskDtoValidator_Tests()
    {
        _validator = new CreateInventoryTaskDtoValidator();
    }

    /*
     * SENARYO: Görev kodu (Code) boş bırakılamaz.
     */
    [Fact]
    public void Should_Have_Error_When_Code_Is_Empty()
    {
        // ARRANGE
        var model = new CreateInventoryTaskDto { Code = "" };

        // ACT & ASSERT
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.Code);
    }

    /*
     * SENARYO: Geçerli tüm alanlar doldurulduğunda validasyon hatası oluşmamalıdır.
     */
    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        // ARRANGE
        var model = new CreateInventoryTaskDto
        {
            Code = "T-001",
            Name = "Saha Montaj Görevi",
            Type = Enums.Tasks.InventoryTaskTypeEnum.WarehouseTransfer,
            Status = Enums.Tasks.TaskStatusEnum.Draft,
            SourceWarehouseId = Guid.NewGuid(),
            TargetWarehouseId = Guid.NewGuid()
        };

        // ACT & ASSERT
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
