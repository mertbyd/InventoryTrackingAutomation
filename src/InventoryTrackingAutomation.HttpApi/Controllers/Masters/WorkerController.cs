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
/// Çalışan CRUD endpoint'leri.
/// </summary>
[Route("api/workers")]
[ApiExplorerSettings(GroupName = "Masters")]
[Tags("Workers")]
public class WorkerController : InventoryTrackingAutomationController
{
    public WorkerController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IWorkerAppService _appService => LazyGetRequiredService<IWorkerAppService>();

    /// <summary>
    /// Çalışan kaydını Id ile getirir.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <remarks>
    /// Response {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpGet("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<WorkerDto>> Get(Guid id)
    {
        var result = await _appService.GetAsync(id);
        return result;
    }

    /// <summary>
    /// Çalışan kayıtlarını sayfalı liste olarak getirir.
    /// </summary>
    /// <param name="input">Sayfalama parametreleri.</param>
    /// <remarks>
    /// Response {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpGet]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.View)]
    public async Task<Result<Volo.Abp.Application.Dtos.PagedResultDto<WorkerDto>>> GetList([FromQuery] Volo.Abp.Application.Dtos.PagedResultRequestDto input)
    {
        var result = await _appService.GetListAsync(input);
        return result;
    }

    /// <summary>
    /// Yeni çalışan kaydı oluşturur.
    /// </summary>
    /// <param name="input">Çalışan bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// Response {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPost]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<WorkerDto>> Create([FromBody] CreateWorkerDto input)
    {
        var result = await _appService.CreateAsync(input);
        return result;
    }

    /// <summary>
    /// Birden fazla çalışan kaydını toplu oluşturur.
    /// </summary>
    /// <param name="inputs">Çalışan bilgileri listesi.</param>
    /// <remarks>
    /// Request {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// Response {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPost("bulk")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<List<WorkerDto>>> CreateMany([FromBody] List<CreateWorkerDto> inputs)
    {
        var result = await _appService.CreateManyAsync(inputs);
        return result;
    }

    /// <summary>
    /// Çalışan kaydını günceller.
    /// </summary>
    /// <param name="id">Kaydın benzersiz Id'si.</param>
    /// <param name="input">Güncel çalışan bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// Response {
    ///   UserId              (Guid)            → ABP Identity kullanıcı Id'si
    ///   RegistrationNumber  (string)          → Sicil numarası
    ///   WorkerType          (WorkerTypeEnum)  → Çalışan tipi
    ///   DepartmentId        (Guid?)           → Bağlı departman Id'si
    ///   DefaultWarehouseId  (Guid?)           → Varsayılan lokasyon Id'si
    ///   ManagerId           (Guid?)           → Yönetici Worker Id'si
    ///   IsActive            (bool)            → Aktif mi
    /// }
    /// </remarks>
    [HttpPut("{id}")]
    [Authorize(InventoryTrackingAutomationPermissions.Masters.Manage)]
    public async Task<Result<WorkerDto>> Update(Guid id, [FromBody] UpdateWorkerDto input)
    {
        var result = await _appService.UpdateAsync(id, input);
        return result;
    }

    /// <summary>
    /// Çalışan kaydını siler.
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
