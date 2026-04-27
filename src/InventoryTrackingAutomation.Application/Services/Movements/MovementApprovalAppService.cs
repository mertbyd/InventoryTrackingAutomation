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
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using Volo.Abp.Users;

namespace InventoryTrackingAutomation.Application.Services.Movements;

// Hareket talebi onay application servisi — onay/red orkestra katmanı; iş kuralları MovementApprovalManager'da.
public class MovementApprovalAppService : InventoryTrackingAutomationAppService, IMovementApprovalAppService
{
    // Domain manager — onay/red iş kuralları ve workflow state machine.
    private readonly MovementApprovalManager _manager;
    // Onay geçmişi listeleme için repository.
    private readonly IRepository<MovementApproval, Guid> _repository;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public MovementApprovalAppService(
        MovementApprovalManager manager,
        IRepository<MovementApproval, Guid> repository,
        IMapper mapper)
    {
        _mapper = mapper;
        _manager = manager;
        _repository = repository;
    }

    // Hareket talebini işler — IsApproved değerine göre manager metodunu çağırır.
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
                throw new Volo.Abp.UserFriendlyException("Reddetme nedeni (Note) gereklidir.");
            }
            approval = await _manager.RejectAsync(movementRequestId, CurrentUser.GetId(), input.Note);
        }
        
        return MapToDto(approval);
    }

    // Bir hareket talebinin tüm onay geçmişini StepOrder'a göre sıralı döner.
    public async Task<List<MovementApprovalDto>> GetApprovalHistoryAsync(Guid movementRequestId)
    {
        var approvals = await _repository.GetListAsync(x => x.MovementRequestId == movementRequestId);
        return MapToDtoList(approvals.OrderBy(x => x.StepOrder).ToList());
    }

    // Mevcut kullanıcının onaylaması bekleyen talepleri döner — manager pending sorgusunu yapar, profile mapping uygular.
    public async Task<List<PendingApprovalDto>> GetPendingApprovalsAsync()
    {
        var pending = await _manager.GetPendingApprovalsForUserAsync(CurrentUser.GetId());
        return _mapper.Map<List<PendingApprovalModel>, List<PendingApprovalDto>>(pending);
    }

    // Tek bir MovementApproval entity'sini DTO'ya map eden helper.
    private MovementApprovalDto MapToDto(MovementApproval approval)
        => _mapper.Map<MovementApproval, MovementApprovalDto>(approval);

    // MovementApproval listesini DTO listesine map eden helper.
    private List<MovementApprovalDto> MapToDtoList(List<MovementApproval> approvals)
        => _mapper.Map<List<MovementApproval>, List<MovementApprovalDto>>(approvals);
}
