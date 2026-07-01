using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using static InventoryTrackingAutomation.Constants.DisplayReferences.DisplayReferenceConventionConstants;

namespace InventoryTrackingAutomation.Application.DisplayReferences;

/// <summary>
/// DTO'lardaki FK alanlarini (...Id) hedef entity ve ekran alanlariyla convention uzerinden eslestiren
/// sifir-config motordur. Bir DTO tipi icin "hangi FK -> hangi entity -> hangi Code/Name alani" sorusunu cozer.
/// </summary>
// Bilincli istisna: Bu sifir-config display enrichment altyapisi, kullanici onayi ile reflection ve isim
// convention'i kullanir; projenin genel "reflection/string-based dynamic kullanma" kuralinin disindadir.
// Gerekce: yeni entity/DTO/FK eklendiginde elle kayit gerektirmeyen sifir-bakim. Reflection bu sinifa
// hapsedilmistir; DB, DI veya repository erisimi yoktur ve sonuclar tip basina cache'lenir.
public sealed class DisplayReferenceConvention : ISingletonDependency
{
    // DTO tipi -> cozulmus referans listesi; ayni DTO icin reflection tekrar calismaz.
    private readonly ConcurrentDictionary<Type, IReadOnlyList<ReferenceFieldMetadata>> _cache = new();

    // Domain entity katalogu (Type + ad) uygulama omru boyunca bir kez taranir.
    private readonly Lazy<IReadOnlyList<(Type Type, string Name)>> _entityCatalog;

    public DisplayReferenceConvention()
    {
        _entityCatalog = new Lazy<IReadOnlyList<(Type Type, string Name)>>(
            () => ScanEntities(typeof(InventoryTrackingAutomationDomainModule).Assembly));
    }

    /// <summary>
    /// DTO tipindeki her FK icin hedef entity ve display alanlarini cozer; cozulemeyen FK sessizce atlanir.
    /// </summary>
    public IReadOnlyList<ReferenceFieldMetadata> GetReferences(Type dtoType)
    {
        ArgumentNullException.ThrowIfNull(dtoType);
        return _cache.GetOrAdd(dtoType, BuildReferences);
    }

    /// <summary>
    /// Generic DTO tipi icin cozulmus referans listesini dondurur.
    /// </summary>
    public IReadOnlyList<ReferenceFieldMetadata> GetReferences<TDto>() => GetReferences(typeof(TDto));

    /// <summary>
    /// DTO FK'lerini referansa donusturur; entity veya display bulunamayan FK'ler OfType ile elenir.
    /// </summary>
    private IReadOnlyList<ReferenceFieldMetadata> BuildReferences(Type dtoType)
    {
        return GetForeignKeyProperties(dtoType)
            .Select(CreateReference)
            .OfType<ReferenceFieldMetadata>()
            .ToList();
    }

    /// <summary>
    /// Tek bir FK property'sini entity ve display metadata ile eslestirir; cozulemezse null dondurur.
    /// </summary>
    private ReferenceFieldMetadata? CreateReference(PropertyInfo property)
    {
        // "...Id" ekini atarak eslesme govdesini uret: VehicleTaskId -> VehicleTask.
        var entityType = ResolveEntity(property.Name[..^IdSuffix.Length]);
        var display = entityType is null ? null : SelectDisplay(entityType);

        return display is null
            ? null
            : new ReferenceFieldMetadata(property, entityType!, display.Code, display.Name);
    }

    /// <summary>
    /// DTO uzerindeki Guid/Guid? tipli, "...Id" ile biten (yalin "Id" haric) FK property'lerini getirir.
    /// </summary>
    private static IEnumerable<PropertyInfo> GetForeignKeyProperties(Type dtoType)
    {
        return GetReadableProperties(dtoType)
            .Where(property => IsGuid(property.PropertyType)
                               && property.Name.Length > IdSuffix.Length
                               && property.Name.EndsWith(IdSuffix, StringComparison.Ordinal));
    }

    /// <summary>
    /// FK govdesini entity kataloguna eslestirir: once tam eslesme, sonra en uzun suffix eslesmesi.
    /// </summary>
    private Type? ResolveEntity(string stem)
    {
        var catalog = _entityCatalog.Value;

        // Tam eslesme belirleyicidir: VehicleTaskId -> VehicleTask, WorkerTypeId -> WorkerType.
        var exact = catalog.FirstOrDefault(entity => string.Equals(entity.Name, stem, StringComparison.Ordinal));
        if (exact.Type is not null)
        {
            return exact.Type;
        }

        // Suffix eslesmesi iki yonludur: ManagerWorker -> Worker ve Category -> ProductCategory.
        return catalog
            .Where(entity => stem.EndsWith(entity.Name, StringComparison.Ordinal)
                             || entity.Name.EndsWith(stem, StringComparison.Ordinal))
            .OrderByDescending(entity => entity.Name.Length)
            .ThenBy(entity => entity.Name, StringComparer.Ordinal)
            .Select(entity => entity.Type)
            .FirstOrDefault();
    }

    /// <summary>
    /// Entity string alanlarindan display karsiligini oncelik sirasiyla secer: Name(+Code) > Title > Code > "...Number".
    /// Code ve Number alanlari DisplayLabelDto.Code'a; Name ve Title alanlari DisplayLabelDto.Name'e gider.
    /// </summary>
    private static DisplayInfo? SelectDisplay(Type entityType)
    {
        var strings = GetReadableProperties(entityType)
            .Where(property => property.PropertyType == typeof(string))
            .ToList();

        var code = strings.FirstOrDefault(property => property.Name == CodeField);
        var name = strings.FirstOrDefault(property => property.Name == NameField);
        var title = strings.FirstOrDefault(property => property.Name == TitleField);
        var number = strings.FirstOrDefault(property => property.Name.EndsWith(NumberSuffix, StringComparison.Ordinal));

        // Oncelik sirasi aday listesiyle ifade edilir; ilk uygun aday secilir.
        var candidates = new[]
        {
            name is not null ? new DisplayInfo(code, name) : null,
            title is not null ? new DisplayInfo(null, title) : null,
            code is not null ? new DisplayInfo(code, null) : null,
            number is not null ? new DisplayInfo(number, null) : null
        };

        return candidates.FirstOrDefault(candidate => candidate is not null);
    }

    /// <summary>
    /// Domain assembly'sindeki somut IEntity&lt;Guid&gt; tiplerini deterministik sirayla kataloglar.
    /// </summary>
    private static IReadOnlyList<(Type Type, string Name)> ScanEntities(Assembly domainAssembly)
    {
        return GetLoadableTypes(domainAssembly)
            .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false }
                           && typeof(IEntity<Guid>).IsAssignableFrom(type))
            .Select(type => (Type: type, Name: type.Name))
            .OrderBy(entity => entity.Name, StringComparer.Ordinal)
            .ThenBy(entity => entity.Type.FullName ?? entity.Name, StringComparer.Ordinal)
            .ToList();
    }

    /// <summary>
    /// Tipin public, okunabilir, indexer olmayan property'lerini stabil (metadata token) sirada getirir.
    /// </summary>
    private static IEnumerable<PropertyInfo> GetReadableProperties(Type type)
    {
        return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is { IsPublic: true } && property.GetIndexParameters().Length == 0)
            .OrderBy(GetStableOrder);
    }

    /// <summary>
    /// Reflection property sirasini tekrar edilebilir tutmak icin metadata token degerini kullanir.
    /// </summary>
    private static int GetStableOrder(PropertyInfo property)
    {
        try
        {
            return property.MetadataToken;
        }
        catch (InvalidOperationException)
        {
            return int.MaxValue;
        }
    }

    /// <summary>
    /// Tipin Guid veya Guid? olup olmadigini kontrol eder.
    /// </summary>
    private static bool IsGuid(Type type) => (Nullable.GetUnderlyingType(type) ?? type) == typeof(Guid);

    /// <summary>
    /// Assembly tiplerini guvenli okur; kismi yukleme hatasinda okunabilen tiplerle devam eder.
    /// </summary>
    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.Where(type => type is not null)!;
        }
    }

    /// <summary>
    /// Bir entity'nin secilmis display alanlarini (DisplayLabelDto.Code / .Name karsiliklari) tasir.
    /// </summary>
    private sealed record DisplayInfo(PropertyInfo? Code, PropertyInfo? Name);
}
