using System;

namespace InventoryTrackingAutomation.Dtos.Common;

/// <summary>
/// Bir FK referansinin ekranda gosterilecek karsiligi (Id + Code/Name).
/// </summary>
// islevi: Anlamsiz bir FK Id'sinin yaninda kullanicinin gorecegi Code/Name karsiligini tasir.
// sistemdeki gorevi: FE'nin Id'yi metne cevirmek icin ekrandan ekstra sorgu atmasini engeller.
public class DisplayLabelDto
{
    /// <summary>
    /// Referans verilen kaydin Id'si.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Referans kaydin kurumsal kodu veya takip numarasidir. Ornek: InventoryTask.Code, Worker.RegistrationNumber.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// Referans kaydin ekranda gorunen adidir. Name veya Title alanlari buraya map'lenir.
    /// </summary>
    public string? Name { get; set; }
}
