namespace InventoryTrackingAutomation.Enums;

/// <summary>
/// Ürünlerin temel ölçü birimlerini tanımlayan enum.
/// </summary>
public enum UnitTypeEnum
{
    Piece = 1,      // Adet bazlı ürünler için kullanılır. Örnek: 5 adet kalem
    Box = 2,        // Kutulu ürünler için kullanılır. Örnek: 3 kutu vida
    Kilogram = 3,   // Ağırlık bazlı ürünler için kullanılır. Örnek: 10 kg demir
    Meter = 4,      // Uzunluk bazlı ürünler için kullanılır. Örnek: 50 metre kablo
    Liter = 5       // Hacim bazlı sıvı ürünler için kullanılır. Örnek: 20 litre yağ
}
