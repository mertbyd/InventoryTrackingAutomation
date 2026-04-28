using AutoMapper;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Tasks;

/// <summary>
/// InventoryTask entity, DTO ve domain model mapping profili.
/// </summary>
public class InventoryTaskMappingProfile : Profile
{
    public InventoryTaskMappingProfile()
    {
        CreateMap<InventoryTask, InventoryTaskDto>().ReverseMap();
        CreateMap<CreateInventoryTaskDto, CreateInventoryTaskModel>();
        CreateMap<UpdateInventoryTaskDto, UpdateInventoryTaskModel>();
        CreateMap<CreateInventoryTaskModel, InventoryTask>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateInventoryTaskModel, InventoryTask>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Status, opt => opt.Ignore());
    }
}
