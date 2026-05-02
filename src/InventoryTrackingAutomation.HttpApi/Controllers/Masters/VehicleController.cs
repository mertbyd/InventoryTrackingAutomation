using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Services.Masters;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Masters;

/// <summary>
/// Araç CRUD endpoint'leri.
/// </summary>
[Route("api/vehicles")]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Vehicles")]
public class VehicleController : InventoryTrackingAutomationController
{
    public VehicleController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleAppService _appService => LazyGetRequiredService<IVehicleAppService>();

    /// <summary>
    /// Araç kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<VehicleDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Araç üzerinde bulunan envanterleri getirir.
    /// </summary>
    /// <param name="id">Araç Id'si.</param>
    /// <remarks>
    /// Response {
    ///   VehicleId         (Guid)  → Araç Id'si
    ///   ProductId         (Guid)  → Ürün Id'si
    ///   VehicleTaskId     (Guid?) → Aktif görev-araç atama Id'si
    ///   TaskId            (Guid?) → Aktif operasyon iş Id'si
    ///   Quantity          (int)   → Fiziksel miktar
    ///   ReservedQuantity  (int)   → Rezerve miktar
    /// }
    /// </remarks>
    [HttpGet("{id}/inventories")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<List<VehicleInventoryDto>>> GetInventories(Guid id)
    {
        var result = await _appService.GetInventoriesAsync(id);
        return result;
    }

    /// <summary>
    /// Araç kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<VehicleDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni araç kaydı oluşturur.
    /// </summary>
    /// <param name="input">Araç bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// Response {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<VehicleDto>> Create([FromBody] CreateVehicleDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla araç kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Araç bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// Response {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<VehicleDto>>> CreateMany([FromBody] List<CreateVehicleDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Araç kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel araç bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// Response {
    ///   PlateNumber  (string)          → Plaka numarası
    ///   VehicleType  (VehicleTypeEnum) → Araç tipi
    ///   IsActive     (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<VehicleDto>> Update(Guid id, [FromBody] UpdateVehicleDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Araç kaydını siler.
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
