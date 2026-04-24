using AutoMapper;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Models.Movements;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Movements;

/// <summary>
/// MovementRequestLine entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class MovementRequestLineMappingProfile : Profile
{
    public MovementRequestLineMappingProfile()
    {
        CreateMap<MovementRequestLine, MovementRequestLineDto>().ReverseMap();
        CreateMap<CreateMovementRequestLineDto, MovementRequestLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateMovementRequestLineDto, MovementRequestLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateMovementRequestLineDto, CreateMovementRequestLineModel>();
        CreateMap<CreateMovementRequestLineModel, MovementRequestLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateMovementRequestLineDto, UpdateMovementRequestLineModel>();
        CreateMap<UpdateMovementRequestLineModel, MovementRequestLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
