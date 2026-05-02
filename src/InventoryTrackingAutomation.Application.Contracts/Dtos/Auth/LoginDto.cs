namespace InventoryTrackingAutomation.Dtos.Auth;

//işlevi: Login verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class LoginDto
{
    /// <summary>
    /// UserName alanı.
    /// </summary>
    public string UserName { get; set; }
    /// <summary>
    /// Password alanı.
    /// </summary>
    public string Password { get; set; }
}
