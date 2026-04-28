using System;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Movements;

// Onay bekleyen talep domain modeli — Manager'dan Service'e taşınan veri taşıyıcı.
public class PendingApprovalModel
{
    public Guid MovementRequestId { get; set; }                 // Onay bekleyen hareket talebi Id'si
    public Guid WorkflowInstanceStepId { get; set; }            // Bu kullanıcıya atanmış workflow step Id'si
    public string RequestNumber { get; set; }                   // Talep numarası. Örnek: "MR-2024-00123"
    public string SourceWarehouseName { get; set; }                  // Kaynak lokasyon adı
    public string TargetWarehouseName { get; set; }                  // Hedef lokasyon adı
    public int CurrentStepOrder { get; set; }                   // Bu adımın iş akışı içindeki sırası
    public string CurrentStepName { get; set; }                 // Adım adı (RequiredRoleName veya ResolverKey)
    public DateTime CreatedAt { get; set; }                     // Step oluşturma tarihi
    public DateTime PlannedDate { get; set; }                   // Talep edilen teslim tarihi
    public string RequestNote { get; set; }                     // Talep gerekçesi
    public MovementPriorityEnum Priority { get; set; }          // Öncelik
}
