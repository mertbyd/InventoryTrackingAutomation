namespace InventoryTrackingAutomation.Dtos.Auth;

//işlevi: Register verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class RegisterDto
{
    /// <summary>
    /// UserName alanı.
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// Email alanı.
    /// </summary>
    public string Email { get; set; }
    /// <summary>
    /// Password alanı.
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// PasswordConfirm alanı.
    /// </summary>
    public string PasswordConfirm { get; set; }
}
