using System;
using System.Threading.Tasks;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Bir WorkflowStepDefinition.ResolverKey değerine karşılık gelen onaycı çözümleme stratejisi.
// Yeni onaycı türleri eklemek için sadece bu interface implement edilir; resolver/manager dosyalarına dokunulmaz (OCP).
public interface IApproverStrategy
{
    // Bu stratejinin yanıt verdiği ResolverKey (WorkflowResolverKeys ile eşleşmeli).
    string Key { get; }

    // Verilen bağlamda onaycının User Id'sini çözer; çözülemezse null döner.
    Task<Guid?> ResolveAsync(ApproverContext context);
}
