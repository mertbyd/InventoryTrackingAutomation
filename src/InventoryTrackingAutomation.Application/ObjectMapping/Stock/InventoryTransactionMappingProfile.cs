using AutoMapper;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Stock;

/// <summary>
/// InventoryTransaction entity, DTO ve domain model mapping profili.
/// </summary>
public class InventoryTransactionMappingProfile : Profile
{
    public InventoryTransactionMappingProfile()
    {
        CreateMap<InventoryTransaction, InventoryTransactionDto>().ReverseMap();
        CreateMap<CreateInventoryTransactionDto, CreateInventoryTransactionModel>();
        CreateMap<UpdateInventoryTransactionDto, UpdateInventoryTransactionModel>();
        CreateMap<CreateInventoryTransactionModel, InventoryTransaction>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateInventoryTransactionModel, InventoryTransaction>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
