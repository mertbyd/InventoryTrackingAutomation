using System;
using Volo.Abp.Domain.Entities;

namespace InventoryTrackingAutomation.Entities.Lookups;

/// <summary>
/// Sistemdeki çalışan tiplerini (Beyaz Yaka, Mavi Yaka vb.) tanımlayan dinamik lookup tablosu.
/// </summary>
// işlevi: Çalışanların tiplerini (rollerini) tutar ve UI üzerinden güncellenebilir olmasını sağlar.
// sistemdeki görevi: WorkerTypeEnum'un yerine geçen referans tablosudur.
public class WorkerType : Entity<Guid>, IPassivable
{
    public string Code { get; set; } = default!; // Çalışan tipinin kısa kodu (Örn: WHITE_COLLAR)
    public string Name { get; set; } = default!; // Çalışan tipinin görünen adı (Örn: Beyaz Yaka)
    public string? Description { get; set; } // Tip hakkında opsiyonel açıklama
    public bool IsActive { get; set; } // Bu çalışan tipinin sistemde aktif olup olmadığını belirler.

    protected WorkerType() { }
    
    public WorkerType(Guid id, string code, string name) : base(id)
    {
        Code = code;
        Name = name;
        IsActive = true;
    }
}
