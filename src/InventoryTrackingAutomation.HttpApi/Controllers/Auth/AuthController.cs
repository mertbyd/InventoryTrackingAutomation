using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InventoryTrackingAutomation.Dtos.Auth;
using InventoryTrackingAutomation.Services.Auth;
using InventoryTrackingAutomation.Permissions;

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
    private readonly IAuthAppService _authAppService;

    public AuthController(IAuthAppService authAppService)
    {
        _authAppService = authAppService;
    }

    /// <summary>
    /// Kullanıcı girişi (Login) — E-posta ve şifreyle JWT token almayı sağlar.
    /// Herkese açık endpoint (kimlik doğrulama gerekmez).
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<TokenResponse> Login([FromBody] LoginDto input)
    {
        var token = await _authAppService.LoginAsync(input);
        return token;
    }

    /// <summary>
    /// Kullanıcı kaydı (Register) — Yeni kullanıcı oluşturur ve kullanıcı ID'sini döner.
    /// Herkese açık endpoint (kimlik doğrulama gerekmez).
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<Guid> Register([FromBody] RegisterDto input)
    {
        var userId = await _authAppService.RegisterAsync(input);
        return userId;
    }

    /// <summary>
    /// Giriş yapmış mevcut kullanıcının ID'sini döner.
    /// [Authorize] ile korunur ve JwtBearer şemasını kullanır.
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<Guid?> GetMe()
    {
        return await _authAppService.GetMeAsync();
    }
}
