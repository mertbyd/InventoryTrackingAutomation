using System;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Sistemdeki araç tiplerini (Kamyon, Minibüs vb.) tanımlayan dinamik lookup tablosu.
/// </summary>
// işlevi: Araçların tiplerini kategori olarak tutar ve dinamik eklenebilir olmasını sağlar.
// sistemdeki görevi: VehicleTypeEnum'un yerine geçen ve UI'dan yönetilebilen referans tablosudur.
public class VehicleType : Entity<Guid>, IPassivable
{
    public string Code { get; set; } = default!; // Araç tipinin kısa kodu (Örn: TRUCK, VAN)
    public string Name { get; set; } = default!; // Araç tipinin görünen adı (Örn: Kamyon, Minibüs)
    public string? Description { get; set; } // Tip hakkında opsiyonel açıklama
    public bool IsActive { get; set; } // Bu araç tipinin sistemde seçilebilir olup olmadığını belirler.

    protected VehicleType() { }

    public VehicleType(Guid id, string code, string name) : base(id)
    {
        Code = code;
        Name = name;
        IsActive = true;
    }
}
