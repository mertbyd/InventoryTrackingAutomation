using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.ObjectMapping.Lookups;

// islevi: VehicleType entity, DTO ve domain model donusumlerini tanimlar.
// sistemdeki gorevi: AppService ve Manager katmanlarinda manuel property kopyalama tekrarini engeller.
public class VehicleTypeMappingProfile : Profile
{
    public VehicleTypeMappingProfile()
    {
        CreateMap<VehicleType, VehicleTypeDto>().ReverseMap();
        CreateMap<CreateVehicleTypeDto, CreateVehicleTypeModel>();
        CreateMap<UpdateVehicleTypeDto, UpdateVehicleTypeModel>();
        CreateMap<CreateVehicleTypeModel, VehicleType>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateVehicleTypeModel, VehicleType>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
