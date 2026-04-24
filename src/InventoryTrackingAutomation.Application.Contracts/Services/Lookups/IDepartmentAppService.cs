using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Lookups;

/// <summary>
/// Departman uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IDepartmentAppService : IApplicationService
{
    /// <summary> Id'ye göre tek departman getirir. </summary>
    Task<DepartmentDto> GetAsync(Guid id);

    /// <summary> Departmanları sayfalı listeler. </summary>
    Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni departman oluşturur. </summary>
    Task<DepartmentDto> CreateAsync(CreateDepartmentDto input);

    /// <summary> Birden fazla departmanı toplu oluşturur. </summary>
    Task<List<DepartmentDto>> CreateManyAsync(List<CreateDepartmentDto> inputs);

    /// <summary> Departmanı günceller. </summary>
    Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input);

    /// <summary> Departmanı soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
