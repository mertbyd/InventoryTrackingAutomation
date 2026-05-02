using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryTrackingAutomation.Dtos.Auth;
using InventoryTrackingAutomation.Services.Auth;
using InventoryTrackingAutomation.Permissions;
using SystemStandards.Results;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Controllers.Auth;

/// <summary>
/// Kimlik doğrulama (Authentication) endpoint'leri — Login ve Register işlemlerini sağlar.
/// </summary>
[Route("api/auth")]
[ApiController]
[IgnoreAntiforgeryToken]
[ApiExplorerSettings(GroupName = "Auth")]
public class AuthController : InventoryTrackingAutomationController
{
    public AuthController(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IAuthAppService _authAppService => LazyGetRequiredService<IAuthAppService>();

    /// <summary>
    /// Kullanıcı adı ve parola ile access token alır.
    /// </summary>
    /// <param name="input">Giriş bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   UserName (string) → Kullanıcı adı
    ///   Password (string) → Parola
    /// }
    /// Response {
    ///   UserId (Guid)           → Sistemdeki kullanıcı Id'si
    ///   AccessToken (string)    → Bearer JWT token
    ///   RefreshToken (string)   → Yenileme token'ı
    ///   ExpiresIn (int)         → Token geçerlilik süresi (saniye)
    /// }
    /// </remarks>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<Result<TokenResponse>> Login([FromBody] LoginDto input)
    {
        var token = await _authAppService.LoginAsync(input);
        return Result<TokenResponse>.Success(token);
    }

    /// <summary>
    /// Yeni kullanıcı kaydı oluşturur.
    /// </summary>
    /// <param name="input">Kayıt bilgileri.</param>
    /// <remarks>
    /// Request {
    ///   UserName        (string) → Kullanıcı adı
    ///   Email           (string) → E-posta adresi
    ///   Password        (string) → Parola
    ///   PasswordConfirm (string) → Parola tekrarı
    /// }
    /// Response {
    ///   (Guid) → Oluşturulan kullanıcının Id'si
    /// }
    /// </remarks>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<Result<Guid>> Register([FromBody] RegisterDto input)
    {
        var userId = await _authAppService.RegisterAsync(input);
        return Result<Guid>.Success(userId);
    }

    /// <summary>
    /// Oturumdaki kullanıcının sistemdeki kullanıcı Id'sini getirir.
    /// </summary>
    /// <remarks>
    /// Response {
    ///   (Guid?) → Oturumdaki kullanıcı Id'si
    /// }
    /// </remarks>
    [HttpGet("me")]
    [Authorize]
    public async Task<Result<Guid?>> GetMe()
    {
        var userId = await _authAppService.GetMeAsync();
        return Result<Guid?>.Success(userId);
    }
}
