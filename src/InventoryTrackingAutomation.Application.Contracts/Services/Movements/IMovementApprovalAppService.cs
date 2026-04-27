using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Movements;

/// <summary>
/// Hareket talebi onay işlemlerinin uygulama servisi.
/// </summary>
public interface IMovementApprovalAppService : IApplicationService
{
    /// <summary>
    /// Hareket talebini işler (Onaylar veya Reddeder).
    /// </summary>
    Task<MovementApprovalDto> ProcessApprovalAsync(Guid movementRequestId, ProcessMovementApprovalDto input);

    /// <summary>
    /// Hareket talebinin onay geçmişini getirir.
    /// </summary>
    Task<List<MovementApprovalDto>> GetApprovalHistoryAsync(Guid movementRequestId);

    /// <summary>
    /// Mevcut kullanıcının onaylaması gereken bekleyen talepleri getirir.
    /// </summary>
    Task<List<PendingApprovalDto>> GetPendingApprovalsAsync();
}
