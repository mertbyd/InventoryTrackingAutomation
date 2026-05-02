using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Microsoft.AspNetCore.Http;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Tasks;

/// <summary>
/// Envanter görevi CRUD endpoint'leri.
/// </summary>
[Route("api/tasks")]
[Route("api/inventory-tasks")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("InventoryTasks")]
public class InventoryTaskController : InventoryTrackingAutomationController
{
    public InventoryTaskController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IInventoryTaskAppService _appService => LazyGetRequiredService<IInventoryTaskAppService>();

    /// <summary>
    /// Envanter görevi kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<InventoryTaskDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Göreve atanmış araçları getirir.
    /// </summary>
    /// <param name="id">Görev Id'si.</param>
    /// <remarks>
    /// Response {
    ///   VehicleTaskId  (Guid)      → Görev-araç atama Id'si
    ///   TaskId         (Guid)      → Operasyon iş Id'si
    ///   VehicleId      (Guid)      → Araç Id'si
    ///   AssignedAt     (DateTime)  → Atama zamanı
    ///   ReleasedAt     (DateTime?) → Ayrılma zamanı
    /// }
    /// </remarks>
    [HttpGet("{id}/vehicles")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<List<TaskVehicleDto>>> GetVehicles(Guid id)
    {
        var result = await _appService.GetVehiclesAsync(id);
        return result;
    }

    /// <summary>
    /// Göreve bağlı envanter satırlarını getirir.
    /// </summary>
    /// <param name="id">Görev Id'si.</param>
    /// <remarks>
    /// Response {
    ///   TaskId            (Guid) → Operasyon iş Id'si
    ///   VehicleTaskId     (Guid) → Görev-araç atama Id'si
    ///   VehicleId         (Guid) → Araç Id'si
    ///   ProductId         (Guid) → Ürün Id'si
    ///   Quantity          (int)  → Fiziksel miktar
    ///   ReservedQuantity  (int)  → Rezerve miktar
    /// }
    /// </remarks>
    [HttpGet("{id}/inventory")]
    [Authorize(InventoryTrackingAutomationPermissions.Inventory.View)]
    public async Task<Result<List<TaskInventoryDto>>> GetInventory(Guid id)
    {
        var result = await _appService.GetInventoryAsync(id);
        return result;
    }

    /// <summary>
    /// Envanter görevi kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<PagedResultDto<InventoryTaskDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni envanter görevi kaydı oluşturur.
    /// </summary>
    /// <param name="input">Görev bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// Response {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<InventoryTaskDto>> Create([FromBody] CreateInventoryTaskDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla envanter görevi kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Görev bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// Response {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<List<InventoryTaskDto>>> CreateMany([FromBody] List<CreateInventoryTaskDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Envanter görevi kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel görev bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// Response {
    ///   Type              (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code              (string)                → Görev kodu
    ///   Name              (string)                → Görev adı
    ///   Region            (string?)               → Görev bölgesi
    ///   StartDate         (DateTime)              → Başlangıç tarihi
    ///   EndDate           (DateTime?)             → Bitiş tarihi
    ///   Status            (TaskStatusEnum)        → Görev durumu
    ///   Description       (string?)               → Görev açıklaması
    ///   ReturnWarehouseId (Guid?)                 → Görev bitince iadenin döneceği depo
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<InventoryTaskDto>> Update(Guid id, [FromBody] UpdateInventoryTaskDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Envanter görevini tamamlandı durumuna taşır.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Type    (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code    (string)                → Görev kodu
    ///   Status  (TaskStatusEnum)        → Görev durumu (Completed)
    /// }
    /// </remarks>
    [HttpPost("{id}/complete")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Complete)]
    public async Task<Result<InventoryTaskDto>> Complete(Guid id)
    {
        var result = await _appService.CompleteAsync(id);
        return result;
    }

    /// <summary>
    /// Envanter görevini iptal edildi durumuna taşır.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   Type    (InventoryTaskTypeEnum) → Görev süreç tipi
    ///   Code    (string)                → Görev kodu
    ///   Status  (TaskStatusEnum)        → Görev durumu (Cancelled)
    /// }
    /// </remarks>
    [HttpPost("{id}/cancel")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Complete)]
    public async Task<Result<InventoryTaskDto>> Cancel(Guid id)
    {
        var result = await _appService.CancelAsync(id);
        return result;
    }

    /// <summary>
    /// Envanter görevi kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }

    // ────────────────────── Lines ──────────────────────

    /// <summary>
    /// Göreve ait kalemleri getirir.
    /// </summary>
    [HttpGet("{id}/lines")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.View)]
    public async Task<Result<List<TaskLineDto>>> GetLines(Guid id)
    {
        var result = await _appService.GetLinesAsync(id);
        return result;
    }

    /// <summary>
    /// Göreve yeni kalem ekler.
    /// </summary>
    [HttpPost("{id}/lines")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<TaskLineDto>> AddLine(Guid id, [FromBody] CreateTaskLineDto input)
    {
        var result = await _appService.AddLineAsync(id, input);
        return result;
    }

    /// <summary>
    /// Görev kalemini günceller.
    /// </summary>
    [HttpPut("{id}/lines/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result<TaskLineDto>> UpdateLine(Guid id, Guid lineId, [FromBody] UpdateTaskLineDto input)
    {
        var result = await _appService.UpdateLineAsync(id, lineId, input);
        return result;
    }

    /// <summary>
    /// Görev kalemini siler.
    /// </summary>
    [HttpDelete("{id}/lines/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.Tasks.Manage)]
    public async Task<Result> DeleteLine(Guid id, Guid lineId)
    {
        await _appService.DeleteLineAsync(id, lineId);
        return Result.Success();
    }
}
