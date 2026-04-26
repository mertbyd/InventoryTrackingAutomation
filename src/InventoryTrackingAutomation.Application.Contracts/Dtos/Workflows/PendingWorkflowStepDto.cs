using System;
using InventoryTrackingAutomation.Enums.Workflows;

namespace InventoryTrackingAutomation.Dtos.Workflows;

/// <summary>
/// Mevcut kullanıcının onaylaması bekleyen iş akışı adımını dönen entity-agnostic DTO.
/// </summary>
public class PendingWorkflowStepDto
{
    /// <summary>İş akışı adım Id'si.</summary>
    public Guid WorkflowInstanceStepId { get; set; }

    /// <summary>Bağlı olduğu iş akışı süreci Id'si.</summary>
    public Guid WorkflowInstanceId { get; set; }

    /// <summary>İş akışına bağlı entity türü (Örn: "MovementRequest").</summary>
    public string EntityType { get; set; }

    /// <summary>İş akışına bağlı entity Id'si.</summary>
    public Guid EntityId { get; set; }

    /// <summary>Adım sırası (1, 2, 3...).</summary>
    public int StepOrder { get; set; }

    /// <summary>Adım adı — RequiredRoleName veya ResolverKey.</summary>
    public string StepName { get; set; }

    /// <summary>İş akışını başlatan kullanıcı Id'si.</summary>
    public Guid InitiatorUserId { get; set; }

    /// <summary>Step'in oluşturulma zamanı.</summary>
    public DateTime CreatedAt { get; set; }
}
