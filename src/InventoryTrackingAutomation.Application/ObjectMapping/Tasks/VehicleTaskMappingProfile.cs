using AutoMapper;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Tasks;

/// <summary>
/// VehicleTask entity, DTO ve domain model mapping profili.
/// </summary>
public class VehicleTaskMappingProfile : Profile
{
    public VehicleTaskMappingProfile()
    {
        CreateMap<VehicleTask, VehicleTaskDto>().ReverseMap();
        CreateMap<CreateVehicleTaskDto, CreateVehicleTaskModel>();
        CreateMap<UpdateVehicleTaskDto, UpdateVehicleTaskModel>();
        CreateMap<CreateVehicleTaskModel, VehicleTask>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateVehicleTaskModel, VehicleTask>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
