using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Services.Masters;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Depo CRUD endpoint'leri.
/// </summary>
[Route("api/warehouses")]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Warehouses")]
public class WarehouseController : InventoryTrackingAutomationController
{
    public WarehouseController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IWarehouseAppService _appService => LazyGetRequiredService<IWarehouseAppService>();

    /// <summary>
    /// Depo kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<WarehouseDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Depo kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<WarehouseDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni depo kaydı oluşturur.
    /// </summary>
    /// <param name="input">Depo bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// Response {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<WarehouseDto>> Create([FromBody] CreateWarehouseDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla depo kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Depo bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// Response {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<WarehouseDto>>> CreateMany([FromBody] List<CreateWarehouseDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Depo kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel depo bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// Response {
    ///   Address          (string?) → Depoya ulaşım için adres bilgisi
    ///   ManagerWorkerId  (Guid?)   → Depodan sorumlu çalışan Id'si
    ///   IsActive         (bool)    → Operasyonlarda kullanılabilirlik durumu
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<WarehouseDto>> Update(Guid id, [FromBody] UpdateWarehouseDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Depo kaydını siler.
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
