using System.Threading.Tasks;
using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation.Hubs;

/// <summary>
/// Envanter ve hareket talebi bildirimleri icin yetkili SignalR hub'i.
/// </summary>
[Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
public class InventoryNotificationHub : AbpHub
{
    /// <summary>
    /// Client baglantisinin kimlik dogrulamali olarak calistigini dogrular.
    /// </summary>
    public Task<string> PingAsync()
    {
        return Task.FromResult(CurrentUser.UserName ?? CurrentUser.Id?.ToString() ?? "authenticated");
    }
}
