namespace InventoryTrackingAutomation;

public static class InventoryTrackingAutomationDbProperties
{
    public static string DbTablePrefix { get; set; } = "";
    public const string ConnectionStringName = "Default";

    // Schema tanımları — appsettings'ten override edilebilir
    public static string LookupSchema { get; set; } = "lookup";
    public static string MasterSchema { get; set; } = "master";
    public static string InventorySchema { get; set; } = "inventory";
    public static string OperationSchema { get; set; } = "operation";
    public static string MovementSchema { get; set; } = "movement";
    public static string WorkflowSchema { get; set; } = "workflow";
}
