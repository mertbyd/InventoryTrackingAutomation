using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Masters;

/// <summary>
/// Site entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class SiteMappingProfile : Profile
{
    public SiteMappingProfile()
    {
        CreateMap<Site, SiteDto>().ReverseMap();
        CreateMap<CreateSiteDto, Site>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateSiteDto, Site>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateSiteDto, CreateSiteModel>()
            .ForMember(dest => dest.LinkedVehicleId, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedWorkerId, opt => opt.Ignore());
        CreateMap<CreateSiteModel, Site>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerWorkerId, opt => opt.Ignore());
        CreateMap<UpdateSiteDto, UpdateSiteModel>()
            .ForMember(dest => dest.LinkedVehicleId, opt => opt.Ignore())
            .ForMember(dest => dest.LinkedWorkerId, opt => opt.Ignore());
        CreateMap<UpdateSiteModel, Site>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ManagerWorkerId, opt => opt.Ignore());
    }
}
