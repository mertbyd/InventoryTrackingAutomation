namespace InventoryTrackingAutomation.Dtos.Auth;

/// <summary>
/// Kayıt (Register) isteği DTO'su.
/// </summary>
//işlevi: Register verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
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
