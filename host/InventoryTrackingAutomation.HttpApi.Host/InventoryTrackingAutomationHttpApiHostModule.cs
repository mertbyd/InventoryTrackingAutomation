using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi;
using OpenIddict.Validation.AspNetCore;
using InventoryTrackingAutomation.EntityFrameworkCore;
using InventoryTrackingAutomation.MultiTenancy;
using InventoryTrackingAutomation.SignalR;
using Microsoft.AspNetCore.Extensions.DependencyInjection;
using SystemStandards.Extensions;
using SystemStandards.Abp;
using SystemStandards.Abp.Extensions;
using Volo.Abp;
using Volo.Abp.Account;
using Volo.Abp.Account.Web;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.AspNetCore.Mvc.UI.MultiTenancy;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Basic;
using Volo.Abp.AspNetCore.Mvc.UI.Theme.Shared;
using Volo.Abp.AspNetCore.Serilog;
using Volo.Abp.AspNetCore.SignalR;
using Volo.Abp.Auditing;
using Volo.Abp.AuditLogging.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Caching;
using Volo.Abp.Data;
using Volo.Abp.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore.PostgreSql;
using Volo.Abp.FeatureManagement;
using Volo.Abp.FeatureManagement.EntityFrameworkCore;
using Volo.Abp.Identity;
using Volo.Abp.Identity.EntityFrameworkCore;
using Volo.Abp.Localization;
using Volo.Abp.Modularity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.OpenIddict;
using Volo.Abp.OpenIddict.EntityFrameworkCore;
using Volo.Abp.PermissionManagement;
using Volo.Abp.PermissionManagement.EntityFrameworkCore;
using Volo.Abp.PermissionManagement.HttpApi;
using Volo.Abp.PermissionManagement.Identity;
using Volo.Abp.PermissionManagement.OpenIddict;
using Volo.Abp.SettingManagement;
using Volo.Abp.SettingManagement.EntityFrameworkCore;
using Volo.Abp.Swashbuckle;
using Volo.Abp.TenantManagement;
using Volo.Abp.TenantManagement.EntityFrameworkCore;
using Volo.Abp.UI.Navigation.Urls;
using Volo.Abp.VirtualFileSystem;

namespace InventoryTrackingAutomation;

[DependsOn(
    // Core modules
    typeof(SystemStandardsAbpModule),
    typeof(InventoryTrackingAutomationApplicationModule),
    typeof(InventoryTrackingAutomationEntityFrameworkCoreModule),
    typeof(InventoryTrackingAutomationHttpApiModule),
    
    // Framework modules
    typeof(AbpAutofacModule),
    typeof(AbpAspNetCoreMvcUiMultiTenancyModule),
    typeof(AbpAspNetCoreMvcUiBasicThemeModule),
    typeof(AbpAspNetCoreSerilogModule),
    typeof(AbpAspNetCoreSignalRModule),
    typeof(AbpSwashbuckleModule),
   
    // Database
    typeof(AbpEntityFrameworkCorePostgreSqlModule),
    
    // Account & Authentication - OpenIddict
    typeof(AbpAccountWebOpenIddictModule),
    typeof(AbpAccountHttpApiModule),
    typeof(AbpAccountApplicationModule),
    
    // OpenIddict
    typeof(AbpOpenIddictEntityFrameworkCoreModule),
    
    // Identity
    typeof(AbpIdentityEntityFrameworkCoreModule),
    typeof(AbpIdentityApplicationModule),
    typeof(AbpIdentityHttpApiModule),
    
    // Permission Management
    typeof(AbpPermissionManagementEntityFrameworkCoreModule),
    typeof(AbpPermissionManagementApplicationModule),
    typeof(AbpPermissionManagementHttpApiModule),
    typeof(AbpPermissionManagementDomainIdentityModule),
    typeof(AbpPermissionManagementDomainOpenIddictModule),
    
    // Setting Management
    typeof(AbpSettingManagementEntityFrameworkCoreModule),
    typeof(AbpSettingManagementApplicationModule),
    typeof(AbpSettingManagementHttpApiModule),
    
    // Feature Management
    typeof(AbpFeatureManagementEntityFrameworkCoreModule),
    typeof(AbpFeatureManagementApplicationModule),
    typeof(AbpFeatureManagementHttpApiModule),
    
    // Tenant Management
    typeof(AbpTenantManagementEntityFrameworkCoreModule),
    
    // Audit Logging
    typeof(AbpAuditLoggingEntityFrameworkCoreModule)
)]
public class InventoryTrackingAutomationHttpApiHostModule : AbpModule
{
    public override void PreConfigureServices(ServiceConfigurationContext context)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        
        PreConfigure<OpenIddictBuilder>(builder =>
        {
            builder.AddValidation(options =>
            {
                options.AddAudiences("InventoryTrackingAutomation");
                options.UseLocalServer();
                options.UseAspNetCore();
            });
        });

        // Geliştirme ortamında HTTP üzerinden token alabilmek için HTTPS zorunluluğunu kaldır
        PreConfigure<OpenIddictServerBuilder>(builder =>
        {
            builder.UseAspNetCore()
                .DisableTransportSecurityRequirement();
        });
    }
    
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        var hostingEnvironment = context.Services.GetHostingEnvironment();
        var configuration = context.Services.GetConfiguration();

        // SystemStandards services are now registered via SystemStandardsAbpModule

        context.Services.AddSingleton<InventorySignalRDebugNotificationStore>();

        // CRITICAL: API isteklerinde Bearer kullanıldığı için CSRF/Antiforgery filtresini kapatıyoruz
        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.AutoValidate = false;
        });

        // CRITICAL: Forward Identity Authentication for Bearer tokens
        // This is what ensures that tokens are correctly recognized in an integrated environment
        context.Services.ForwardIdentityAuthenticationForBearer(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);

        // /api/* path'lerinde auth başarısız olunca 302 → /Account/Login yerine 401/403 dön
        context.Services.ConfigureApplicationCookie(options =>
        {
            options.Events.OnRedirectToLogin = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                    ctx.Response.StatusCode = 401;
                else
                    ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
            options.Events.OnRedirectToAccessDenied = ctx =>
            {
                if (ctx.Request.Path.StartsWithSegments("/api"))
                    ctx.Response.StatusCode = 403;
                else
                    ctx.Response.Redirect(ctx.RedirectUri);
                return Task.CompletedTask;
            };
        });

        Configure<AbpDbContextOptions>(options =>
        {
            options.UseNpgsql();
        });
        
        Configure<AbpMultiTenancyOptions>(options =>
        {
            options.IsEnabled = MultiTenancyConsts.IsEnabled;
        });
        
        if (hostingEnvironment.IsDevelopment())
        {
            Configure<AbpVirtualFileSystemOptions>(options =>
            {
                options.FileSets.ReplaceEmbeddedByPhysical<InventoryTrackingAutomationDomainSharedModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, 
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}InventoryTrackingAutomation.Domain.Shared"));
                options.FileSets.ReplaceEmbeddedByPhysical<InventoryTrackingAutomationDomainModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, 
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}InventoryTrackingAutomation.Domain"));
                options.FileSets.ReplaceEmbeddedByPhysical<InventoryTrackingAutomationApplicationContractsModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, 
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}InventoryTrackingAutomation.Application.Contracts"));
                options.FileSets.ReplaceEmbeddedByPhysical<InventoryTrackingAutomationApplicationModule>(
                    Path.Combine(hostingEnvironment.ContentRootPath, 
                    $"..{Path.DirectorySeparatorChar}..{Path.DirectorySeparatorChar}src{Path.DirectorySeparatorChar}InventoryTrackingAutomation.Application"));
            });
        }
        
        context.Services.AddAbpSwaggerGenWithOAuth(
            configuration["AuthServer:Authority"]!,
            new Dictionary<string, string>
            {
                {"InventoryTrackingAutomation", "InventoryTrackingAutomation API"}
            },
            options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo() { Title = "InventoryTrackingAutomation API", Version = "v1" });
                options.DocInclusionPredicate((docName, description) => true);
                options.CustomSchemaIds(type => type.FullName);
                
                // HTTP/Bearer scheme — Swagger UI 'Bearer ' prefix'ini OTOMATİK ekler
                // Kullanıcı sadece access_token'ı yapıştırır, "Bearer" yazmasına gerek yok
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Sadece JWT access_token'ı yapıştırın (Bearer prefix'i otomatik eklenir)."
                });

                // Microsoft.OpenApi 2.x: OpenApiSecuritySchemeReference, AddSecurityRequirement(Func<doc, req>)
                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecuritySchemeReference("Bearer", document),
                        new List<string>()
                    }
                });
            });
        
        Configure<AbpLocalizationOptions>(options =>
        {
            options.Languages.Add(new LanguageInfo("ar", "ar", "العربية"));
            options.Languages.Add(new LanguageInfo("cs", "cs", "Čeština"));
            options.Languages.Add(new LanguageInfo("en", "en", "English"));
            options.Languages.Add(new LanguageInfo("en-GB", "en-GB", "English (UK)"));
            options.Languages.Add(new LanguageInfo("fi", "fi", "Finnish"));
            options.Languages.Add(new LanguageInfo("fr", "fr", "Français"));
            options.Languages.Add(new LanguageInfo("hi", "hi", "Hindi"));
            options.Languages.Add(new LanguageInfo("is", "is", "Icelandic"));
            options.Languages.Add(new LanguageInfo("it", "it", "Italiano"));
            options.Languages.Add(new LanguageInfo("hu", "hu", "Magyar"));
            options.Languages.Add(new LanguageInfo("pt-BR", "pt-BR", "Português"));
            options.Languages.Add(new LanguageInfo("ro-RO", "ro-RO", "Română"));
            options.Languages.Add(new LanguageInfo("ru", "ru", "Русский"));
            options.Languages.Add(new LanguageInfo("sk", "sk", "Slovak"));
            options.Languages.Add(new LanguageInfo("tr", "tr", "Türkçe"));
            options.Languages.Add(new LanguageInfo("zh-Hans", "zh-Hans", "简体中文"));
            options.Languages.Add(new LanguageInfo("zh-Hant", "zh-Hant", "繁體中文"));
            options.Languages.Add(new LanguageInfo("de-DE", "de-DE", "Deutsch"));
            options.Languages.Add(new LanguageInfo("es", "es", "Español"));
            options.Languages.Add(new LanguageInfo("el", "el", "Ελληνικά"));
        });
        
        Configure<AbpDistributedCacheOptions>(options =>
        {
            options.KeyPrefix = "InventoryTrackingAutomation:";
        });
        
        // Data Protection — Redis yok, sadece local
        context.Services.AddDataProtection().SetApplicationName("InventoryTrackingAutomation");
        
        context.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(builder =>
            {
                builder
                    .WithOrigins(
                        configuration["App:CorsOrigins"]?
                            .Split(",", StringSplitOptions.RemoveEmptyEntries)
                            .Select(o => o.RemovePostFix("/"))
                            .ToArray() ?? Array.Empty<string>()
                    )
                    .WithAbpExposedHeaders()
                    .SetIsOriginAllowedToAllowWildcardSubdomains()
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });
        
        Configure<AbpAuditingOptions>(options =>
        {
            options.IsEnabled = true;
            options.EntityHistorySelectors.AddAllEntities();
        });
    }
    
    public override async Task OnApplicationInitializationAsync(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();
        var env = context.GetEnvironment();
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseHsts();
        }
        
        // app.UseHttpsRedirection(); // Geliştirme ortamında HTTP kullanıyoruz
        app.UseCorrelationId();
        app.UseSystemStandardsAspNetCore();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseSystemStandardsAbp(); // ABP spesifik zenginleştirmeler (örn. UserId, TenantId çekimi)
        app.UseAbpOpenIddictValidation();
        
        if (MultiTenancyConsts.IsEnabled)
        {
            app.UseMultiTenancy();
        }
        
        app.UseAbpRequestLocalization();
        app.UseAuthorization();
        app.UseSwagger();
        app.UseAbpSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryTrackingAutomation API");
            var configuration = context.ServiceProvider.GetRequiredService<IConfiguration>();
            options.OAuthClientId(configuration["AuthServer:SwaggerClientId"]);
            options.OAuthScopes("InventoryTrackingAutomation");
        });
        app.UseAuditing();
        app.UseAbpSerilogEnrichers();
        app.UseConfiguredEndpoints();
        
        await SeedDataAsync(context);
    }
    
    private async Task SeedDataAsync(ApplicationInitializationContext context)
    {
        using (var scope = context.ServiceProvider.CreateScope())
        {
            await scope.ServiceProvider
                .GetRequiredService<IDataSeeder>()
                .SeedAsync();
        }
    }
}
