using AutoMapper;
using InventoryTrackingAutomation.Managers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Volo.Abp.Identity;
using Volo.Abp;
using InventoryTrackingAutomation.Models.Auth;
using InventoryTrackingAutomation.Roles;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Auth;

// Kimlik doğrulama domain servisi — kullanıcı oluşturma ve giriş doğrulama iş kurallarını yönetir.
//işlevi: Auth etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class AuthManager : InventoryTrackingAutomationDomainService
{
    public AuthManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // ABP Identity user manager — CRUD ve şifre işlemleri için.
    private IdentityUserManager _identityUserManager => LazyGetRequiredService<IdentityUserManager>();
    // Identity user repository — direkt query için.
    private IIdentityUserRepository _identityUserRepository => LazyGetRequiredService<IIdentityUserRepository>();
    // Identity role manager — rol atama işlemleri için.
    private IdentityRoleManager _identityRoleManager => LazyGetRequiredService<IdentityRoleManager>();
    // Operasyonel log için.
    private ILogger<AuthManager> _logger => LazyGetRequiredService<ILogger<AuthManager>>();

    // Tüm bağımlılıkları DI ile alır.
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    // Yeni kullanıcı oluşturur — email/username uniqueness kontrolü, şifre eşleşme doğrulaması ve varsayılan rol ataması yapar.
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<IdentityUser> CreateUserAsync(RegisterModel model)
    {
        // Email ve username uniqueness kontrolü.
        await ValidateEmailUniquenessAsync(model.Email);
        await ValidateUsernameUniquenessAsync(model.UserName);

        // Şifre ve şifre tekrarı eşleşmeli.
        if (model.Password != model.PasswordConfirm)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.PasswordMismatch);
        }

        // IdentityUser entity'si oluştur ve ABP üzerinden persist et.
        var userId = GuidGenerator.Create();
        var user = new IdentityUser(userId, model.UserName, model.Email);
        var result = await _identityUserManager.CreateAsync(user, model.Password);

        // IdentityResult başarısızsa hataları exception WithData'ya geç (sebep kaybolmasın).
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.UserCreationFailed)
                .WithData("Errors", errors);
        }

        // Varsayılan rolü ata (FieldWorker).
        await _identityUserManager.AddToRoleAsync(user, InventoryTrackingAutomationRoleConstants.FieldWorker);
        return user;
    }

    // Kullanıcı giriş bilgilerini doğrular — username + password eşleşmesi yoksa InvalidCredentials atar.
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<IdentityUser> ValidateLoginAsync(LoginModel model)
    {
        // Username ile kullanıcıyı bul.
        var user = await _identityUserManager.FindByNameAsync(model.UserName);
        if (user == null)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.InvalidCredentials);

        // Şifreyi doğrula.
        var isPasswordValid = await _identityUserManager.CheckPasswordAsync(user, model.Password);
        if (!isPasswordValid)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.InvalidCredentials);

        return user;
    }

    // E-posta adresinin sistem içinde benzersiz olduğunu doğrular.
    private async Task ValidateEmailUniquenessAsync(string email)
    {
        var existingUser = await _identityUserManager.FindByEmailAsync(email);
        if (existingUser != null)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.EmailAlreadyExists);
    }

    // Username'in sistem içinde benzersiz olduğunu doğrular.
    private async Task ValidateUsernameUniquenessAsync(string userName)
    {
        var existingUser = await _identityUserManager.FindByNameAsync(userName);
        if (existingUser != null)
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.UserNameAlreadyExists);
    }
}
