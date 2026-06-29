using System;
using InventoryTrackingAutomation.Entities;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Worker entity audit ve aktiflik sozlesmesini dogrulayan testler.
/// </summary>
public class Worker_Tests
{
    [Fact]
    public void Should_Use_Update_Audit_Without_Soft_Delete()
    {
        // islevi: Worker master verisinin soft-delete kolonlari tasimadigini dogrular.
        // sistemdeki gorevi: Audit temizliklerinde Worker'in AuditedEntity seviyesinde kalmasini guvenceye alir.
        var type = typeof(Worker);

        typeof(AuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(type).ShouldBeFalse();
        typeof(IPassivable).IsAssignableFrom(type).ShouldBeTrue();
    }
}
