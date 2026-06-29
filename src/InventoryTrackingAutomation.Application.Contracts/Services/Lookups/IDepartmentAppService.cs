using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

/// <summary>
/// Departman uygulama servisi kontratÄ± â€” 6 temel CRUD operasyonunu tanÄ±mlar.
/// </summary>
public interface IDepartmentAppService : IApplicationService
{
    /// <summary> Id'ye gÃ¶re tek departman getirir. </summary>
    /// <summary>
    /// Id'ye göre tekil kaydý getirir.
    /// </summary>
    Task<DepartmentDto> GetAsync(Guid id);

    /// <summary> DepartmanlarÄ± sayfalÄ± listeler. </summary>
    /// <summary>
    /// Sayfalamalý listeleme yapar.
    /// </summary>
    Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni departman oluÅŸturur. </summary>
    /// <summary>
    /// Yeni kayýt oluþturur.
    /// </summary>
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);

    /// <summary> Birden fazla departmanÄ± toplu oluÅŸturur. </summary>
    /// <summary>
    /// Toplu yeni kayýtlar oluþturur.
    /// </summary>
    Task<List<DepartmentDto>> CreateManyAsync(List<CreateDepartmentDto> inputs);

    /// <summary> DepartmanÄ± gÃ¼nceller. </summary>
    /// <summary>
    /// Ýlgili kaydý günceller.
    /// </summary>
    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input);

    /// <summary> DepartmanÄ± soft delete ile siler. </summary>
    /// <summary>
    /// Ýlgili kaydý siler.
    /// </summary>
    Task DeleteAsync(Guid id);
}

