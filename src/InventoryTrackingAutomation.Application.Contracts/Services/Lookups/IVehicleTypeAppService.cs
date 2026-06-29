using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

// islevi: VehicleType lookup CRUD operasyonlarinin uygulama kontratini tanimlar.
// sistemdeki gorevi: HttpApi ve diger istemcilerin arac tipi referans verisine standart servis uzerinden erismesini saglar.
public interface IVehicleTypeAppService : IApplicationService
{
    /// <summary>
    /// Id'ye göre tekil kaydý getirir.
    /// </summary>
    Task<VehicleTypeDto> GetAsync(Guid id);
    /// <summary>
    /// Sayfalamalý listeleme yapar.
    /// </summary>
    Task<PagedResultDto<VehicleTypeDto>> GetListAsync(PagedResultRequestDto input);
    /// <summary>
    /// Yeni kayýt oluþturur.
    /// </summary>
    Task<VehicleTypeDto> CreateAsync(CreateVehicleTypeDto input);
    /// <summary>
    /// Toplu yeni kayýtlar oluþturur.
    /// </summary>
    Task<List<VehicleTypeDto>> CreateManyAsync(List<CreateVehicleTypeDto> inputs);
    /// <summary>
    /// Ýlgili kaydý günceller.
    /// </summary>
    Task<VehicleTypeDto> UpdateAsync(Guid id, UpdateVehicleTypeDto input);
    /// <summary>
    /// Ýlgili kaydý siler.
    /// </summary>
    Task DeleteAsync(Guid id);
}

