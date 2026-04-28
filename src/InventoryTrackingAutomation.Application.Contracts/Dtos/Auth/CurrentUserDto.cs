using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Auth;

//işlevi: CurrentUser verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class CurrentUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}
