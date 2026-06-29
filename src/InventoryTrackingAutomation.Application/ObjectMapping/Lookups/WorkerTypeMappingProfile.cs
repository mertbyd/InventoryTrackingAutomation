using AutoMapper;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.ObjectMapping.Lookups;

// islevi: WorkerType entity, DTO ve domain model donusumlerini tanimlar.
// sistemdeki gorevi: AppService ve Manager katmanlarinda manuel property kopyalama tekrarini engeller.
public class WorkerTypeMappingProfile : Profile
{
    public WorkerTypeMappingProfile()
    {
        CreateMap<WorkerType, WorkerTypeDto>().ReverseMap();
        CreateMap<CreateWorkerTypeDto, CreateWorkerTypeModel>();
        CreateMap<UpdateWorkerTypeDto, UpdateWorkerTypeModel>();
        CreateMap<CreateWorkerTypeModel, WorkerType>().ForMember(dest => dest.Id, opt => opt.Ignore());
        CreateMap<UpdateWorkerTypeModel, WorkerType>().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
