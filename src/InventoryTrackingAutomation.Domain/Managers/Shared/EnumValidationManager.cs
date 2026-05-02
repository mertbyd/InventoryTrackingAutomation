using System;
using InventoryTrackingAutomation.Managers;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Shared;

/// <summary>
/// Proje genelindeki Enum doðrulama iþlemlerini SettingProvider üzerinden
/// dinamik ve merkezi olarak yöneten servis.
/// </summary>
//iþlevi: EnumValidation etki alaný (domain) kurallarýný ve karmaþýk veri bütünlüðünü saðlar.
//sistemdeki görevi: Domain katmanýndaki iþ kurallarýnýn merkezi yönetimini ve validasyonunu saðlar.
public class EnumValidationManager : InventoryTrackingAutomationDomainService
{
    public EnumValidationManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private ISettingProvider _settingProvider => LazyGetRequiredService<ISettingProvider>();



    /// <summary>
    /// Verilen Enum deðerinin belirtilen ayar adýndaki izin verilen deðerler
    /// listesinde olup olmadýðýný kontrol eder.
    /// </summary>
    /// <typeparam name="TEnum">Enum tipi</typeparam>
    /// <param name="enumValue">Kontrol edilecek Enum deðeri</param>
    /// <param name="settingName">Ýzin verilen deðerlerin tutulduðu Setting adý (Örn: InventoryTrackingAutomationSettings.Workflows.AllowedStates)</param>
    public async Task ValidateAllowedEnumAsync<TEnum>(TEnum enumValue, string settingName) where TEnum : struct, Enum
    {
        var allowedValuesStr = await _settingProvider.GetOrNullAsync(settingName);
        if (string.IsNullOrWhiteSpace(allowedValuesStr))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidOperation)
                .WithData("Message", $"Missing setting configuration for '{settingName}'");
        }

        var intValue = Convert.ToInt32(enumValue);
        
        var allowedInts = allowedValuesStr
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => int.TryParse(x, out var parsed) ? parsed : (int?)null)
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .ToList();

        if (!allowedInts.Contains(intValue))
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.General.InvalidEnumValue)
                .WithData("EnumType", typeof(TEnum).Name)
                .WithData("InvalidValue", enumValue.ToString())
                .WithData("AllowedValues", allowedValuesStr);
        }
    }
}
