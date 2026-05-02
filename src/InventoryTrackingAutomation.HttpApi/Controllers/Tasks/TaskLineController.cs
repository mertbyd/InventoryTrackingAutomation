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
/// Görev kalemi endpoint'leri.
/// </summary>
[Route("api/task-lines")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("TaskLines")]
public class TaskLineController : InventoryTrackingAutomationController
{
    public TaskLineController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private ITaskLineAppService _appService => LazyGetRequiredService<ITaskLineAppService>();

    /// <summary>
    /// Görev kalemini Id ile getirir.
    /// </summary>
    /// <param name="id">Görev kalemi Id'si.</param>
    /// <remarks>
    /// Response {
    ///   TaskId             (Guid) → Kalemin bağlı olduğu InventoryTask Id'si
    ///   ProductId          (Guid) → Talep edilen ürün Id'si
    ///   Quantity           (int)  → Görev için istenen toplam miktar
    ///   AllocatedQuantity  (int)  → VehicleTaskLine toplamından hesaplanan dağıtılmış miktar
    ///   RemainingQuantity  (int)  → Henüz dağıtılmamış miktar
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.TaskLines.View)]
    public async Task<Result<TaskLineDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Göreve bağlı kalemleri listeler.
    /// </summary>
    /// <param name="taskId">InventoryTask Id'si.</param>
    /// <remarks>
    /// Response {
    ///   TaskId             (Guid) → Kalemin bağlı olduğu InventoryTask Id'si
    ///   ProductId          (Guid) → Talep edilen ürün Id'si
    ///   Quantity           (int)  → Görev toplam talep miktarı
    ///   AllocatedQuantity  (int)  → VehicleTaskLine toplamından hesaplanan tahsis miktarı
    ///   RemainingQuantity  (int)  → Henüz dağıtılmamış miktar
    /// }
    /// </remarks>
    [HttpGet("by-task/{taskId}")]
    [Authorize(InventoryTrackingAutomationPermissions.TaskLines.View)]
    public async Task<Result<List<TaskLineDto>>> GetByTask(Guid taskId)
    {
        var result = await _appService.GetByTaskAsync(taskId);
        return result;
    }

    /// <summary>
    /// Göreve yeni ürün kalemi ekler.
    /// </summary>
    /// <param name="taskId">InventoryTask Id'si.</param>
    /// <param name="input">Ürün ve miktar bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   ProductId  (Guid) → Talep edilen ürün Id'si
    ///   Quantity   (int)  → Görev için istenen miktar
    /// }
    /// Response {
    ///   TaskId             (Guid) → Kalemin bağlı olduğu InventoryTask Id'si
    ///   ProductId          (Guid) → Talep edilen ürün Id'si
    ///   Quantity           (int)  → Görev için istenen toplam miktar
    ///   AllocatedQuantity  (int)  → Tahsis edilen miktar (ilk oluşturmada 0)
    ///   RemainingQuantity  (int)  → Dağıtılabilir miktar
    /// }
    /// </remarks>
    [HttpPost("for-task/{taskId}")]
    [Authorize(InventoryTrackingAutomationPermissions.TaskLines.Create)]
    public async Task<Result<TaskLineDto>> CreateForTask(Guid taskId, [FromBody] CreateTaskLineDto input)
    {
        var result = await _appService.CreateForTaskAsync(taskId, input);
        return result;
    }

    /// <summary>
    /// Görev kalemini günceller.
    /// </summary>
    /// <param name="taskId">InventoryTask Id'si.</param>
    /// <param name="lineId">TaskLine Id'si.</param>
    /// <param name="input">Yeni miktar bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Quantity (int) → Yeni toplam miktar (AllocatedQuantity değerinin altına indirilemez)
    /// }
    /// Response {
    ///   Quantity           (int) → Güncel toplam görev miktarı
    ///   AllocatedQuantity  (int) → Yeniden hesaplanan mevcut tahsis miktarı
    ///   RemainingQuantity  (int) → Dağıtılabilir miktar
    /// }
    /// </remarks>
    [HttpPut("for-task/{taskId}/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.TaskLines.Edit)]
    public async Task<Result<TaskLineDto>> Update(Guid taskId, Guid lineId, [FromBody] UpdateTaskLineDto input)
    {
        var result = await _appService.UpdateAsync(taskId, lineId, input);
        return result;
    }

    /// <summary>
    /// Görev kalemini siler.
    /// </summary>
    /// <param name="taskId">InventoryTask Id'si.</param>
    /// <param name="lineId">TaskLine Id'si.</param>
    [HttpDelete("for-task/{taskId}/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.TaskLines.Delete)]
    public async Task<Result> Delete(Guid taskId, Guid lineId)
    {
        await _appService.DeleteAsync(taskId, lineId);
        return Result.Success();
    }
}
