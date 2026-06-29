using System;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Enums;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Movements;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Movements/
 * ACIKLAMA: 'MovementApproval' onay/red karar izidir; karar kaydi sonradan guncellenmez veya soft-delete edilmez.
 */
public class MovementApproval_Tests
{
    [Fact]
    public void Should_Initialize_Movement_Approval()
    {
        // ARRANGE
        var id = Guid.NewGuid();
        var movementRequestId = Guid.NewGuid();
        var approverWorkerId = Guid.NewGuid();

        // ACT
        var approval = new MovementApproval(id)
        {
            MovementRequestId = movementRequestId,
            ApproverWorkerId = approverWorkerId,
            StepOrder = 1,
            Status = ApprovalStatusEnum.Approved,
            DecidedAt = DateTime.UtcNow,
            Note = "Approved"
        };

        // ASSERT
        approval.Id.ShouldBe(id);
        approval.MovementRequestId.ShouldBe(movementRequestId);
        approval.ApproverWorkerId.ShouldBe(approverWorkerId);
        approval.Status.ShouldBe(ApprovalStatusEnum.Approved);
    }

    [Fact]
    public void Should_Use_Creation_Audit_Only()
    {
        // ARRANGE
        var approval = new MovementApproval(Guid.NewGuid());

        // ACT
        var approvalType = approval.GetType();

        // ASSERT
        // islevi: Onay karar izinin sadece olusturma audit bilgisi tasidigini dogrular.
        // sistemdeki gorevi: MovementApproval'in yanlislikla tekrar FullAudited/SoftDelete yapilmasini engeller.
        typeof(CreationAuditedEntity<Guid>).IsAssignableFrom(approvalType).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(approvalType).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(approvalType).ShouldBeFalse();
    }
}
