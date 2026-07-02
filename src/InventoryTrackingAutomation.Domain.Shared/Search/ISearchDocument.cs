using System;

namespace InventoryTrackingAutomation.Search;

/// <summary>
/// Elasticsearch'e yazilan arama dokumanlarinin ortak sozlesmesi.
/// </summary>
// islevi: Her arama dokumaninin hangi index'e ait oldugunu ve kaynak entity Id'sini compile-time bildirir.
// sistemdeki gorevi: Generic Elasticsearch repository index adini bu sozlesmeden okur; index adi parametre olarak tasinmaz.
public interface ISearchDocument
{
    /// <summary>
    /// Dokumanin ait oldugu Elasticsearch index adi; SearchIndexNames sabitlerinden verilir.
    /// </summary>
    static abstract string IndexName { get; }

    /// <summary>
    /// Kaynak entity'nin Id'si; dokumanin ES _id degeri olarak kullanilir.
    /// </summary>
    Guid Id { get; }
}
