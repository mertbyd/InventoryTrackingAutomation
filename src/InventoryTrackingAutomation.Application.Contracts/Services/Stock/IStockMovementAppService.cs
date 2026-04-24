using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Stock;

/// <summary>
/// Stok hareketi uygulama servisi kontratı — 6 temel CRUD operasyonunu tanımlar.
/// </summary>
public interface IStockMovementAppService : IApplicationService
{
    /// <summary> Id'ye göre tek stok hareketi getirir. </summary>
    Task<StockMovementDto> GetAsync(Guid id);

    /// <summary> Stok hareketlerini sayfalı listeler. </summary>
    Task<PagedResultDto<StockMovementDto>> GetListAsync(PagedResultRequestDto input);

    /// <summary> Yeni stok hareketi oluşturur. </summary>
    Task<StockMovementDto> CreateAsync(CreateStockMovementDto input);

    /// <summary> Birden fazla stok hareketini toplu oluşturur. </summary>
    Task<List<StockMovementDto>> CreateManyAsync(List<CreateStockMovementDto> inputs);

    /// <summary> Stok hareketini günceller. </summary>
    Task<StockMovementDto> UpdateAsync(Guid id, UpdateStockMovementDto input);

    /// <summary> Stok hareketini soft delete ile siler. </summary>
    Task DeleteAsync(Guid id);
}
