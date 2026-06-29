using AutoMapper;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Models.Inventory;

namespace InventoryTrackingAutomation.ObjectMapping.Stock;

/// <summary>
/// InventoryTransaction entity, DTO ve domain model mapping profili.
/// </summary>
public class InventoryTransactionMappingProfile : Profile
{
    public InventoryTransactionMappingProfile()
    {
        CreateMap<InventoryTransaction, InventoryTransactionDto>().ReverseMap();
        CreateMap<CreateInventoryTransactionDto, CreateInventoryTransactionModel>()
            .ForMember(dest => dest.PerformedByUserId, opt => opt.Ignore());
        CreateMap<CreateInventoryTransactionModel, InventoryTransaction>()
            .ForMember(dest => dest.CreationTime, opt => opt.Ignore())
            .ForMember(dest => dest.CreatorId, opt => opt.Ignore())
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
