using AutoMapper;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Models.Stock;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Stock;

/// <summary>
/// StockMovement entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class StockMovementMappingProfile : Profile
{
    public StockMovementMappingProfile()
    {
        CreateMap<StockMovement, StockMovementDto>().ReverseMap();
        CreateMap<CreateStockMovementDto, StockMovement>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateStockMovementDto, StockMovement>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateStockMovementDto, CreateStockMovementModel>();
        CreateMap<CreateStockMovementModel, StockMovement>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateStockMovementDto, UpdateStockMovementModel>();
        CreateMap<UpdateStockMovementModel, StockMovement>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
