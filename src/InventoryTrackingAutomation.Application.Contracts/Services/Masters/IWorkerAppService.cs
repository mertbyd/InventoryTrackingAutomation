using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Masters;

/// <summary>
/// Çalışan uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IWorkerAppService : IApplicationService
{
    /// <summary> Id'ye göre tek çalışan getirir. </summary>
    Task<WorkerDto> GetAsync(Guid id);

    /// <summary> Çalışanları sayfalı listeler. </summary>
    Task<PagedResultDto<WorkerDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni çalışan oluşturur. </summary>
    Task<WorkerDto> CreateAsync(CreateWorkerDto input);

    /// <summary> Birden fazla çalışanı toplu oluşturur. </summary>
    Task<List<WorkerDto>> CreateManyAsync(List<CreateWorkerDto> inputs);

    /// <summary> Çalışanı günceller. </summary>
    Task<WorkerDto> UpdateAsync(Guid id, UpdateWorkerDto input);

    /// <summary> Çalışanı soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
