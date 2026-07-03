using System;

namespace InventoryTrackingAutomation.Events.Movements;

/// <summary>
/// Bir hareket talebinin sevkiyati tamamlandiginda firlatilan Event Transfer Object.
/// </summary>
[Serializable]
//işlevi: WarehouseStockDispatched olayı gerçekleştiğinde taşınacak olan veriyi tanımlar.
//sistemdeki görevi: Sevkiyat kaç ürün satırı içerirse içersin dispatch başına tek olay taşır; bildirim tarafı satır sayısından bağımsız çalışır.
public class WarehouseStockDispatchedEto
{
    public Guid MovementRequestId { get; set; }
    public Guid SourceWarehouseId { get; set; }
}
