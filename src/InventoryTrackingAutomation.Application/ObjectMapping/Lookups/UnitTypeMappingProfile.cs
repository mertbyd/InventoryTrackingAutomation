using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.ObjectMapping.Lookups;

// islevi: UnitType entity, DTO ve domain model donusumlerini tanimlar.
// sistemdeki gorevi: AppService ve Manager katmanlarinda manuel property kopyalama tekrarini engeller.
public class UnitTypeMappingProfile : Profile
{
    public UnitTypeMappingProfile()
    {
        CreateMap<UnitType, UnitTypeDto>().ReverseMap();
        CreateMap<CreateUnitTypeDto, CreateUnitTypeModel>();
        CreateMap<UpdateUnitTypeDto, UpdateUnitTypeModel>();
        CreateMap<CreateUnitTypeModel, UnitType>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateUnitTypeModel, UnitType>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
