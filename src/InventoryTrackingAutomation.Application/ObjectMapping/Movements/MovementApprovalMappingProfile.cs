using AutoMapper;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Models.Movements;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Movements;

/// <summary>
/// MovementApproval entity'si ile DTO'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class MovementApprovalMappingProfile : Profile
{
    public MovementApprovalMappingProfile()
    {
        CreateMap<MovementApproval, MovementApprovalDto>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ReverseMap();

        CreateMap<PendingApprovalModel, PendingApprovalDto>();
    }
}
