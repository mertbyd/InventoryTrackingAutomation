using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Inventory;

//işlevi: ProductStockSummary verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class ProductStockSummaryDto
{
    /// <summary>
    /// Ozetlenen urun Id&apos;si.
    /// </summary>
    public Guid ProductId { get; set; }                                          // Ozetlenen urun Id'si.
    /// <summary>
    /// Toplam stok miktari.
    /// </summary>
    public int TotalQuantity { get; set; }                                       // Toplam stok miktari.
    /// <summary>
    /// Depolardaki toplam miktar.
    /// </summary>
    public int WarehouseQuantity { get; set; }                                   // Depolardaki toplam miktar.
    /// <summary>
    /// Araclardaki toplam miktar.
    /// </summary>
    public int VehicleQuantity { get; set; }                                     // Araclardaki toplam miktar.
    /// <summary>
    /// Aktif gorevlerdeki toplam miktar.
    /// </summary>
    public int ActiveTaskQuantity { get; set; }                                  // Aktif gorevlerdeki toplam miktar.
    public List<ProductStockLocationSummaryDto> Locations { get; set; } = new(); // Lokasyon detaylari.

    public string ProductName { get; set; }
}

