namespace InventoryTrackingAutomation.Dtos.Auth;

/// <summary>
/// Giriş sonrası dönen token yanıt DTO'su.
/// </summary>
//işlevi: TokenResponse.cs verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class TokenResponse
{
    /// <summary>
    /// Giriş yapan kullanıcının ID'si.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// JWT erişim (access) tokeni.
    /// </summary>
    public string AccessToken { get; set; }

    /// <summary>
    /// Refresh tokeni.
    /// </summary>
    public string RefreshToken { get; set; }

    /// <summary>
    /// Tokenlerin geçerlilik süresi (saniye cinsinden).
    /// </summary>
    public int ExpiresIn { get; set; }

    /// <summary>
    /// Token türü (genellikle "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "Bearer";
}
