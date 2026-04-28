namespace InventoryTrackingAutomation;

/// <summary>
/// Projedeki tum domain error code'larini merkezi olarak tutan sabit sinif.
/// </summary>
public static class InventoryTrackingAutomationErrorCodes
{
    private const string Prefix = "InventoryTrackingAutomation";

    public static class Products
    {
        public const string NotFound = Prefix + ":Product.NotFound";
        public const string AlreadyExists = Prefix + ":Product.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Product.CodeNotUnique";
    }

    public static class ProductCategories
    {
        public const string NotFound = Prefix + ":ProductCategory.NotFound";
        public const string AlreadyExists = Prefix + ":ProductCategory.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":ProductCategory.CodeNotUnique";
    }

    public static class Departments
    {
        public const string NotFound = Prefix + ":Department.NotFound";
        public const string AlreadyExists = Prefix + ":Department.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Department.CodeNotUnique";
    }

    public static class Warehouses
    {
        public const string NotFound = Prefix + ":Warehouse.NotFound";
        public const string AlreadyExists = Prefix + ":Warehouse.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Warehouse.CodeNotUnique";
    }

    public static class Vehicles
    {
        public const string NotFound = Prefix + ":Vehicle.NotFound";
        public const string AlreadyExists = Prefix + ":Vehicle.AlreadyExists";
        public const string PlateNumberNotUnique = Prefix + ":Vehicle.PlateNumberNotUnique";
    }

    public static class Workers
    {
        public const string NotFound = Prefix + ":Worker.NotFound";
        public const string AlreadyExists = Prefix + ":Worker.AlreadyExists";
        public const string SelfAssignmentNotAllowed = Prefix + ":Worker.SelfAssignmentNotAllowed";
    }

    public static class MovementRequests
    {
        public const string NotFound = Prefix + ":MovementRequest.NotFound";
        public const string InvalidStatus = Prefix + ":MovementRequest.InvalidStatus";
        public const string RequestNumberNotUnique = Prefix + ":MovementRequest.RequestNumberNotUnique";
    }

    public static class MovementRequestLines
    {
        public const string NotFound = Prefix + ":MovementRequestLine.NotFound";
    }

    public static class MovementApprovals
    {
        public const string NotFound = Prefix + ":MovementApproval.NotFound";
        public const string AlreadyDecided = Prefix + ":MovementApproval.AlreadyDecided";
        public const string UnauthorizedApprover = Prefix + ":MovementApproval.UnauthorizedApprover";
        public const string InvalidMovementStatus = Prefix + ":MovementApproval.InvalidMovementStatus";
        public const string WorkflowNotFound = Prefix + ":MovementApproval.WorkflowNotFound";
        public const string NoPendingApprovalStep = Prefix + ":MovementApproval.NoPendingApprovalStep";
    }

    public static class InventoryTasks
    {
        public const string NotFound = Prefix + ":InventoryTask.NotFound";
        public const string CodeNotUnique = Prefix + ":InventoryTask.CodeNotUnique";
    }

    public static class VehicleTasks
    {
        public const string NotFound = Prefix + ":VehicleTask.NotFound";
        public const string VehicleAlreadyAssigned = Prefix + ":VehicleTask.VehicleAlreadyAssigned";
    }

    public static class StockLocations
    {
        public const string NotFound = Prefix + ":StockLocation.NotFound";
        public const string InvalidLocation = Prefix + ":StockLocation.InvalidLocation";
        public const string InsufficientStock = Prefix + ":StockLocation.InsufficientStock";
    }

    public static class InventoryTransactions
    {
        public const string NotFound = Prefix + ":InventoryTransaction.NotFound";
        public const string InvalidTransfer = Prefix + ":InventoryTransaction.InvalidTransfer";
    }

    public static class Workflows
    {
        public const string DefinitionNotFound = Prefix + ":Workflow.DefinitionNotFound";
        public const string InstanceNotFound = Prefix + ":Workflow.InstanceNotFound";
        public const string StepNotFound = Prefix + ":Workflow.StepNotFound";
        public const string UnauthorizedApproval = Prefix + ":Workflow.UnauthorizedApproval";
        public const string InstanceNotActive = Prefix + ":Workflow.InstanceNotActive";
    }

    public static class Auth
    {
        public const string UserNameAlreadyExists = Prefix + ":Auth.UserNameAlreadyExists";
        public const string EmailAlreadyExists = Prefix + ":Auth.EmailAlreadyExists";
        public const string InvalidCredentials = Prefix + ":Auth.InvalidCredentials";
        public const string PasswordMismatch = Prefix + ":Auth.PasswordMismatch";
        public const string UserCreationFailed = Prefix + ":Auth.UserCreationFailed";
        public const string RoleNotFound = Prefix + ":Auth.RoleNotFound";
        public const string RoleAssignmentFailed = Prefix + ":Auth.RoleAssignmentFailed";
        public const string TokenRequestFailed = Prefix + ":Auth.TokenRequestFailed";
        public const string MissingConfiguration = Prefix + ":Auth.MissingConfiguration";
    }

    public static class General
    {
        public const string InvalidOperation = Prefix + ":General.InvalidOperation";
        public const string NotAuthorized = Prefix + ":General.NotAuthorized";
        public const string InvalidEnumValue = Prefix + ":General.InvalidEnumValue";
        public const string SoftDeleteNotSupported = "Entity:SoftDeleteNotSupported";
    }
}
