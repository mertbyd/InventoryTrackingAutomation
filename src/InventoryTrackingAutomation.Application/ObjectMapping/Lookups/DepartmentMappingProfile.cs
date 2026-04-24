using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.AutoMapper;

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
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateDepartmentDto, Department>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateDepartmentDto, CreateDepartmentModel>();
        CreateMap<CreateDepartmentModel, Department>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateDepartmentDto, UpdateDepartmentModel>();
        CreateMap<UpdateDepartmentModel, Department>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
