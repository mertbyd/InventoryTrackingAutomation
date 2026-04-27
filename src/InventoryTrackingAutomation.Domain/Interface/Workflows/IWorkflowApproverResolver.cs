using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Managers.Workflows.Approvers;

namespace InventoryTrackingAutomation.Interface.Workflows;

/// <summary>
/// Dinamik iş akışlarında, ilgili adımın onaycısının "kim" olduğunu çözümleyen (resolve eden) arayüz.
/// Strategy registry üzerinden ResolverKey → IApproverStrategy haritasını çalıştırır.
/// </summary>
public interface IWorkflowApproverResolver
{
    /// <summary>
    /// Belirtilen adımın onaycısının User Id'sini döner; çözülemezse null döner.
    /// </summary>
    /// <param name="context">Onaycı çözümleme bağlamı (entity, initiator).</param>
    /// <param name="stepDefinition">Çözümlenecek adımın tanımı (ResolverKey burada).</param>
    Task<Guid?> ResolveApproverAsync(ApproverContext context, WorkflowStepDefinition stepDefinition);
}
