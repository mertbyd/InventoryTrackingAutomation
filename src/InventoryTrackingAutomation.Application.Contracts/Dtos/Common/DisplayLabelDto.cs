using System;

namespace InventoryTrackingAutomation.Dtos.Common;

/// <summary>
/// Bir FK referansının ekranda gösterilecek karşılığı (Id + Code/Name).
/// </summary>
// işlevi: Anlamsız bir FK Id'sinin yanında kullanıcının göreceği Code/Name karşılığını taşır.
// sistemdeki görevi: FE'nin Id'yi metne çevirmek için ekrandan ekstra sorgu atmasını engeller.
public class DisplayLabelDto
{
    /// <summary>
    /// Referans verilen kaydın Id'si.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Referans kaydın kurumsal kodu. Örnek: Product.Code, InventoryTask.Code.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Referans kaydın ekranda görünen adı. Name / PlateNumber / RegistrationNumber buraya map'lenir.
    /// </summary>
    public string? Name { get; set; }
}
