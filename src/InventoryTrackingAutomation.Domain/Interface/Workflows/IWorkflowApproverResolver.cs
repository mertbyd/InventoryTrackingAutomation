using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;

namespace InventoryTrackingAutomation.Interface.Workflows;

/// <summary>
/// Dinamik iş akışlarında, ilgili adımın onaycısının "kim" olduğunu çözümleyen (resolve eden) arayüz.
/// Bu sayede Workflow motoru, Envanter (Worker, Site) bağımlılıklarından izole edilir.
/// </summary>
public interface IWorkflowApproverResolver
{
    /// <summary>
    /// Belirtilen adımın onaycısının Id'sini döner.
    /// </summary>
    /// <param name="entityType">İşleme giren entity tipi (Örn: "MovementRequest")</param>
    /// <param name="entityId">İşleme giren entity'nin Id'si</param>
    /// <param name="stepDefinition">Çözümlenecek adımın tanımı</param>
    /// <returns>Onaycı Kullanıcı Id'si (Eğer çözülemezse null)</returns>
    Task<Guid?> ResolveApproverAsync(string entityType, Guid entityId, WorkflowStepDefinition stepDefinition);
}
