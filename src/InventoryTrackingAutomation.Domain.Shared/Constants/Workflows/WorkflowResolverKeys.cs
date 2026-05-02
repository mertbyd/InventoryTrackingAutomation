namespace InventoryTrackingAutomation.Workflows;

/// <summary>
/// Workflow step onaycisi cozmek icin kullanilan resolver key sabitlerini merkezi olarak tutar.
/// </summary>
public static class WorkflowResolverKeys
{
    // Is akisini baslatan kullanicinin dogrudan yoneticisini onayci olarak cozer.
    public const string InitiatorManager = "InitiatorManager";

    // Is akisina bagli entity'nin kaynak lokasyonu (Warehouse) yoneticisini onayci olarak cozer.
    public const string SourceWarehouseManager = "SourceWarehouseManager";

    // Is akisina bagli entity'nin hedef lokasyonu (Warehouse) yoneticisini onayci olarak cozer.
    public const string TargetWarehouseManager = "TargetWarehouseManager";

    // Lojistik biriminden yetkili birini onayci olarak cozer.
    public const string LogisticsManager = "LogisticsManager";
}
