using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Auth;
using Volo.Abp.Application.Services;

namespace InventoryTrackingAutomation.Services.Auth;

/// <summary>
/// Kimlik doğrulama (Authentication) uygulama servisi kontratı.
/// </summary>
public interface IAuthAppService : IApplicationService
{
    /// <summary>
    /// Kullanıcı girişi (login) işlemini gerçekleştirir.
    /// E-posta ve şifreyle token almayı sağlar.
    /// </summary>
    Task<TokenResponse> LoginAsync(LoginDto input);

    /// <summary>
    /// Yeni kullanıcı kaydı işlemini gerçekleştirir.
    /// Başarılı kayıt sonrası kayıtlı kullanıcının ID'sini döner.
    /// </summary>
    Task<Guid> RegisterAsync(RegisterDto input);

    /// <summary>
    /// Giriş yapmış mevcut kullanıcının ID'sini döner.
    /// </summary>
    Task<Guid?> GetMeAsync();
}
