namespace InventoryTrackingAutomation.Dtos.Auth;

//işlevi: TokenResponse.cs verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TokenResponse
{
    /// <summary>
    /// UserId alanı.
    /// </summary>
    public Guid UserId { get; set; }
    /// <summary>
    /// AccessToken alanı.
    /// </summary>
    public string AccessToken { get; set; }
    /// <summary>
    /// RefreshToken alanı.
    /// </summary>
    public string RefreshToken { get; set; }
    /// <summary>
    /// ExpiresIn alanı.
    /// </summary>
    public int ExpiresIn { get; set; }

    public string TokenType { get; set; } = "Bearer";
}
