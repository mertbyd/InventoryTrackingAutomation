using System;
using InventoryTrackingAutomation.Enums;

namespace InventoryTrackingAutomation.Models.Stock;

/// <summary>
/// Envanter hareketi olusturma domain modeli.
/// </summary>
public class CreateInventoryTransactionModel
{
    public Guid ProductId { get; set; }                                  // Hareket eden urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Islem tipi.
    public int Quantity { get; set; }                                    // Hareket miktari.
    public InventoryLocationTypeEnum? SourceLocationType { get; set; }   // Kaynak lokasyon tipi.
    public Guid? SourceWarehouseSiteId { get; set; }                     // Kaynak depo Id'si.
    public Guid? SourceVehicleId { get; set; }                           // Kaynak arac Id'si.
    public InventoryLocationTypeEnum? TargetLocationType { get; set; }   // Hedef lokasyon tipi.
    public Guid? TargetWarehouseSiteId { get; set; }                     // Hedef depo Id'si.
    public Guid? TargetVehicleId { get; set; }                           // Hedef arac Id'si.
    public Guid? MovementRequestId { get; set; }                         // Bagli talep Id'si.
    public Guid? VehicleTaskId { get; set; }                             // Bagli arac-gorev Id'si.
    public DateTime OccurredAt { get; set; }                             // Hareket zamani.
    public string? Note { get; set; }                                    // Islem notu.
}
