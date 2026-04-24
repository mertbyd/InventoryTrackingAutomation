using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Lookups;

/// <summary>
/// ProductCategory entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class ProductCategoryMappingProfile : Profile
{
    public ProductCategoryMappingProfile()
    {
        CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
        CreateMap<CreateProductCategoryDto, ProductCategory>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateProductCategoryDto, ProductCategory>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateProductCategoryDto, CreateProductCategoryModel>();
        CreateMap<CreateProductCategoryModel, ProductCategory>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateProductCategoryDto, UpdateProductCategoryModel>();
        CreateMap<UpdateProductCategoryModel, ProductCategory>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
