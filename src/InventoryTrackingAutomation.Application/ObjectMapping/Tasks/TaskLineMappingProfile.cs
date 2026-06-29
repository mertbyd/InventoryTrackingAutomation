using AutoMapper;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Tasks;

/// <summary>
/// TaskLine entity, DTO ve domain model mapping profili.
/// </summary>
public class TaskLineMappingProfile : Profile
{
    public TaskLineMappingProfile()
    {
        CreateMap<TaskLine, TaskLineDto>()
            .ForMember(dest => dest.AllocatedQuantity, opt => opt.Ignore());
        CreateMap<CreateTaskLineDto, CreateTaskLineModel>()
            .ForMember(dest => dest.TaskId, opt => opt.Ignore());
        CreateMap<UpdateTaskLineDto, UpdateTaskLineModel>();
        CreateMap<CreateTaskLineModel, TaskLine>()
            .IgnoreAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateTaskLineModel, TaskLine>()
            .IgnoreAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TaskId, opt => opt.Ignore());
    }
}
