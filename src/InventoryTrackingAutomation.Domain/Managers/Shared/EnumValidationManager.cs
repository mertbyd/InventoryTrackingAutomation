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
/// Proje genelindeki Enum do�rulama i�lemlerini SettingProvider �zerinden
/// dinamik ve merkezi olarak y�neten servis.
/// </summary>
//i�levi: EnumValidation etki alan� (domain) kurallar�n� ve karma��k veri b�t�nl���n� sa�lar.
//sistemdeki g�revi: Domain katman�ndaki i� kurallar�n�n merkezi y�netimini ve validasyonunu sa�lar.
public class EnumValidationManager : InventoryTrackingAutomationDomainService
{
    public EnumValidationManager(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private ISettingProvider _settingProvider => LazyGetRequiredService<ISettingProvider>();



    /// <summary>
    /// Verilen Enum de�erinin belirtilen ayar ad�ndaki izin verilen de�erler
    /// listesinde olup olmad���n� kontrol eder.
    /// </summary>
    /// <typeparam name="TEnum">Enum tipi</typeparam>
    /// <param name="enumValue">Kontrol edilecek Enum de�eri</param>
    /// <param name="settingName">�zin verilen de�erlerin tutuldu�u Setting ad� (�rn: InventoryTrackingAutomationSettings.Workflows.AllowedStates)</param>
    public async Task ValidateAllowedEnumAsync<TEnum>(TEnum enumValue, string settingName) where TEnum : struct, Enum
    {
        var allowedValuesStr = await _settingProvider.GetOrNullAsync(settingName);
        if (string.IsNullOrWhiteSpace(allowedValuesStr))
        {
            throw new BusinessException(GeneralExceptionCodes.InvalidOperation);
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
            throw new BusinessException(GeneralExceptionCodes.InvalidEnumValue);
        }
    }
}

