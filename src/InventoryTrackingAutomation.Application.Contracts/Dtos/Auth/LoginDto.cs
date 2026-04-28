namespace InventoryTrackingAutomation.Dtos.Auth;

/// <summary>
/// Giriş (Login) isteği DTO'su.
/// </summary>
//işlevi: Login verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
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
