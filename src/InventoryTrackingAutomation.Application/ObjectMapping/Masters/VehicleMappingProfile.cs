using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Masters;

/// <summary>
/// Vehicle entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class VehicleMappingProfile : Profile
{
    public VehicleMappingProfile()
    {
        CreateMap<Vehicle, VehicleDto>().ReverseMap();
        CreateMap<CreateVehicleDto, Vehicle>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateVehicleDto, Vehicle>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateVehicleDto, CreateVehicleModel>();
        CreateMap<CreateVehicleModel, Vehicle>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateVehicleDto, UpdateVehicleModel>();
        CreateMap<UpdateVehicleModel, Vehicle>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
