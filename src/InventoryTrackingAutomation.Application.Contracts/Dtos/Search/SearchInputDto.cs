using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Search;

/// <summary>
/// Metin aramasi yapan tum endpoint'lerin ortak giris sozlesmesi.
/// </summary>
// islevi: Arama anahtar kelimesini ve sayfalama parametrelerini tasir.
// sistemdeki gorevi: Yeni bir modul arama endpoint'i eklendiginde bu base kalitilir; keyword/sayfalama alanlari tekrar yazilmaz.
public class SearchInputDto : PagedAndSortedResultRequestDto
{
    public string? Keyword { get; set; } // Aranacak serbest metin; bos ise tum kayitlar sayfali doner.
}
