using System;

namespace InventoryTrackingAutomation.Managers.Workflows.Approvers;

// Onaycı çözümleme stratejilerine geçirilen tüm bağlamı tek noktada toplayan kayıt.
// EntityType/EntityId entity-bazlı resolver'lar için, InitiatorUserId initiator-bazlı resolver için kullanılır.
public sealed record ApproverContext(string EntityType, Guid EntityId, Guid InitiatorUserId);
