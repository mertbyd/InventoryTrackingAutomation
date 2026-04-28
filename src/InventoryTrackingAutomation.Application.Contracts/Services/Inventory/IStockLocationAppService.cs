using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Inventory;

/// <summary>
/// Lokasyon bazli stok uygulama servisi kontrati.
/// </summary>
public interface IStockLocationAppService : IApplicationService
{
    Task<StockLocationDto> GetAsync(Guid id);
    Task<PagedResultDto<StockLocationDto>> GetListAsync(PagedResultRequestDto input);
    Task<StockLocationDto> CreateAsync(CreateStockLocationDto input);
    Task<List<StockLocationDto>> CreateManyAsync(List<CreateStockLocationDto> inputs);
    Task<StockLocationDto> UpdateAsync(Guid id, UpdateStockLocationDto input);
    Task DeleteAsync(Guid id);
}
