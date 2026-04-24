using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Movements;

/// <summary>
/// Hareket talebi uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IMovementRequestAppService : IApplicationService
{
    /// <summary> Id'ye göre tek hareket talebi getirir. </summary>
    Task<MovementRequestDto> GetAsync(Guid id);

    /// <summary> Hareket taleplerini sayfalı listeler. </summary>
    Task<PagedResultDto<MovementRequestDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni hareket talebi oluşturur. </summary>
    Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input);

    /// <summary> Birden fazla hareket talebini toplu oluşturur. </summary>
    Task<List<MovementRequestDto>> CreateManyAsync(List<CreateMovementRequestDto> inputs);

    /// <summary> Hareket talebini günceller. </summary>
    Task<MovementRequestDto> UpdateAsync(Guid id, UpdateMovementRequestDto input);

    /// <summary> Hareket talebini soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
