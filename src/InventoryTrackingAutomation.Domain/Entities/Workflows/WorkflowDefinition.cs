using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Dinamik iş akışı tanımlarını temsil eden entity.
/// </summary>
public class WorkflowDefinition : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// İş akışının adı (Örn: "MovementRequest"). Lookup için kullanılır.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// İş akışının okunabilir açıklaması (Örn: "Hareket Talebi İş Akışı").
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// İş akışının aktif olup olmadığı.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// İş akışının versiyon numarası.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// İş akışının içerdiği adımlar.
    /// </summary>
    public virtual ICollection<WorkflowStepDefinition> Steps { get; private set; }

    private WorkflowDefinition()
    {
    }

    public WorkflowDefinition(Guid id, string name, string? description, bool isActive, int version = 1)
        : base(id)
    {
        Name = name;
        Description = description;
        IsActive = isActive;
        Version = version;
        Steps = new List<WorkflowStepDefinition>();
    }
}
