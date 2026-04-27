namespace InventoryTrackingAutomation.Shipments;

/// <summary>
/// Sevkiyat domain'i icin merkezi sabitler.
/// </summary>
public static class ShipmentNumberPrefixes
{
    // Hareket talebi onaylandiginda olusan sevkiyat numarasi prefix'i.
    public const string ApprovedMovementRequest = "SHP";
}

/// <summary>
/// Sevkiyat planlama varsayilanlari.
/// </summary>
public static class ShipmentPlanningDefaults
{
    // Onaylanan talebin varsayilan planli cikis suresi.
    public const int PlannedDepartureHours = 2;
}
