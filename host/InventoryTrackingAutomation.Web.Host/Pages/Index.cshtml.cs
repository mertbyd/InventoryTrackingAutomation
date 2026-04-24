using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

namespace InventoryTrackingAutomation.Pages;

public class IndexModel : InventoryTrackingAutomationPageModel
{
    public void OnGet()
    {

    }

    public async Task OnPostLoginAsync()
    {
        await HttpContext.ChallengeAsync("oidc");
    }
}
