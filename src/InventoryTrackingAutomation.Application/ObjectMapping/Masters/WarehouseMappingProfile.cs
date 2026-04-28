using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Masters;

/// <summary>
/// Warehouse entity, DTO ve domain modelleri arasindaki map kurallarini tutar.
/// </summary>
public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<Warehouse, WarehouseDto>().ReverseMap();
        CreateMap<CreateWarehouseDto, CreateWarehouseModel>();
        CreateMap<UpdateWarehouseDto, UpdateWarehouseModel>();
        CreateMap<CreateWarehouseModel, Warehouse>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateWarehouseModel, Warehouse>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
