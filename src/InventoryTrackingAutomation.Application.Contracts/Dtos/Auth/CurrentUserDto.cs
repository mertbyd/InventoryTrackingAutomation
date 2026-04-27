using System;
using System.Collections.Generic;

namespace InventoryTrackingAutomation.Dtos.Auth;

public class CurrentUserDto
{
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string[] Roles { get; set; }
}
