using InventoryTrackingAutomation.Application.Mappers.Movements;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using Shouldly;
using Xunit;
using System;

namespace InventoryTrackingAutomation.Mappings.Movements;

public abstract class MovementRequestMapping_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly MovementRequestMapper _mapper;

    protected MovementRequestMapping_Tests()
    {
        _mapper = new MovementRequestMapper();
    }

    [Fact]
    public void Should_Map_MovementRequest_To_Dto()
    {
        // ARRANGE
        var entity = new MovementRequest(Guid.NewGuid())
        {
            RequestNumber = "REQ-456",
            Status = Enums.MovementStatusEnum.Shipped,
            Priority = Enums.MovementPriorityEnum.Normal
        };

        // ACT
        var dto = _mapper.MapToDto(entity);

        // ASSERT
        dto.Id.ShouldBe(entity.Id);
        dto.RequestNumber.ShouldBe(entity.RequestNumber);
        dto.Status.ShouldBe(entity.Status);
    }

    [Fact]
    public void Should_Map_CreateDto_To_Model()
    {
        // ARRANGE
        var dto = new CreateMovementRequestDto
        {
            RequestNumber = "NEW-REQ",
            VehicleTaskId = Guid.NewGuid(),
            RequestNote = "Initial note"
        };

        // ACT
        var model = _mapper.MapToModel(dto);

        // ASSERT
        model.RequestNumber.ShouldBe(dto.RequestNumber);
        model.VehicleTaskId.ShouldBe(dto.VehicleTaskId);
        model.RequestNote.ShouldBe(dto.RequestNote);
    }
}
