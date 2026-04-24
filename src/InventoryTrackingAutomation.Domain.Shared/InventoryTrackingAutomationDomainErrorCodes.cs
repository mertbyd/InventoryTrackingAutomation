namespace InventoryTrackingAutomation;

/// <summary>
/// Projedeki tüm domain error code'larını merkezi olarak tutan sabit sınıf.
/// </summary>
public static class InventoryTrackingAutomationDomainErrorCodes
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

    public static class Sites
    {
        public const string NotFound = Prefix + ":Site.NotFound";
        public const string AlreadyExists = Prefix + ":Site.AlreadyExists";
        public const string CodeNotUnique = Prefix + ":Site.CodeNotUnique";
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

    public static class ProductStocks
    {
        public const string NotFound = Prefix + ":ProductStock.NotFound";
        public const string InsufficientStock = Prefix + ":ProductStock.InsufficientStock";
        public const string AlreadyExistsForProductAndSite = Prefix + ":ProductStock.AlreadyExistsForProductAndSite";
    }

    public static class StockMovements
    {
        public const string NotFound = Prefix + ":StockMovement.NotFound";
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
    }

    public static class Shipments
    {
        public const string NotFound = Prefix + ":Shipment.NotFound";
        public const string InvalidStatus = Prefix + ":Shipment.InvalidStatus";
        public const string ShipmentNumberNotUnique = Prefix + ":Shipment.ShipmentNumberNotUnique";
    }

    public static class ShipmentLines
    {
        public const string NotFound = Prefix + ":ShipmentLine.NotFound";
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
