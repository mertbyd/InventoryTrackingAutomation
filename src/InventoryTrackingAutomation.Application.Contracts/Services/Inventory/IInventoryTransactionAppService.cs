using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Inventory;

/// <summary>
/// Envanter hareketleri uygulama servisi kontrati.
/// </summary>
public interface IInventoryTransactionAppService : IApplicationService
{
    Task<InventoryTransactionDto> GetAsync(Guid id);
    Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(PagedResultRequestDto input);
    Task<InventoryTransactionDto> CreateAsync(CreateInventoryTransactionDto input);
    Task<List<InventoryTransactionDto>> CreateManyAsync(List<CreateInventoryTransactionDto> inputs);

    /// <summary>
    /// Append-only ledger kurali geregi guncelleme istegini InventoryTransaction.ImmutableLedger hatasi ile reddeder.
    /// </summary>
    Task<InventoryTransactionDto> UpdateAsync(Guid id, UpdateInventoryTransactionDto input);

    /// <summary>
    /// Append-only ledger kurali geregi silme istegini InventoryTransaction.ImmutableLedger hatasi ile reddeder.
    /// </summary>
    Task DeleteAsync(Guid id);
}
