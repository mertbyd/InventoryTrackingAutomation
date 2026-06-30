namespace InventoryTrackingAutomation.ExceptionCodes;

public static class MovementApprovalExceptionCodes
{
    private const string MovementApprovalErrorCodesPrefix = $"MovementManagement.MovementApproval";
    public const string NotFound = $"{MovementApprovalErrorCodesPrefix}:00001";
    public const string AlreadyDecided = $"{MovementApprovalErrorCodesPrefix}:00002";
    public const string UnauthorizedApprover = $"{MovementApprovalErrorCodesPrefix}:00003";
    public const string InvalidMovementStatus = $"{MovementApprovalErrorCodesPrefix}:00004";
    public const string WorkflowNotFound = $"{MovementApprovalErrorCodesPrefix}:00005";
    public const string NoPendingApprovalStep = $"{MovementApprovalErrorCodesPrefix}:00006";
    public const string RejectionNoteRequired = $"{MovementApprovalErrorCodesPrefix}:00007";
}
