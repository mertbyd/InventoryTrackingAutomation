using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using InventoryTrackingAutomation.Dtos.Auth;
using InventoryTrackingAutomation.Managers.Auth;
using InventoryTrackingAutomation.Models.Auth;
using InventoryTrackingAutomation.Services.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Logging;
using Volo.Abp;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Auth;

/// <summary>
/// Kimlik doğrulama (Authentication) uygulama servisi — Login ve Register işlemlerini orkestre eder.
/// AppService katmanı, Domain Service'ten gelen iş mantığını HTTP (OpenIddict) ile birleştirir.
/// </summary>
[RemoteService(IsEnabled = false)]
//işlevi: Auth iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class AuthAppService : InventoryTrackingAutomationAppService, IAuthAppService
{
    // Domain manager — kullanıcı oluşturma ve giriş doğrulama iş kuralları.
    private readonly AuthManager _authManager;
    // OpenIddict token endpoint çağrısı için HTTP client factory.
    private readonly IHttpClientFactory _httpClientFactory;
    // SelfUrl, client_id gibi token çağrısı parametreleri için.
    private readonly IConfiguration _configuration;
    // HTTP token akışındaki hataları loglamak için.
    private readonly ILogger<AuthAppService> _logger;
    // DTO ↔ Model dönüşümü için.
    private readonly IMapper _mapper;

    // Tüm bağımlılıkları DI ile alır.
    public AuthAppService(
        AuthManager authManager,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<AuthAppService> logger,
        IMapper mapper)
    {
        _authManager = authManager;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _mapper = mapper;
    }

    /// <summary>
    /// Yeni kullanıcı kaydı işlemini gerçekleştirir.
    /// DTO → Model mapping, AuthManager.CreateUserAsync çağrısı, kullanıcı ID'sini döner.
    /// </summary>
    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<Guid> RegisterAsync(RegisterDto input)
    {
        // DTO → Model mapping
        var registerModel = _mapper.Map<RegisterModel>(input);

        // Domain servisi çağrı (CreateUserAsync)
        var user = await _authManager.CreateUserAsync(registerModel);

        // Oluşturulan kullanıcının ID'sini döner
        return user.Id;
    }

    /// <summary>
    /// Kullanıcı girişi (login) işlemini gerçekleştirir.
    /// Kimlik doğrulama (AuthManager) + OpenIddict token istek kombinasyonu.
    /// </summary>
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<TokenResponse> LoginAsync(LoginDto input)
    {
        // DTO → Model mapping
        var loginModel = _mapper.Map<LoginModel>(input);
        // Domain servisi çağrı (Kimlik doğrulama)
        var user = await _authManager.ValidateLoginAsync(loginModel);
        // OpenIddict token endpoint'i çağrısı
        var tokenResponse = await GetTokenFromOpenIddictAsync(input.UserName, input.Password);
        if (tokenResponse == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.Auth.TokenRequestFailed);
        }
        // Token response'a kullanıcı ID'sini ekle
        tokenResponse.UserId = user.Id;
        return tokenResponse;
    }

    /// <summary>
    /// Giriş yapmış mevcut kullanıcının ID'sini döner.
    /// </summary>
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<Guid?> GetMeAsync()
    {
        return CurrentUser.Id;
    }

    /// <summary>
    /// OpenIddict token endpoint'ine HTTP istek yaparak token alır.
    /// Grant type: password (Resource Owner Password Credentials flow)
    /// </summary>
    private async Task<TokenResponse> GetTokenFromOpenIddictAsync(string username, string password)
    {
        // Configuration'dan base URL al
        var baseUrl = _configuration["App:SelfUrl"] ?? "https://localhost:5000";
        // HTTP client oluştur
        var httpClient = _httpClientFactory.CreateClient();
        // Token istek parametreleri
        var tokenRequest = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "password"),
            new KeyValuePair<string, string>("username", username),
            new KeyValuePair<string, string>("password", password),
            new KeyValuePair<string, string>("client_id", "InventoryTrackingAutomation_App"),
            new KeyValuePair<string, string>("scope", "openid profile email roles InventoryTrackingAutomation")
        });

        try
        {
            // OpenIddict token endpoint'ine POST isteği
            var response = await httpClient.PostAsync($"{baseUrl}/connect/token", tokenRequest);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Token request başarısız: {response.StatusCode}");
                return null;
            }

            // Response JSON'ı parse et
            var tokenResponseJson = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenResponseJson, options);

            // TokenResponse nesnesi oluştur
            return new TokenResponse
            {
                AccessToken = tokenData.GetProperty("access_token").GetString(),
                TokenType = tokenData.GetProperty("token_type").GetString(),
                ExpiresIn = tokenData.GetProperty("expires_in").GetInt32(),
                RefreshToken = tokenData.TryGetProperty("refresh_token", out var refreshToken)
                    ? refreshToken.GetString()
                    : null
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"OpenIddict token istek hatası: {ex.Message}");
            return null;
        }
    }
}
