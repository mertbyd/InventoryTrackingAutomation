using AutoMapper;
using InventoryTrackingAutomation.Dtos.Shipments;
using InventoryTrackingAutomation.Entities.Shipments;
using InventoryTrackingAutomation.Models.Shipments;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Shipments;

/// <summary>
/// Shipment entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class ShipmentMappingProfile : Profile
{
    public ShipmentMappingProfile()
    {
        CreateMap<Shipment, ShipmentDto>().ReverseMap();
        CreateMap<CreateShipmentDto, Shipment>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DepartureTime, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveryTime, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateShipmentDto, Shipment>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.DepartureTime, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveryTime, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateShipmentDto, CreateShipmentModel>()
            .ForMember(dest => dest.DepartureTime, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveryTime, opt => opt.Ignore());
        CreateMap<CreateShipmentModel, Shipment>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateShipmentDto, UpdateShipmentModel>()
            .ForMember(dest => dest.DepartureTime, opt => opt.Ignore())
            .ForMember(dest => dest.DeliveryTime, opt => opt.Ignore());
        CreateMap<UpdateShipmentModel, Shipment>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
