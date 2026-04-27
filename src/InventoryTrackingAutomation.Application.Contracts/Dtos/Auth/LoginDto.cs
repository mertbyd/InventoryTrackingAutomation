namespace InventoryTrackingAutomation.Dtos.Auth;

/// <summary>
/// Giriş (Login) isteği DTO'su.
/// </summary>
public class LoginDto
{
    /// <summary>
    /// Kullanıcı adı.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Kullanıcı şifresi.
    /// </summary>
    public string Password { get; set; }
}
