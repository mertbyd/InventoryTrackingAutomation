using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Lookups;

/// <summary>
/// Departman CRUD endpoint'leri.
/// </summary>
[Route("api/departments")]
[ApiExplorerSettings(GroupName = "Lookups")]
[Tags("Departments")]
public class DepartmentController : InventoryTrackingAutomationController
{
    public DepartmentController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IDepartmentAppService _appService => LazyGetRequiredService<IDepartmentAppService>();

    /// <summary>
    /// Departman kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<DepartmentDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Departman kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<DepartmentDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni departman kaydı oluşturur.
    /// </summary>
    /// <param name="input">Departman bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// Response {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<DepartmentDto>> Create([FromBody] CreateDepartmentDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla departman kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Departman bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// Response {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<DepartmentDto>>> CreateMany([FromBody] List<CreateDepartmentDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Departman kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel departman bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// Response {
    ///   Code (string) → Departman kodu
    ///   Name (string) → Departman adı
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<DepartmentDto>> Update(Guid id, [FromBody] UpdateDepartmentDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Departman kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
