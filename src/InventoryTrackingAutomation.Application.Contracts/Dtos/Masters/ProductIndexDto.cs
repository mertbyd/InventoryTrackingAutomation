using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Elasticsearch'teki urun arama dokumani; okuma-tarafi projeksiyondur, domain entity degildir.
/// </summary>
// islevi: Urunun aranabilir alanlarini ve FE'nin listede gosterecegi denormalize etiketleri tasir.
// sistemdeki gorevi: ProductSearchManager tarafindan yazilir, arama endpoint'i tarafindan okunur; PostgreSQL karar verisi olarak kullanilmaz.
public class ProductIndexDto : EntityDto<Guid>
{
    public string Code { get; set; } = default!; // Urun kodu; birebir eslesme agirlikli aranir.
    public string Name { get; set; } = default!; // Urun adi; fuzzy metin aramasinin ana alanidir.
    public Guid? CategoryId { get; set; } // Bagli kategori Id'si (Guid lookup Id).
    public string? CategoryName { get; set; } // Kategori adinin okuma-tarafi denormalize kopyasi.
    public Guid UnitTypeId { get; set; } // Olcu birimi lookup Id'si (Guid lookup Id).
    public string? UnitTypeName { get; set; } // Olcu birimi adinin okuma-tarafi denormalize kopyasi.
}
