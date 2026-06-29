using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

// islevi: WorkerType lookup CRUD operasyonlarinin uygulama kontratini tanimlar.
// sistemdeki gorevi: HttpApi ve diger istemcilerin calisan tipi referans verisine standart servis uzerinden erismesini saglar.
public interface IWorkerTypeAppService : IApplicationService
{
    /// <summary>
    /// Id'ye göre tekil kaydý getirir.
    /// </summary>
    Task<WorkerTypeDto> GetAsync(Guid id);
    /// <summary>
    /// Sayfalamalý listeleme yapar.
    /// </summary>
    Task<PagedResultDto<WorkerTypeDto>> GetListAsync(PagedResultRequestDto input);
    /// <summary>
    /// Yeni kayýt oluþturur.
    /// </summary>
    Task<WorkerTypeDto> CreateAsync(CreateWorkerTypeDto input);
    /// <summary>
    /// Toplu yeni kayýtlar oluþturur.
    /// </summary>
    Task<List<WorkerTypeDto>> CreateManyAsync(List<CreateWorkerTypeDto> inputs);
    /// <summary>
    /// Ýlgili kaydý günceller.
    /// </summary>
    Task<WorkerTypeDto> UpdateAsync(Guid id, UpdateWorkerTypeDto input);
    /// <summary>
    /// Ýlgili kaydý siler.
    /// </summary>
    Task DeleteAsync(Guid id);
}

