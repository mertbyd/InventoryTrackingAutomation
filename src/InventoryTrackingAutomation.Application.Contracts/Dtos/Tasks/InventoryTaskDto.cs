using System;
using System.Collections.Generic;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using System;
using Volo.Abp.Application.Dtos;
using InventoryTrackingAutomation.Dtos.Common;

namespace InventoryTrackingAutomation.Dtos.Tasks;

//işlevi: InventoryTask verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class InventoryTaskDto : EnrichedEntityDto<Guid>
{
    /// <summary>
    /// Gorev surec tipi.
    /// </summary>
    public InventoryTaskTypeEnum Type { get; set; }       // Gorev surec tipi.
    /// <summary>
    /// Gorev kodu.
    /// </summary>
    public string Code { get; set; }                 // Gorev kodu.
    /// <summary>
    /// Gorev adi.
    /// </summary>
    public string Name { get; set; }                 // Gorev adi.
    /// <summary>
    /// Gorev bolgesi.
    /// </summary>
    public string? Region { get; set; }               // Gorev bolgesi.
    /// <summary>
    /// Baslangic tarihi.
    /// </summary>
    public DateTime StartDate { get; set; }          // Baslangic tarihi.
    /// <summary>
    /// Bitis tarihi.
    /// </summary>
    public DateTime? EndDate { get; set; }           // Bitis tarihi.
    /// <summary>
    /// Gorev durumu.
    /// </summary>
    public TaskStatusEnum Status { get; set; } // Gorev durumu.
    /// <summary>
    /// Gorev aciklamasi.
    /// </summary>
    public string? Description { get; set; }         // Gorev aciklamasi.
    /// <summary>
    /// Operasyon malzemesinin cikacagi kaynak depo.
    /// </summary>
    public Guid SourceWarehouseId { get; set; }      // Operasyon malzemesinin cikacagi kaynak depo.
    /// <summary>
    /// Depo transferlerinde malzemenin gidecegi hedef depo.
    /// </summary>
    public Guid? TargetWarehouseId { get; set; }     // Depo transferlerinde hedef depo.
    /// <summary>
    /// Gorev bitince iadenin donecegi depo.
    /// </summary>
    public Guid? ReturnWarehouseId { get; set; }     // Gorev bitince iadenin donecegi depo.
    /// <summary>
    /// Gorev kalemleri. Detayli sorgularda dolu gelir.
    /// </summary>
    public List<TaskLineDto>? Lines { get; set; }    // Gorev kalemleri.
    public string SourceWarehouseName { get; set; }
    public string TargetWarehouseName { get; set; }
    public string ReturnWarehouseName { get; set; }
}
