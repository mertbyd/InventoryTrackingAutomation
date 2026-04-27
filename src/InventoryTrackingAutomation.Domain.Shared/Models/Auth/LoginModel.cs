namespace InventoryTrackingAutomation.Models.Auth;

/// <summary>
/// Giriş (Login) işlemi için domain model.
/// </summary>
public class LoginModel
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
