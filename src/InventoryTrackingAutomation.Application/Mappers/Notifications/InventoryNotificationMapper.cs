using Riok.Mapperly.Abstractions;
using InventoryTrackingAutomation.Dtos.Notifications;
using InventoryTrackingAutomation.Events.Movements;
using InventoryTrackingAutomation.Events.Workflows;

namespace InventoryTrackingAutomation.Application.Mappers.Notifications;

// islevi: Domain event'lerini (ETO) client'a giden bildirim payload'larina cevirir.
// sistemdeki gorevi: Bildirim tipinin sabit metinleri (Type/Title/Message) MapValue ile merkezi sabitlerden baglanir; CreatedAt'i handler base'i damgalar.
// MapValue ifadeleri generated koda yazildigi gibi kopyalanir; bu yuzden sabitler tam nitelenmis (fully qualified) durur.
[Mapper]
public partial class InventoryNotificationMapper
{
    [MapValue(nameof(WorkflowStepAssignedNotificationPayload.Type), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Types.WorkflowStepAssigned)]
    [MapValue(nameof(WorkflowStepAssignedNotificationPayload.Title), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Messages.WorkflowStepAssignedTitle)]
    [MapValue(nameof(WorkflowStepAssignedNotificationPayload.Message), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Messages.WorkflowStepAssignedMessage)]
    [MapperIgnoreSource(nameof(WorkflowStepAssignedEto.WorkflowStepDefinitionId))]
    [MapperIgnoreSource(nameof(WorkflowStepAssignedEto.AssignedUserId))]
    [MapperIgnoreTarget(nameof(WorkflowStepAssignedNotificationPayload.CreatedAt))]
    public partial WorkflowStepAssignedNotificationPayload MapToPayload(WorkflowStepAssignedEto source);

    [MapValue(nameof(WarehouseStockDispatchedNotificationPayload.Type), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Types.WarehouseStockDispatched)]
    [MapValue(nameof(WarehouseStockDispatchedNotificationPayload.Title), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Messages.WarehouseStockDispatchedTitle)]
    [MapValue(nameof(WarehouseStockDispatchedNotificationPayload.Message), InventoryTrackingAutomation.Notifications.InventoryNotificationConstants.Messages.WarehouseStockDispatchedMessage)]
    [MapValue(nameof(WarehouseStockDispatchedNotificationPayload.EntityType), nameof(InventoryTrackingAutomation.Entities.Movements.MovementRequest))]
    [MapProperty(nameof(WarehouseStockDispatchedEto.MovementRequestId), nameof(WarehouseStockDispatchedNotificationPayload.EntityId))]
    [MapperIgnoreTarget(nameof(WarehouseStockDispatchedNotificationPayload.CreatedAt))]
    public partial WarehouseStockDispatchedNotificationPayload MapToPayload(WarehouseStockDispatchedEto source);
}
