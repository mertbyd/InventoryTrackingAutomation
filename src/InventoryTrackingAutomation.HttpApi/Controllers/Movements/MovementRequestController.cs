using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Services.Movements;
using InventoryTrackingAutomation.Permissions;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Movements;

/// <summary>
/// Hareket talebi CRUD endpoint'leri.
/// </summary>
[Route("api/movement-requests")]
[ApiExplorerSettings(GroupName = "Movements")]
[Tags("MovementRequests")]
public class MovementRequestController : InventoryTrackingAutomationController
{
    public MovementRequestController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IMovementRequestAppService _appService => LazyGetRequiredService<IMovementRequestAppService>();

    /// <summary>
    /// Hareket talebi kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<MovementRequestDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Hareket talebi kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<MovementRequestDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni hareket talebi kaydı oluşturur.
    /// </summary>
    /// <param name="input">Talep bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   RequestNumber  (string)                → Talep numarası
    ///   VehicleTaskId  (Guid)                  → Araç-görev atama Id'si; task, araç ve rota buradan çözülür
    ///   Priority       (MovementPriorityEnum)  → Öncelik
    ///   RequestNote    (string)                → Talep gerekçesi
    ///   PlannedDate    (DateTime)              → Planlanan teslim tarihi
    /// }
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<MovementRequestDto>> Create([FromBody] CreateMovementRequestDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla hareket talebi kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Talep bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   RequestNumber  (string)                → Talep numarası
    ///   VehicleTaskId  (Guid)                  → Araç-görev atama Id'si; task, araç ve rota buradan çözülür
    ///   Priority       (MovementPriorityEnum)  → Öncelik
    ///   RequestNote    (string)                → Talep gerekçesi
    ///   PlannedDate    (DateTime)              → Planlanan teslim tarihi
    /// }
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
    public async Task<Result<List<MovementRequestDto>>> CreateMany([FromBody] List<CreateMovementRequestDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Hareket talebi kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel talep bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   RequestNumber  (string)                → Talep numarası
    ///   VehicleTaskId  (Guid)                  → Araç-görev atama Id'si
    ///   Priority       (MovementPriorityEnum)  → Öncelik
    ///   RequestNote    (string)                → Talep gerekçesi
    ///   PlannedDate    (DateTime)              → Planlanan teslim tarihi
    /// }
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Edit)]
    public async Task<Result<MovementRequestDto>> Update(Guid id, [FromBody] UpdateMovementRequestDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Hareket talebini depodan araca sevk edildi durumuna taşır.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Sevk notu (opsiyonel).</param>
    /// <remarks>
    /// Request {
    ///   DispatchNote (string?) → Sevk/yükleme notu
    /// }
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpPost("{id}/dispatch")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Dispatch)]
    public async Task<Result<MovementRequestDto>> Dispatch(Guid id, [FromBody] DispatchMovementRequestDto? input)
    {
        var result = await _appService.DispatchAsync(id, input ?? new DispatchMovementRequestDto());
        return result;
    }

    /// <summary>
    /// Hareket talebini hedef lokasyonda teslim alındı durumuna taşır.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Teslim alma notu ve satır detayları (opsiyonel).</param>
    /// <remarks>
    /// Request {
    ///   ReceiveNote (string?) → Teslim alma notu
    /// }
    /// Response {
    ///   RequestNumber             (string)                → Talep numarası
    ///   RequestedByWorkerId       (Guid)                  → Talebi oluşturan çalışan Id'si
    ///   TaskId                    (Guid)                  → Bağlı operasyon iş Id'si
    ///   VehicleTaskId             (Guid)                  → Araç-operasyon atama Id'si
    ///   ParentMovementRequestId   (Guid?)                 → İade hareketinde ana talep Id'si
    ///   Status                    (MovementStatusEnum)    → Talep durumu
    ///   Priority                  (MovementPriorityEnum)  → Öncelik
    ///   RequestNote               (string)                → Talep gerekçesi
    ///   PlannedDate               (DateTime)              → Planlanan teslim tarihi
    ///   CancellationNote          (string?)               → İptal gerekçesi
    ///   WorkflowInstanceId        (Guid?)                 → Bağlı iş akışı Id'si
    /// }
    /// </remarks>
    [HttpPost("{id}/receive")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Receive)]
    public async Task<Result<MovementRequestDto>> Receive(Guid id, [FromBody] ReceiveMovementRequestDto? input)
    {
        var result = await _appService.ReceiveAsync(id, input ?? new ReceiveMovementRequestDto());
        return result;
    }

    /// <summary>
    /// Hareket talebi kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Delete)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }
}
