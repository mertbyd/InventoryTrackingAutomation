namespace InventoryTrackingAutomation.Enums.Tasks;

/// <summary>
/// Saha gorevinin yasam dongusu durumlari (PITON: tasks.status).
/// </summary>
public enum TaskStatusEnum
{
    Draft = 1,        // Planlama asamasinda, henuz baslamamis.
    InProgress = 2,   // Aktif yurutulen gorev — vehicle_tasks bu durumda kurulur.
    Completed = 3,    // Tamamlandi — otomatik iade tetiklenir.
    Cancelled = 4     // Iptal — atanmis araclar serbest, stok depoya iade.
}
