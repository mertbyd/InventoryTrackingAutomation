using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Auth;

//işlevi: CurrentUser verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CurrentUserDto
{
    /// <summary>
    /// Id alanı.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// UserName alanı.
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// Name alanı.
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Email alanı.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Roles alanı.
    /// </summary>
    public string[] Roles { get; set; }
}
