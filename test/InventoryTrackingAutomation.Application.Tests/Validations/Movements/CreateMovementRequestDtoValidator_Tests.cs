using System;
using FluentValidation.TestHelper;
using InventoryTrackingAutomation.Dtos.Movements;
using Xunit;

namespace InventoryTrackingAutomation.FluentValidation.Movements;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Application.Tests/Validations/Movements/
 * ACIKLAMA: 'CreateMovementRequestDtoValidator' sınıfının giriş doğruluğunu test eder.
 * NEDEN BURADA?: FluentValidation kuralları Application katmanında DTO düzeyinde kontrol edilir.
 */
public class CreateMovementRequestDtoValidator_Tests
{
    private readonly CreateMovementRequestDtoValidator _validator;

    public CreateMovementRequestDtoValidator_Tests()
    {
        _validator = new CreateMovementRequestDtoValidator();
    }

    [Fact]
    public void Should_Have_Error_When_RequestNote_Is_Empty()
    {
        // ARRANGE
        var model = new CreateMovementRequestDto { RequestNote = "" };

        // ACT & ASSERT
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.RequestNote);
    }

    [Fact]
    public void Should_Have_Error_When_VehicleTaskId_Is_Empty()
    {
        // ARRANGE
        var model = new CreateMovementRequestDto { VehicleTaskId = Guid.Empty };

        // ACT & ASSERT
        var result = _validator.TestValidate(model);
        result.ShouldHaveValidationErrorFor(x => x.VehicleTaskId)
              .WithErrorCode(InventoryTrackingAutomation.ExceptionCodes.MovementRequestExceptionCodes.ValidationExceptions.VehicleTaskId.CannotEmpty);
    }

    [Fact]
    public void Should_Not_Have_Error_When_Valid()
    {
        // ARRANGE
        var model = new CreateMovementRequestDto
        {
            RequestNumber = "REQ-001",
            RequestNote = "Valid note",
            VehicleTaskId = Guid.NewGuid(),
            Priority = Enums.MovementPriorityEnum.Normal
        };

        // ACT & ASSERT
        var result = _validator.TestValidate(model);
        result.ShouldNotHaveAnyValidationErrors();
    }
}
