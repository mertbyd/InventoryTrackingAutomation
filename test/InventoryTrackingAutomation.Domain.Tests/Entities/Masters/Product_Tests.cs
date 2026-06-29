using System;
using InventoryTrackingAutomation.Entities;
using Shouldly;
using Volo.Abp;
using Volo.Abp.Domain.Entities.Auditing;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Masters;

/// <summary>
/// Product entity audit ve aktiflik sozlesmesini dogrulayan testler.
/// </summary>
public class Product_Tests
{
    [Fact]
    public void Should_Use_Update_Audit_Without_Soft_Delete()
    {
        // islevi: Product master verisinin soft-delete kolonlari tasimadigini dogrular.
        // sistemdeki gorevi: Audit temizliklerinde Product'in AuditedEntity seviyesinde kalmasini guvenceye alir.
        var type = typeof(Product);

        typeof(AuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeTrue();
        typeof(FullAuditedEntity<Guid>).IsAssignableFrom(type).ShouldBeFalse();
        typeof(ISoftDelete).IsAssignableFrom(type).ShouldBeFalse();
        typeof(IPassivable).IsAssignableFrom(type).ShouldBeTrue();
    }
}
