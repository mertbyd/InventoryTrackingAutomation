using System;
using InventoryTrackingAutomation.Entities.Tasks;
using Shouldly;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Tasks;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Tasks/
 * ACIKLAMA: 'VehicleTask' entity'si arac-gorev atamasini temsil eder.
 * NEDEN BURADA?: Atama mantiginin (orn: ReleasedAt kontrolu) dogru calistigindan emin olmak icin Domain.Tests altindadir.
 */
public class VehicleTask_Tests
{
    /*
     * SENARYO: Arac atamasi yapildiginda baslangic zamani (AssignedAt) set edilmis olmalidir.
     */
    [Fact]
    public void Should_Set_Assignment_Details()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var vehicleId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        var workerId = Guid.NewGuid();
        var assignedAt = DateTime.UtcNow;

        // ACT
        var vehicleTask = new VehicleTask(id)
        {
            VehicleId = vehicleId,
            TaskId = taskId,
            ResponsibleWorkerId = workerId,
            AssignedAt = assignedAt
        };

        // ASSERT
        vehicleTask.Id.ShouldBe(id);
        vehicleTask.VehicleId.ShouldBe(vehicleId);
        vehicleTask.TaskId.ShouldBe(taskId);
        vehicleTask.ResponsibleWorkerId.ShouldBe(workerId);
        vehicleTask.AssignedAt.ShouldBe(assignedAt);
        vehicleTask.ReleasedAt.ShouldBeNull(); // Yeni atamada ReleasedAt bos olmalidir.
    }

    /*
     * SENARYO: Arac serbest birakildiginda ReleasedAt degeri dolmalidir.
     */
    [Fact]
    public void Should_Set_ReleasedAt_When_Finished()
    {
        // ARRANGE
        var vehicleTask = new VehicleTask(Guid.NewGuid());
        var releasedAt = DateTime.UtcNow.AddHours(5);

        // ACT
        vehicleTask.ReleasedAt = releasedAt;

        // ASSERT
        vehicleTask.ReleasedAt.ShouldBe(releasedAt);
    }
}
