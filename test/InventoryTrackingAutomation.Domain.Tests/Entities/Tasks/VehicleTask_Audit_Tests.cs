using System;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// VehicleTask entity audit sozlesmesini dogrulayan testler.
/// </summary>
public class VehicleTask_Audit_Tests
{
    [Fact]
    public void Should_Use_Update_Audit_Without_Soft_Delete()
    {
        // islevi: VehicleTask atama kaydinin soft-delete kolonlari tasimadigini dogrular.
        // sistemdeki gorevi: Atama gecmisinin ReleasedAt ve iliskili satirlarla korunacagini guvenceye alir.
        var type = typeof(VehicleTask);

        typeof(AuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(type).ShouldBeFalse();
    }
}
