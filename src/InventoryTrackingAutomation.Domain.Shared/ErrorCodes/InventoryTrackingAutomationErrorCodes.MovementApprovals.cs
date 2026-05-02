namespace InventoryTrackingAutomation;

public static partial class InventoryTrackingAutomationErrorCodes
{
    public static class MovementApprovals
    {
        public const string NotFound = Prefix + ":MovementApproval.NotFound";
        public const string AlreadyDecided = Prefix + ":MovementApproval.AlreadyDecided";
        public const string UnauthorizedApprover = Prefix + ":MovementApproval.UnauthorizedApprover";
        public const string InvalidMovementStatus = Prefix + ":MovementApproval.InvalidMovementStatus";
        public const string WorkflowNotFound = Prefix + ":MovementApproval.WorkflowNotFound";
        public const string NoPendingApprovalStep = Prefix + ":MovementApproval.NoPendingApprovalStep";
        public const string RejectionNoteRequired = Prefix + ":MovementApproval.RejectionNoteRequired";
    }
}
