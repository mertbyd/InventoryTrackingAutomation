using InventoryTrackingAutomation.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SystemStandards.Results;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Tasks;

/// <summary>
/// Araç-görev ataması CRUD endpoint'leri.
/// </summary>
[Route("api/vehicle-tasks")]
[ApiExplorerSettings(GroupName = "Tasks")]
[Tags("VehicleTasks")]
public class VehicleTaskController : InventoryTrackingAutomationController
{
    public VehicleTaskController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTaskAppService _appService => LazyGetRequiredService<IVehicleTaskAppService>();

    /// <summary>
    /// Araç görevi kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.View)]
    public async Task<Result<VehicleTaskDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Araç görevi kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.View)]
    public async Task<Result<PagedResultDto<VehicleTaskDto>>> GetList([FromQuery] PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni araç görevi kaydı oluşturur.
    /// </summary>
    /// <param name="input">Araç görevi bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// Response {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result<VehicleTaskDto>> Create([FromBody] CreateVehicleTaskDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla araç görevi kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Araç görevi bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// Response {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result<List<VehicleTaskDto>>> CreateMany([FromBody] List<CreateVehicleTaskDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Araç görevi kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel araç görevi bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// Response {
    ///   VehicleId            (Guid)      → Araç Id'si
    ///   TaskId               (Guid)      → Operasyon iş Id'si
    ///   ResponsibleWorkerId  (Guid)      → Atamadan sorumlu çalışan Id'si
    ///   AssignedAt           (DateTime)  → Atama zamanı
    ///   ReleasedAt           (DateTime?) → Bırakma zamanı
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result<VehicleTaskDto>> Update(Guid id, [FromBody] UpdateVehicleTaskDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Araç görevi kaydını siler.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    [HttpDelete("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result> Delete(Guid id)
    {
        await _appService.DeleteAsync(id);
        return Result.Success();
    }

    // ────────────────────── Lines ──────────────────────

    /// <summary>
    /// Araç görevine ait kalemleri getirir.
    /// </summary>
    [HttpGet("{id}/lines")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.View)]
    public async Task<Result<List<VehicleTaskLineDto>>> GetLines(Guid id)
    {
        var result = await _appService.GetLinesAsync(id);
        return result;
    }

    /// <summary>
    /// Araç görevine yeni kalem ekler.
    /// </summary>
    [HttpPost("{id}/lines")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result<VehicleTaskLineDto>> AddLine(Guid id, [FromBody] CreateVehicleTaskLineDto input)
    {
        var result = await _appService.AddLineAsync(id, input);
        return result;
    }

    /// <summary>
    /// Araç görevi kaleminin tahsis miktarını günceller.
    /// </summary>
    [HttpPut("{id}/lines/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result<VehicleTaskLineDto>> UpdateLine(Guid id, Guid lineId, [FromBody] UpdateVehicleTaskLineDto input)
    {
        var result = await _appService.UpdateLineAsync(id, lineId, input);
        return result;
    }

    /// <summary>
    /// Araç görevi kalemini siler.
    /// </summary>
    [HttpDelete("{id}/lines/{lineId}")]
    [Authorize(InventoryTrackingAutomationPermissions.VehicleTasks.Manage)]
    public async Task<Result> DeleteLine(Guid id, Guid lineId)
    {
        await _appService.DeleteLineAsync(id, lineId);
        return Result.Success();
    }
}
