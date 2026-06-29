using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

// islevi: UnitType lookup CRUD operasyonlarinin uygulama kontratini tanimlar.
// sistemdeki gorevi: HttpApi ve diger istemcilerin olcu birimi referans verisine standart servis uzerinden erismesini saglar.
public interface IUnitTypeAppService : IApplicationService
{
    /// <summary>
    /// Id'ye göre tekil kaydý getirir.
    /// </summary>
    Task<UnitTypeDto> GetAsync(Guid id);
    /// <summary>
    /// Sayfalamalý listeleme yapar.
    /// </summary>
    Task<PagedResultDto<UnitTypeDto>> GetListAsync(PagedResultRequestDto input);
    /// <summary>
    /// Yeni kayýt oluþturur.
    /// </summary>
    Task<UnitTypeDto> CreateAsync(CreateUnitTypeDto input);
    /// <summary>
    /// Toplu yeni kayýtlar oluþturur.
    /// </summary>
    Task<List<UnitTypeDto>> CreateManyAsync(List<CreateUnitTypeDto> inputs);
    /// <summary>
    /// Ýlgili kaydý günceller.
    /// </summary>
    Task<UnitTypeDto> UpdateAsync(Guid id, UpdateUnitTypeDto input);
    /// <summary>
    /// Ýlgili kaydý siler.
    /// </summary>
    Task DeleteAsync(Guid id);
}

