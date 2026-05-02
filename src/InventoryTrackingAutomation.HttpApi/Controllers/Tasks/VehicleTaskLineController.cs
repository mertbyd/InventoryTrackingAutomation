using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Permissions;
using InventoryTrackingAutomation.Services.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SystemStandards.Results;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Tasks;

/// <summary>
/// Araç-görev kalemi endpoint'leri.
/// </summary>
[Route("api/vehicle-task-lines")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("VehicleTaskLines")]
public class VehicleTaskLineController : InventoryTrackingAutomationController
{
    public VehicleTaskLineController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTaskLineAppService _appService => LazyGetRequiredService<IVehicleTaskLineAppService>();

    /// <summary>
    /// Araç-görev kalemini Id ile getirir.
    /// </summary>
    /// <param name="id">VehicleTaskLine Id'si.</param>
    /// <remarks>
    /// Response {
    ///   VehicleTaskId      (Guid)    → Araç-görev ataması Id'si
    ///   TaskLineId         (Guid)    → Kaynak görev kalemi Id'si
    ///   ProductId          (Guid)    → TaskLine üzerinden türetilen ürün Id'si
    ///   AllocatedQuantity  (int)     → Araca tahsis edilen miktar
    ///   ReceivedQuantity   (int)     → İade tesliminde depoya alınan sağlam miktar
    ///   DamagedQuantity    (int)     → Hasarlı miktar
    ///   LostQuantity       (int)     → Kayıp miktar
    ///   ConsumedQuantity   (int)     → Sarf edilen miktar
    ///   ReceiveNote        (string?) → İade teslim notu
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTaskLines.View)]
    public async Task<Result<VehicleTaskLineDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Araç-görev atamasına bağlı kalemleri listeler.
    /// </summary>
    /// <param name="vehicleTaskId">VehicleTask Id'si.</param>
    /// <remarks>
    /// Response {
    ///   VehicleTaskId      (Guid)    → Araç-görev ataması Id'si
    ///   TaskLineId         (Guid)    → Kaynak görev kalemi Id'si
    ///   ProductId          (Guid)    → Ürün Id'si
    ///   AllocatedQuantity  (int)     → Araçtaki tahsis miktarı
    ///   ReceivedQuantity   (int)     → İade tesliminde depoya alınan sağlam miktar
    ///   DamagedQuantity    (int)     → Hasarlı miktar
    ///   LostQuantity       (int)     → Kayıp miktar
    ///   ConsumedQuantity   (int)     → Sarf edilen miktar
    ///   ReceiveNote        (string?) → İade teslim notu
    /// }
    /// </remarks>
    [HttpGet("by-vehicle-task/{vehicleTaskId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTaskLines.View)]
    public async Task<Result<List<VehicleTaskLineDto>>> GetByVehicleTask(Guid vehicleTaskId)
    {
        var result = await _appService.GetByVehicleTaskAsync(vehicleTaskId);
        return result;
    }

    /// <summary>
    /// Araç-görev atamasına görev kalemi tahsisi ekler.
    /// </summary>
    /// <param name="vehicleTaskId">VehicleTask Id'si.</param>
    /// <param name="input">TaskLine Id'si ve tahsis miktarı.</param>
    /// <remarks>
    /// Request {
    ///   TaskLineId         (Guid) → Kaynak görev kalemi Id'si
    ///   AllocatedQuantity  (int)  → Araç için ayrılan miktar
    /// }
    /// Response {
    ///   VehicleTaskId      (Guid) → Araç-görev ataması Id'si
    ///   TaskLineId         (Guid) → Kaynak görev kalemi Id'si
    ///   ProductId          (Guid) → TaskLine üzerinden türetilen ürün Id'si
    ///   AllocatedQuantity  (int)  → Araç için ayrılan miktar
    /// }
    /// </remarks>
    [HttpPost("for-vehicle-task/{vehicleTaskId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTaskLines.Create)]
    public async Task<Result<VehicleTaskLineDto>> CreateForVehicleTask(Guid vehicleTaskId, [FromBody] CreateVehicleTaskLineDto input)
    {
        var result = await _appService.CreateForVehicleTaskAsync(vehicleTaskId, input);
        return result;
    }

    /// <summary>
    /// Araç-görev kaleminin tahsis miktarını günceller.
    /// </summary>
    /// <param name="vehicleTaskId">VehicleTask Id'si.</param>
    /// <param name="lineId">VehicleTaskLine Id'si.</param>
    /// <param name="input">Yeni tahsis miktarı.</param>
    /// <remarks>
    /// Request {
    ///   AllocatedQuantity (int) → Güncel araç tahsis miktarı (uzlaşma başlamışsa değiştirilemez)
    /// }
    /// Response {
    ///   AllocatedQuantity (int) → Güncel araç tahsis miktarı
    /// }
    /// </remarks>
    [HttpPut("for-vehicle-task/{vehicleTaskId}/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTaskLines.Edit)]
    public async Task<Result<VehicleTaskLineDto>> Update(Guid vehicleTaskId, Guid lineId, [FromBody] UpdateVehicleTaskLineDto input)
    {
        var result = await _appService.UpdateAsync(vehicleTaskId, lineId, input);
        return result;
    }

    /// <summary>
    /// Araç-görev kalemini siler.
    /// </summary>
    /// <param name="vehicleTaskId">VehicleTask Id'si.</param>
    /// <param name="lineId">VehicleTaskLine Id'si.</param>
    [HttpDelete("for-vehicle-task/{vehicleTaskId}/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTaskLines.Delete)]
    public async Task<Result> Delete(Guid vehicleTaskId, Guid lineId)
    {
        await _appService.DeleteAsync(vehicleTaskId, lineId);
        return Result.Success();
    }
}
