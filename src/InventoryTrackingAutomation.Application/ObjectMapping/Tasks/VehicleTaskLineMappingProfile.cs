using AutoMapper;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Tasks;

/// <summary>
/// VehicleTaskLine entity, DTO ve domain model mapping profili.
/// </summary>
public class VehicleTaskLineMappingProfile : Profile
{
    public VehicleTaskLineMappingProfile()
    {
        CreateMap<VehicleTaskLine, VehicleTaskLineDto>()
            .ForMember(dest => dest.ProductId, opt => opt.Ignore());
        CreateMap<CreateVehicleTaskLineDto, CreateVehicleTaskLineModel>()
            .ForMember(dest => dest.VehicleTaskId, opt => opt.Ignore());
        CreateMap<UpdateVehicleTaskLineDto, UpdateVehicleTaskLineModel>();
        CreateMap<ReceiveVehicleTaskLineDto, ReceiveVehicleTaskLineModel>()
            .ForMember(dest => dest.VehicleTaskLineId, opt => opt.Ignore());
        CreateMap<CreateVehicleTaskLineModel, VehicleTaskLine>()
            .IgnoreAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ReceivedQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.DamagedQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.LostQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.ConsumedQuantity, opt => opt.Ignore())
            .ForMember(dest => dest.ReceiveNote, opt => opt.Ignore());
        CreateMap<ReceiveVehicleTaskLineModel, VehicleTaskLine>()
            .IgnoreAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.VehicleTaskId, opt => opt.Ignore())
            .ForMember(dest => dest.TaskLineId, opt => opt.Ignore())
            .ForMember(dest => dest.AllocatedQuantity, opt => opt.Ignore());
    }
}
