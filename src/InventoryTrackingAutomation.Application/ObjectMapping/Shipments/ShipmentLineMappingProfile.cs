using AutoMapper;
using InventoryTrackingAutomation.Dtos.Shipments;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Models.Shipments;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Shipments;

/// <summary>
/// ShipmentLine entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class ShipmentLineMappingProfile : Profile
{
    public ShipmentLineMappingProfile()
    {
        CreateMap<ShipmentLine, ShipmentLineDto>().ReverseMap();
        CreateMap<CreateShipmentLineDto, ShipmentLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateShipmentLineDto, ShipmentLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateShipmentLineDto, CreateShipmentLineModel>()
            .ForMember(dest => dest.DamageNote, opt => opt.Ignore());
        CreateMap<CreateShipmentLineModel, ShipmentLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateShipmentLineDto, UpdateShipmentLineModel>()
            .ForMember(dest => dest.DamageNote, opt => opt.Ignore());
        CreateMap<UpdateShipmentLineModel, ShipmentLine>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
