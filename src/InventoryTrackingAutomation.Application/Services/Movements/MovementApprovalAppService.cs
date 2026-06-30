using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Managers.Movements;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Services.Movements;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Movements;

// Hareket talebi onay application servisi — onay/red orkestra katmanı; iş kuralları MovementApprovalManager'da.
//işlevi: MovementApproval iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class MovementApprovalAppService : InventoryTrackingAutomationAppService, IMovementApprovalAppService
{
    public MovementApprovalAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Domain manager — onay/red iş kuralları ve workflow state machine.
    private MovementApprovalManager _manager => LazyGetRequiredService<MovementApprovalManager>();
    // Onay geçmişi listeleme için repository.
    private IRepository<MovementApproval, Guid> _repository => LazyGetRequiredService<IRepository<MovementApproval, Guid>>();

    // Tüm bağımlılıkları DI ile alır.
    private IMapper _mapper => LazyGetRequiredService<IMapper>();


    /// Onay işlemini gerçekleştirmek için kullanılır.
    [UnitOfWork]
    public async Task<MovementApprovalDto> ProcessApprovalAsync(Guid movementRequestId, ProcessMovementApprovalDto input)
    {
        MovementApproval approval;
        
        if (input.IsApproved)
        {
            approval = await _manager.ApproveAsync(movementRequestId, CurrentUser.GetId(), input.Note);
        }
        else
        {
            // Red durumunda note zorunlu
            if (string.IsNullOrWhiteSpace(input.Note))
            {
                throw new BusinessException(MovementApprovalExceptionCodes.RejectionNoteRequired);
            }
            approval = await _manager.RejectAsync(movementRequestId, CurrentUser.GetId(), input.Note);
        }
        
        return MapToDto(approval);
    }

    /// Onay geçmişini getirmek için kullanılır.
    public async Task<List<MovementApprovalDto>> GetApprovalHistoryAsync(Guid movementRequestId)
    {
        var approvals = await _repository.GetListAsync(x => x.MovementRequestId == movementRequestId);
        return MapToDtoList(approvals.OrderBy(x => x.StepOrder).ToList());
    }

    /// Bekleyen onayları getirmek için kullanılır.
    public async Task<List<PendingApprovalDto>> GetPendingApprovalsAsync()
    {
        var pending = await _manager.GetPendingApprovalsForUserAsync(CurrentUser.GetId());
        return _mapper.Map<List<PendingApprovalModel>, List<PendingApprovalDto>>(pending);
    }

    /// Veriyi DTO modeline dönüştürmek için kullanılır.
    private MovementApprovalDto MapToDto(MovementApproval approval)
        => _mapper.Map<MovementApproval, MovementApprovalDto>(approval);

    /// Veri listesini DTO listesine dönüştürmek için kullanılır.
    private List<MovementApprovalDto> MapToDtoList(List<MovementApproval> approvals)
        => _mapper.Map<List<MovementApproval>, List<MovementApprovalDto>>(approvals);
}
