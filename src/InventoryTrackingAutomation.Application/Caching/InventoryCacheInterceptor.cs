using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Volo.Abp.DynamicProxy;

namespace InventoryTrackingAutomation.Application.Caching;

/// <summary>
/// InventoryCacheAttribute ile isaretlenen async okuma metotlari icin cache-aside akisini yurutur.
/// </summary>
// islevi: Once Redis'ten okur, cache yoksa metodu calistirir ve donen sonucu Redis'e yazar.
// sistemdeki gorevi: AppService'leri thin tutarak manuel cache kod tekrarini tek cross-cutting noktada toplar.
public class InventoryCacheInterceptor : IAbpInterceptor
{
    // Key sablonunda {id} gibi cozulmemis alan kalirsa hatayi sessiz gecmek yerine acik gostermek icin kullanilir.
    private static readonly Regex UnresolvedTokenRegex = new(@"\{[^{}]+\}", RegexOptions.Compiled);
    private readonly IDistributedCache _cache;

    public InventoryCacheInterceptor(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task InterceptAsync(IAbpMethodInvocation invocation)
    {
        var attribute = FindCacheAttribute(invocation);
        var resultType = GetTaskResultType(invocation.Method.ReturnType);

        // Attribute yoksa veya metod Task<T> donmuyorsa normal ABP akisini bozmadan devam edilir.
        if (attribute is null || resultType is null)
        {
            await invocation.ProceedAsync();
            return;
        }

        var cacheKey = BuildCacheKey(attribute.KeyTemplate, invocation);
        var cached = await _cache.GetStringAsync(cacheKey);

        // Cache hit: JSON Redis degerini metodun gercek donus tipine cevirip DB yoluna girmeden doner.
        if (cached is not null)
        {
            var cachedResult = JsonSerializer.Deserialize(cached, resultType);
            if (cachedResult is not null)
            {
                invocation.ReturnValue = cachedResult;
                return;
            }
        }

        // Cache miss: asil AppService metodu calisir; manager/repository/mapping davranisi aynen korunur.
        await invocation.ProceedAsync();

        if (invocation.ReturnValue is null)
        {
            return;
        }

        // Metot sonucu ayni key ile Redis'e yazilir; invalidation tarafindaki CacheKeys ayni sablonu kullanir.
        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(invocation.ReturnValue, resultType),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(attribute.AbsoluteExpirationMinutes)
            });
    }

    /// <summary>
    /// Attribute'i hem interface/proxy metodunda hem de gercek implementation metodunda arar.
    /// </summary>
    private static InventoryCacheAttribute? FindCacheAttribute(IAbpMethodInvocation invocation)
    {
        var attribute = invocation.Method.GetCustomAttribute<InventoryCacheAttribute>();
        if (attribute is not null)
        {
            return attribute;
        }

        var parameterTypes = invocation.Method.GetParameters()
            .Select(parameter => parameter.ParameterType)
            .ToArray();

        if (invocation.TargetObject is null)
        {
            return null;
        }

        var targetMethod = invocation.TargetObject.GetType()
            .GetMethod(invocation.Method.Name, parameterTypes);

        return targetMethod?.GetCustomAttribute<InventoryCacheAttribute>();
    }

    /// <summary>
    /// Sadece Task&lt;T&gt; donen okuma metotlari cache'lenir; void Task akisi cache disinda kalir.
    /// </summary>
    private static Type? GetTaskResultType(Type returnType)
    {
        return returnType.IsGenericType && returnType.GetGenericTypeDefinition() == typeof(Task<>)
            ? returnType.GetGenericArguments()[0]
            : null;
    }

    /// <summary>
    /// Attribute sablonundaki {id} veya {0} alanlarini metod argumanlari ile doldurur.
    /// </summary>
    private static string BuildCacheKey(string template, IAbpMethodInvocation invocation)
    {
        var key = template;
        var parameters = invocation.Method.GetParameters();

        for (var i = 0; i < parameters.Length; i++)
        {
            var value = Convert.ToString(invocation.Arguments[i], CultureInfo.InvariantCulture) ?? string.Empty;
            key = key
                .Replace("{" + i + "}", value, StringComparison.Ordinal)
                .Replace("{" + parameters[i].Name + "}", value, StringComparison.Ordinal);
        }

        if (UnresolvedTokenRegex.IsMatch(key))
        {
            throw new InvalidOperationException($"Cache key template '{template}' contains an unresolved parameter token.");
        }

        return key;
    }
}
