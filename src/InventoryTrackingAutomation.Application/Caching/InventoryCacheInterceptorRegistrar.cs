using System;
using System.Linq;
using System.Reflection;
using Volo.Abp.Collections;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Caching;

/// <summary>
/// InventoryCacheAttribute kullanan servisleri ABP interceptor zincirine ekler.
/// </summary>
// islevi: Servis kayit aninda cache attribute'u olan implementation tiplerini bulur.
// sistemdeki gorevi: Tum AppService'lere interceptor eklemek yerine yalnizca cache isteyen servisleri proxy'ler.
public static class InventoryCacheInterceptorRegistrar
{
    public static void RegisterIfNeeded(IOnServiceRegistredContext context)
    {
        // Attribute olmayan servislerin proxy zincirine cache interceptor eklenmez.
        if (HasCacheAttribute(context.ImplementationType))
        {
            context.Interceptors.TryAdd<InventoryCacheInterceptor>();
        }
    }

    /// <summary>
    /// Public instance metotlarda InventoryCacheAttribute var mi kontrol eder.
    /// </summary>
    private static bool HasCacheAttribute(Type implementationType)
    {
        return implementationType
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Any(method => method.GetCustomAttribute<InventoryCacheAttribute>() is not null);
    }
}
