using AutoMapper;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Models.Movements;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Movements;

/// <summary>
/// MovementRequest entity'si ile DTO ve Model'lar arasindaki AutoMapper mapping profili.
/// </summary>
public class MovementRequestMappingProfile : Profile
{
    public MovementRequestMappingProfile()
    {
        CreateMap<MovementRequest, MovementRequestDto>().ReverseMap();

        CreateMap<CreateMovementRequestDto, MovementRequest>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedByWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationNote, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<UpdateMovementRequestDto, MovementRequest>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.RequestedByWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationNote, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateMovementRequestDto, CreateMovementRequestModel>()
            .ForMember(dest => dest.RequestedByWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<UpdateMovementRequestDto, UpdateMovementRequestModel>()
            .ForMember(dest => dest.RequestedByWorkerId, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());

        CreateMap<CreateMovementRequestModel, MovementRequest>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationNote, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore());

        CreateMap<UpdateMovementRequestModel, MovementRequest>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationNote, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore());

        // With-lines mapping'leri DTO -> Model -> Entity akisini bozmadan kurar.
        CreateMap<CreateMovementRequestWithLinesDto, CreateMovementRequestWithLinesModel>()
            .ForMember(dest => dest.RequestedByWorkerId, opt => opt.Ignore());

        CreateMap<CreateMovementRequestLineItemDto, CreateMovementRequestLineItemModel>();

        CreateMap<CreateMovementRequestWithLinesModel, MovementRequest>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore())
            .ForMember(dest => dest.CancellationNote, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowInstanceId, opt => opt.Ignore())
            .ForSourceMember(src => src.Lines, opt => opt.DoNotValidate());

        CreateMap<CreateMovementRequestLineItemModel, MovementRequestLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.MovementRequestId, opt => opt.Ignore()); // Manager set eder.
    }
}
