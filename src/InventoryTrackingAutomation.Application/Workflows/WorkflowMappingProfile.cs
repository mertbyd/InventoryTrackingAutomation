using AutoMapper;
using InventoryTrackingAutomation.Dtos.Workflows;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Models.Workflows;
using Volo.Abp.AutoMapper;

namespace InventoryTrackingAutomation.Workflows;

/// <summary>
/// İş akışı entity'leri ve DTO'ları arasındaki AutoMapper konfigürasyonları.
/// </summary>
public class WorkflowMappingProfile : Profile
{
    public WorkflowMappingProfile()
    {
        // WorkflowDefinition Mappings
        CreateMap<WorkflowDefinition, WorkflowDefinitionDto>();
        CreateMap<CreateWorkflowDefinitionDto, WorkflowDefinition>()
            .IgnoreFullAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.ConcurrencyStamp, opt => opt.Ignore())
            .ForMember(dest => dest.ExtraProperties, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore());

        // WorkflowStepDefinition Mappings
        CreateMap<WorkflowStepDefinition, WorkflowStepDefinitionDto>();
        CreateMap<CreateWorkflowStepDefinitionDto, WorkflowStepDefinition>()
            .IgnoreAuditedObjectProperties()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowDefinitionId, opt => opt.Ignore())
            .ForMember(dest => dest.ResolverKey, opt => opt.Ignore())
            .ForMember(dest => dest.WorkflowDefinition, opt => opt.Ignore());

        // WorkflowInstance Mappings
        CreateMap<WorkflowInstance, WorkflowInstanceDto>();

        // WorkflowInstanceStep Mappings
        CreateMap<WorkflowInstanceStep, WorkflowInstanceStepDto>();

        // Dto to Model Mappings
        CreateMap<StartWorkflowDto, StartWorkflowModel>()
            .ForMember(dest => dest.InitiatorUserId, opt => opt.Ignore())
            .ForMember(dest => dest.InitiatorsManagerUserId, opt => opt.Ignore());
        CreateMap<ProcessApprovalDto, ProcessApprovalModel>()
            .ForMember(dest => dest.CurrentUserId, opt => opt.Ignore())
            .ForMember(dest => dest.CurrentUserRoles, opt => opt.Ignore());
    }
}
