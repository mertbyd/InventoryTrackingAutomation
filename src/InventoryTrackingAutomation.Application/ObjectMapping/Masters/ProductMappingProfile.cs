using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Masters;

/// <summary>
/// Product entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>().ReverseMap();

        CreateMap<CreateProductDto, Product>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<UpdateProductDto, Product>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateProductDto, CreateProductModel>()
            .ForMember(dest => dest.MinimumStockLevel, opt => opt.Ignore());
        CreateMap<CreateProductModel, Product>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<UpdateProductDto, UpdateProductModel>()
            .ForMember(dest => dest.MinimumStockLevel, opt => opt.Ignore());
        CreateMap<UpdateProductModel, Product>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
