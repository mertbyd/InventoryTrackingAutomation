namespace InventoryTrackingAutomation.Enums.Inventory;

/// <summary>
/// Bir stok kaydinin fiziksel konumunun tipi.
/// Polymorphic stock_locations tablosunda LocationId ile birlikte kullanilir.
/// Yeni tip eklemek (Transit, CustomerSite vb.) sadece bu enum'a deger eklemek demektir.
/// </summary>
public enum StockLocationTypeEnum
{
    Warehouse = 1,    // master.warehouses referansi.
    Vehicle = 2       // master.vehicles referansi.
}
