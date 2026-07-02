using InventoryTrackingAutomation.Dtos.Search;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Urun metin aramasi giris sozlesmesi.
/// </summary>
// islevi: Urun aramasina ozel giris parametrelerini tasir; su an ortak keyword/sayfalama yeterlidir.
// sistemdeki gorevi: Urun aramasina filtre (kategori, aktiflik vb.) eklenecekse alanlar buraya gelir, base bozulmaz.
public class ProductSearchInputDto : SearchInputDto
{
}
