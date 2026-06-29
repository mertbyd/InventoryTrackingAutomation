using System;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Tasks;

/// <summary>
/// VehicleTaskLine entity audit sozlesmesini dogrulayan testler.
/// </summary>
public class VehicleTaskLine_Audit_Tests
{
    [Fact]
    public void Should_Use_Update_Audit_Without_Soft_Delete()
    {
        // islevi: VehicleTaskLine child kaydinin soft-delete kolonlari tasimadigini dogrular.
        // sistemdeki gorevi: Teslim/iade uzlasmasi almamis tahsis satirlarinin fiziksel delete ile temizlenebilecegini guvenceye alir.
        var type = typeof(VehicleTaskLine);

        typeof(AuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(type).ShouldBeFalse();
    }
}
