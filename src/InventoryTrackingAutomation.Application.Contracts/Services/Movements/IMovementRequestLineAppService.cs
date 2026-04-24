using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Movements;

/// <summary>
/// Hareket talebi satırı uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IMovementRequestLineAppService : IApplicationService
{
    /// <summary> Id'ye göre tek hareket talebi satırı getirir. </summary>
    Task<MovementRequestLineDto> GetAsync(Guid id);

    /// <summary> Hareket talebi satırlarını sayfalı listeler. </summary>
    Task<PagedResultDto<MovementRequestLineDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni hareket talebi satırı oluşturur. </summary>
    Task<MovementRequestLineDto> CreateAsync(CreateMovementRequestLineDto input);

    /// <summary> Birden fazla hareket talebi satırını toplu oluşturur. </summary>
    Task<List<MovementRequestLineDto>> CreateManyAsync(List<CreateMovementRequestLineDto> inputs);

    /// <summary> Hareket talebi satırını günceller. </summary>
    Task<MovementRequestLineDto> UpdateAsync(Guid id, UpdateMovementRequestLineDto input);

    /// <summary> Hareket talebi satırını soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
