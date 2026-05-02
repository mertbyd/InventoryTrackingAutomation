using System.Threading.Tasks;
using InventoryTrackingAutomation.Permissions;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Modularity;
using Xunit;

namespace InventoryTrackingAutomation.Movements;

public abstract class MovementRequestAppService_Permission_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IPermissionDefinitionManager _permissionDefinitionManager;

    protected MovementRequestAppService_Permission_Tests()
    {
        _permissionDefinitionManager = GetRequiredService<IPermissionDefinitionManager>();
    }

    [Fact]
    public async Task MovementRequest_Create_Permission_Should_Be_Defined()
    {
        var permission = await _permissionDefinitionManager.GetOrNullAsync(
            InventoryTrackingAutomationPermissions.MovementRequests.Create);

        Assert.NotNull(permission);
        Assert.Equal(
            InventoryTrackingAutomationPermissions.MovementRequests.Default,
            permission!.Parent!.Name);
    }
}
