using System;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Ürünlerin temel ölçü birimlerini (Adet, Kutu, Kg vb.) tanımlayan dinamik lookup tablosu.
/// </summary>
// işlevi: Stok ürünlerinin ölçüm birimlerini veritabanı seviyesinde tutar.
// sistemdeki görevi: UnitTypeEnum'un yerine geçen ve dinamik eklenebilen ölçü birimi tablosudur.
public class UnitType : Entity<Guid>, IPassivable
{
    public string Code { get; set; } = default!; // Ölçü biriminin kısa kodu (Örn: PIECE, KG, BOX)
    public string Name { get; set; } = default!; // Ölçü biriminin görünen adı (Örn: Adet, Kilogram)
    public string? Description { get; set; } // Birim hakkında opsiyonel açıklama
    public bool IsActive { get; set; } // Bu birimin sistemde kullanıma açık olup olmadığını belirler.

    protected UnitType() { }
    
    public UnitType(Guid id, string code, string name) : base(id)
    {
        Code = code;
        Name = name;
        IsActive = true;
    }
}
