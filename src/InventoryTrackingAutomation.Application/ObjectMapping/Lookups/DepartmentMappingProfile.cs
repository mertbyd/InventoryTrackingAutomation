using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.ObjectMapping.Lookups;

/// <summary>
/// Department entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class DepartmentMappingProfile : Profile
{
    public DepartmentMappingProfile()
    {
        CreateMap<Department, DepartmentDto>().ReverseMap();
        CreateMap<CreateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateDepartmentDto, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateDepartmentDto, CreateDepartmentModel>();
        CreateMap<CreateDepartmentModel, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateDepartmentDto, UpdateDepartmentModel>();
        CreateMap<UpdateDepartmentModel, Department>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
