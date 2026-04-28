using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Stock;

/// <summary>
/// Envanter hareketleri uygulama servisi kontrati.
/// </summary>
public interface IInventoryTransactionAppService : IApplicationService
{
    Task<InventoryTransactionDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(PagedResultRequestDto input);
    Task<InventoryTransactionDto> CreateAsync(CreateInventoryTransactionDto input);
    Task<List<InventoryTransactionDto>> CreateManyAsync(List<CreateInventoryTransactionDto> inputs);
    Task<InventoryTransactionDto> UpdateAsync(Guid id, UpdateInventoryTransactionDto input);
    Task DeleteAsync(Guid id);
}
