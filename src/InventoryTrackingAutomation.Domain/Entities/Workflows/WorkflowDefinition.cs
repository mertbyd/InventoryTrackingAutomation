using System;
using System.Collections.Generic;
using Volo.Abp.Domain.Entities.Auditing;

namespace InventoryTrackingAutomation.Entities.Workflows;

/// <summary>
/// Dinamik is akisi tanimini temsil eden aggregate root.
/// </summary>
public class WorkflowDefinition : FullAuditedAggregateRoot<Guid>
{
    /// <summary>
    /// Is akisinin teknik adini tasir. Ornek: MovementRequest.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Is akisinin kullanici tarafindan okunabilir aciklamasini tasir.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Is akisi taniminin yeni surecler icin kullanilip kullanilmayacagini belirler.
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Is akisi taniminin surum bilgisini tasir.
    /// </summary>
    public int Version { get; private set; }

    /// <summary>
    /// Workflow entity'leri mevcut mimaride navigation kullanan istisnai kume oldugu icin
    /// EF konfigurasyonlari adim iliskisini bu koleksiyon uzerinden kurar.
    /// </summary>
    public virtual ICollection<WorkflowStepDefinition> Steps { get; private set; }

    private WorkflowDefinition()
    {
        Name = string.Empty;
        Steps = new List<WorkflowStepDefinition>();
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
