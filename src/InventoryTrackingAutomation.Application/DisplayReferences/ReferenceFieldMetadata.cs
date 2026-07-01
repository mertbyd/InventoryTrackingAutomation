using System;
using System.Reflection;

namespace InventoryTrackingAutomation.Application.DisplayReferences;

/// <summary>
/// Bir DTO FK alaninin convention ile cozulmus display metadata'sini tasir:
/// FK property'si, hedef entity ve DisplayLabelDto'nun Code/Name alanlarina map'lenecek entity property'leri.
/// </summary>
public sealed record ReferenceFieldMetadata(
    PropertyInfo ForeignKeyProperty,
    Type EntityType,
    PropertyInfo? CodeProperty,
    PropertyInfo? NameProperty)
{
    /// <summary>
    /// FK property adi; DTO'nun References sozlugunde key olarak kullanilir.
    /// </summary>
    public string PropertyName => ForeignKeyProperty.Name;
}
