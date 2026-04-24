using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Masters;

/// <summary>
/// Lokasyon uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface ISiteAppService : IApplicationService
{
    /// <summary> Id'ye göre tek lokasyon getirir. </summary>
    Task<SiteDto> GetAsync(Guid id);

    /// <summary> Lokasyonları sayfalı listeler. </summary>
    Task<PagedResultDto<SiteDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni lokasyon oluşturur. </summary>
    Task<SiteDto> CreateAsync(CreateSiteDto input);

    /// <summary> Birden fazla lokasyonu toplu oluşturur. </summary>
    Task<List<SiteDto>> CreateManyAsync(List<CreateSiteDto> inputs);

    /// <summary> Lokasyonu günceller. </summary>
    Task<SiteDto> UpdateAsync(Guid id, UpdateSiteDto input);

    /// <summary> Lokasyonu soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
