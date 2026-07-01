using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Common;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Application.DisplayReferences;

/// <summary>
/// IHasDisplayReferences DTO'larinin References sozlugunu, convention metadata'sina gore entity display
/// degerleriyle (Code/Name) BATCH doldurur. Ayni entity tipine dusen Id'ler tek sorguda cekilir.
/// </summary>
// Bilincli istisna: Sifir-config enrichment icin reflection ve runtime generic repository dispatch kullanilir;
// bu, [[DisplayReferenceConvention]] ile ayni kullanici onayli reflection istisnasi kapsamindadir.
public sealed class DisplayReferenceEnricher : ITransientDependency
{
    // entityType basina generic yukleme metodunu reflection ile cagirmak icin bir kez alinir.
    private static readonly MethodInfo LoadEntitiesGenericMethod = typeof(DisplayReferenceEnricher)
        .GetMethod(nameof(LoadEntitiesAsync), BindingFlags.Instance | BindingFlags.NonPublic)!;

    private readonly DisplayReferenceConvention _convention;
    private readonly IServiceProvider _serviceProvider;

    public DisplayReferenceEnricher(DisplayReferenceConvention convention, IServiceProvider serviceProvider)
    {
        _convention = convention;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Tek bir DTO'nun display referanslarini doldurur.
    /// </summary>
    public Task EnrichAsync(IHasDisplayReferences target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return EnrichAsync(new[] { target });
    }

    /// <summary>
    /// Birden fazla DTO'yu tek batch akisinda doldurur; ayni entity tipi tek sorguyla cekilir.
    /// </summary>
    public async Task EnrichAsync(IReadOnlyCollection<IHasDisplayReferences> targets)
    {
        if (targets is null || targets.Count == 0)
        {
            return;
        }

        // DTO + FK metadata + okunan Id ucluleri; sonraki adimlar bu duz liste uzerinden calisir.
        var bindings = CollectBindings(targets);
        if (bindings.Count == 0)
        {
            return;
        }

        // Her entity tipi icin Id -> entity sozlugu tek batch sorguyla yuklenir.
        var entitiesByType = await LoadEntitiesByTypeAsync(bindings);

        // Doldurma tamamen bellek ici; dongude DB erisimi yoktur.
        ApplyLabels(bindings, entitiesByType);
    }

    /// <summary>
    /// Hedef DTO'lardan convention FK'lerini ve gecerli Id degerlerini toplar.
    /// </summary>
    private List<ReferenceBinding> CollectBindings(IReadOnlyCollection<IHasDisplayReferences> targets)
    {
        var bindings = new List<ReferenceBinding>();

        foreach (var target in targets)
        {
            foreach (var metadata in _convention.GetReferences(target.GetType()))
            {
                if (ReadForeignKeyId(metadata.ForeignKeyProperty, target) is { } id && id != Guid.Empty)
                {
                    bindings.Add(new ReferenceBinding(target, metadata, id));
                }
            }
        }

        return bindings;
    }

    /// <summary>
    /// Entity tipine gore gruplanan distinct Id'leri tip basina tek sorguyla yukler.
    /// </summary>
    private async Task<Dictionary<Type, IReadOnlyDictionary<Guid, object>>> LoadEntitiesByTypeAsync(
        IReadOnlyCollection<ReferenceBinding> bindings)
    {
        var result = new Dictionary<Type, IReadOnlyDictionary<Guid, object>>();

        var idsByType = bindings
            .GroupBy(binding => binding.Metadata.EntityType)
            .ToDictionary(group => group.Key, group => group.Select(binding => binding.Id).Distinct().ToList());

        // Dongu entity tipi basinadir (liste satiri basina degil); her tip icin tek batch sorgu -> N+1 yok.
        foreach (var (entityType, ids) in idsByType)
        {
            var lookup = await InvokeLoadAsync(entityType, ids);
            if (lookup is not null)
            {
                result[entityType] = lookup;
            }
        }

        return result;
    }

    /// <summary>
    /// Runtime entity tipi icin generic yukleme metodunu reflection ile cagirir.
    /// </summary>
    private Task<IReadOnlyDictionary<Guid, object>?> InvokeLoadAsync(Type entityType, List<Guid> ids)
    {
        var method = LoadEntitiesGenericMethod.MakeGenericMethod(entityType);
        return (Task<IReadOnlyDictionary<Guid, object>?>)method.Invoke(this, new object[] { ids })!;
    }

    /// <summary>
    /// Generic read-only repository ile Id listesini tek sorguda ceker; repository kayitli degilse atlar.
    /// </summary>
    private async Task<IReadOnlyDictionary<Guid, object>?> LoadEntitiesAsync<TEntity>(List<Guid> ids)
        where TEntity : class, IEntity<Guid>
    {
        // DbSet'i olmayan (repository kaydi bulunmayan) entity tipleri sessizce atlanir.
        var repository = _serviceProvider.GetService<IReadOnlyRepository<TEntity, Guid>>();
        if (repository is null)
        {
            return null;
        }

        var entities = await repository.GetListAsync(entity => ids.Contains(entity.Id));
        return entities.ToDictionary(entity => entity.Id, entity => (object)entity);
    }

    /// <summary>
    /// Yuklenen entity'lerden DisplayLabelDto uretip her DTO'nun References sozlugune yazar.
    /// </summary>
    private static void ApplyLabels(
        IReadOnlyCollection<ReferenceBinding> bindings,
        IReadOnlyDictionary<Type, IReadOnlyDictionary<Guid, object>> entitiesByType)
    {
        foreach (var binding in bindings)
        {
            if (!entitiesByType.TryGetValue(binding.Metadata.EntityType, out var lookup)
                || !lookup.TryGetValue(binding.Id, out var entity))
            {
                continue;
            }

            binding.Target.References[binding.Metadata.PropertyName] = new DisplayLabelDto
            {
                Id = binding.Id,
                Code = binding.Metadata.CodeProperty?.GetValue(entity) as string,
                Name = binding.Metadata.NameProperty?.GetValue(entity) as string
            };
        }
    }

    /// <summary>
    /// FK property'sinden Guid degerini okur; deger Guid degilse (ornekte null Guid?) null doner.
    /// </summary>
    private static Guid? ReadForeignKeyId(PropertyInfo property, object target)
    {
        return property.GetValue(target) is Guid id ? id : null;
    }

    private sealed record ReferenceBinding(IHasDisplayReferences Target, ReferenceFieldMetadata Metadata, Guid Id);
}
