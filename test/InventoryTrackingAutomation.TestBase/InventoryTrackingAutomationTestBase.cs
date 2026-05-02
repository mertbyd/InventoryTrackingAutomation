using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Volo.Abp.Modularity;
using Volo.Abp.Uow;
using Volo.Abp.Testing;

namespace InventoryTrackingAutomation;

/* All test classes are derived from this class, directly or indirectly. */
public abstract class InventoryTrackingAutomationTestBase<TStartupModule> : AbpIntegratedTest<TStartupModule>
    where TStartupModule : IAbpModule
{
    protected override void SetAbpApplicationCreationOptions(AbpApplicationCreationOptions options)
    {
        options.UseAutofac();
    }

    protected virtual Task WithUnitOfWorkAsync(Func<Task> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task WithUnitOfWorkAsync(AbpUnitOfWorkOptions options, Func<Task> action)
    {
        var uowManager = GetRequiredService<IUnitOfWorkManager>();

        using (var uow = uowManager.Begin(options))
        {
            await action();

            await uow.CompleteAsync();
        }
    }

    protected virtual Task<TResult> WithUnitOfWorkAsync<TResult>(Func<Task<TResult>> func)
    {
        return WithUnitOfWorkAsync(new AbpUnitOfWorkOptions(), func);
    }

    protected virtual async Task<TResult> WithUnitOfWorkAsync<TResult>(AbpUnitOfWorkOptions options, Func<Task<TResult>> func)
    {
        var uowManager = GetRequiredService<IUnitOfWorkManager>();

        using (var uow = uowManager.Begin(options))
        {
            var result = await func();
            await uow.CompleteAsync();
            return result;
        }
    }
}
