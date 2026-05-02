using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.FluentValidation.Movements;
using Xunit;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Validations.Movements;

/*
 * MovementRequest artik satir tasimaz.
 * API sadece hazir VehicleTaskId alir; TaskLine ve VehicleTaskLine kendi servislerinde dogrulanir.
 */
public abstract class MovementRequestValidation_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    [Fact]
    public async Task CreateMovementRequest_Valid_Should_Pass()
    {
        var validator = new CreateMovementRequestDtoValidator();

        var dto = new CreateMovementRequestDto
        {
            RequestNumber = "MR-001",
            RequestNote = "Arac-gorev atamasi icin hareket talebi",
            Priority = Enums.MovementPriorityEnum.Normal,
            VehicleTaskId = Guid.NewGuid(),
            PlannedDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await validator.ValidateAsync(dto);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateMovementRequest_EmptyVehicleTaskId_Should_Fail()
    {
        var validator = new CreateMovementRequestDtoValidator();

        var dto = new CreateMovementRequestDto
        {
            RequestNote = "Test",
            Priority = Enums.MovementPriorityEnum.Normal,
            VehicleTaskId = Guid.Empty,
            PlannedDate = DateTime.UtcNow.AddDays(1)
        };

        var result = await validator.ValidateAsync(dto);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateMovementRequestDto.VehicleTaskId));
    }
}
