using System;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Services;
using Volo.Abp.Settings;

namespace InventoryTrackingAutomation.Managers.Shared;

/// <summary>
/// Proje genelindeki Enum doğrulama işlemlerini SettingProvider üzerinden
/// dinamik ve merkezi olarak yöneten servis.
/// </summary>
public class EnumValidationManager : DomainService
{
    private readonly ISettingProvider _settingProvider;

    public EnumValidationManager(ISettingProvider settingProvider)
    {
        _settingProvider = settingProvider;
    }

    /// <summary>
    /// Verilen Enum değerinin belirtilen ayar adındaki izin verilen değerler
    /// listesinde olup olmadığını kontrol eder.
    /// </summary>
    /// <typeparam name="TEnum">Enum tipi</typeparam>
    /// <param name="enumValue">Kontrol edilecek Enum değeri</param>
    /// <param name="settingName">İzin verilen değerlerin tutulduğu Setting adı (Örn: InventoryTrackingAutomationSettings.Workflows.AllowedStates)</param>
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
