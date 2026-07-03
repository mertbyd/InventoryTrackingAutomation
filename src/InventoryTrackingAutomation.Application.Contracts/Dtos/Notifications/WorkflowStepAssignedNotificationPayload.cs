using System;

namespace InventoryTrackingAutomation.Dtos.Notifications;

/// <summary>
/// Workflow adimi atama bildiriminin payload'u; ortak cekirdege surec baglamini ekler.
/// </summary>
// islevi: Onaycinin dogrudan ilgili workflow adimina gidebilmesi icin surec/adim kimliklerini tasir.
// sistemdeki gorevi: Workflow alanlari yalniz bu bildirimde vardir; diger bildirim tipleri bu alanlari tasimaz.
public class WorkflowStepAssignedNotificationPayload : InventoryNotificationPayload
{
    public Guid WorkflowInstanceId { get; set; } // Bildirimin ait oldugu surec kimligi.
    public Guid WorkflowInstanceStepId { get; set; } // Onay bekleyen adim kimligi.
}
