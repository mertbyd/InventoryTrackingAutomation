namespace InventoryTrackingAutomation.Dtos.Auth;

/// <summary>
/// Kayıt (Register) isteği DTO'su.
/// </summary>
public class RegisterDto
{
    /// <summary>
    /// Kullanıcı adı.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Kullanıcı e-posta adresi.
    /// </summary>
    public string Email { get; set; }

    /// <summary>
    /// Kullanıcı şifresi.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Şifre doğrulama (confirmation).
    /// </summary>
    public string PasswordConfirm { get; set; }
}
