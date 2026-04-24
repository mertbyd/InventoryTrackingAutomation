using AutoMapper;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Models.Stock;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Stock;

/// <summary>
/// ProductStock entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class ProductStockMappingProfile : Profile
{
    public ProductStockMappingProfile()
    {
        CreateMap<ProductStock, ProductStockDto>().ReverseMap();
        CreateMap<CreateProductStockDto, ProductStock>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateProductStockDto, ProductStock>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<CreateProductStockDto, CreateProductStockModel>();
        CreateMap<CreateProductStockModel, ProductStock>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateProductStockDto, UpdateProductStockModel>();
        CreateMap<UpdateProductStockModel, ProductStock>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
