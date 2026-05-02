using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Workflows;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Managers;
using InventoryTrackingAutomation.Managers.Workflows.Approvers;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Workflows;

// Strategy-registry tabanlı onaycı çözümleyici — DI ile gelen tüm IApproverStrategy implementasyonlarını
// ResolverKey'e göre indeksler ve adımın anahtarına göre ilgili strategy'i çalıştırır.
// Yeni resolver eklemek için sadece yeni bir IApproverStrategy implementasyonu eklenir; bu sınıfa dokunulmaz.
//işlevi: DefaultWorkflowApproverResolver.cs etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class DefaultWorkflowApproverResolver : InventoryTrackingAutomationLazyService, IWorkflowApproverResolver, ITransientDependency
{
    public DefaultWorkflowApproverResolver(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IReadOnlyDictionary<string, IApproverStrategy> _strategies =>
        LazyGetRequiredService<IEnumerable<IApproverStrategy>>()
            .GroupBy(s => s.Key, StringComparer.Ordinal)
            .ToDictionary(g => g.Key, g => g.First(), StringComparer.Ordinal);

//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public Task<Guid?> ResolveApproverAsync(ApproverContext context, WorkflowStepDefinition stepDefinition)
    {
        if (string.IsNullOrEmpty(stepDefinition.ResolverKey))
        {
            return Task.FromResult<Guid?>(null);
        }

        return _strategies.TryGetValue(stepDefinition.ResolverKey, out var strategy)
            ? strategy.ResolveAsync(context)
            : Task.FromResult<Guid?>(null);
    }
}
