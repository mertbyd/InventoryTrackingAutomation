using System;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
using Shouldly;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Movements;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Movements/
 * ACIKLAMA: 'MovementRequest' stok hareket taleplerinin temelidir.
 * NEDEN BURADA?: Talebin temel durumlarini ve iliskilerini dogrulamak icin buradadir.
 */
public class MovementRequest_Tests
{
    /*
     * SENARYO: Hareket talebi dogru alanlarla ilklendirilmelidir.
     */
    [Fact]
    public void Should_Initialize_Movement_Request()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var requestId = "REQ-123";
        var vehicleTaskId = Guid.NewGuid();

        // ACT
        var request = new MovementRequest(id)
        {
            RequestNumber = requestId,
            VehicleTaskId = vehicleTaskId,
            Status = MovementStatusEnum.Pending,
            Priority = MovementPriorityEnum.High,
            RequestNote = "Test notu",
            PlannedDate = DateTime.UtcNow.AddDays(1)
        };

        // ASSERT
        request.Id.ShouldBe(id);
        request.RequestNumber.ShouldBe(requestId);
        request.VehicleTaskId.ShouldBe(vehicleTaskId);
        request.Status.ShouldBe(MovementStatusEnum.Pending);
        request.Priority.ShouldBe(MovementPriorityEnum.High);
    }

    /*
     * SENARYO: Iade (Return) hareketi ise ParentMovementRequestId dolu olmalidir.
     */
    [Fact]
    public void Should_Be_A_Return_Request_If_ParentId_Is_Set()
    {
        // ARRANGE
        var request = new MovementRequest(Guid.NewGuid());
        var parentId = Guid.NewGuid();

        // ACT
        request.ParentMovementRequestId = parentId;

        // ASSERT
        request.ParentMovementRequestId.ShouldBe(parentId);
        // Not: IsReturnFlow bir property ise o da test edilmeli. 
        // Ama su an entity sadece POCO oldugu icin basit set/get testi yapiyoruz.
    }
}
