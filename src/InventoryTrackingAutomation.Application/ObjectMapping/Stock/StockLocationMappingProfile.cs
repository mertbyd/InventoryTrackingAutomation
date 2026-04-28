using AutoMapper;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Stock;

/// <summary>
/// StockLocation entity, DTO ve domain model mapping profili.
/// </summary>
public class StockLocationMappingProfile : Profile
{
    public StockLocationMappingProfile()
    {
        CreateMap<StockLocation, StockLocationDto>().ReverseMap();
        CreateMap<CreateStockLocationDto, CreateStockLocationModel>();
        CreateMap<UpdateStockLocationDto, UpdateStockLocationModel>();
        CreateMap<CreateStockLocationModel, StockLocation>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateStockLocationModel, StockLocation>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
