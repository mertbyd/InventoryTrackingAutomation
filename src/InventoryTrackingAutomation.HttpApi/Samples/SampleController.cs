using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Samples;

[Area(InventoryTrackingAutomationRemoteServiceConsts.ModuleName)]
[RemoteService(Name = InventoryTrackingAutomationRemoteServiceConsts.RemoteServiceName)]
[Route("api/InventoryTrackingAutomation/sample")]
public class SampleController : InventoryTrackingAutomationController, ISampleAppService
{
    public SampleController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private ISampleAppService _sampleAppService => LazyGetRequiredService<ISampleAppService>();



    [HttpGet]
    public async Task<SampleDto> GetAsync()
    {
        return await _sampleAppService.GetAsync();
    }

    [HttpGet]
    [Route("authorized")]
    [Authorize]
    public async Task<SampleDto> GetAuthorizedAsync()
    {
        return await _sampleAppService.GetAsync();
    }
}
