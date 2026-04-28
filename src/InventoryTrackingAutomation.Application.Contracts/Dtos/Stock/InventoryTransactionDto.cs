using System;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Stock;

/// <summary>
/// Envanter hareketi response DTO'su.
/// </summary>
public class InventoryTransactionDto : FullAuditedEntityDto<Guid>
{
    public Guid ProductId { get; set; }                                  // Urun Id'si.
    public InventoryTransactionTypeEnum TransactionType { get; set; }    // Islem tipi.
    public int Quantity { get; set; }                                    // Miktar.
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
