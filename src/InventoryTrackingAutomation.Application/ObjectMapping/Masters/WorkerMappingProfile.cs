using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Models.Masters;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.ObjectMapping.Masters;

/// <summary>
/// Worker entity'si ile DTO ve Model'lar arasındaki AutoMapper mapping profili.
/// </summary>
public class WorkerMappingProfile : Profile
{
    public WorkerMappingProfile()
    {
        CreateMap<Worker, WorkerDto>().ReverseMap();
        CreateMap<CreateWorkerDto, Worker>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();
        CreateMap<UpdateWorkerDto, Worker>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ReverseMap();

        CreateMap<CreateWorkerDto, CreateWorkerModel>();
        CreateMap<CreateWorkerModel, Worker>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateWorkerDto, UpdateWorkerModel>();
        CreateMap<UpdateWorkerModel, Worker>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
