using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Movements;

/// <summary>
/// Hareket talebi satırı response DTO'su — GetAll ve GetById operasyonlarında döner.
/// </summary>
//işlevi: MovementRequestLine verisinin transferi sırasında taşınacak olan yapıyı tanımlar.
//sistemdeki görevi: Katmanlar arası veri alışverişini standartlaştırır.
public class MovementRequestLineDto : FullAuditedEntityDto<Guid>
{
    public Guid MovementRequestId { get; set; }     // Bağlı talep Id.
    public Guid ProductId { get; set; }             // Ürün Id.
    public int Quantity { get; set; }               // Talep edilen miktar. Örnek: 20
    public int ReceivedQuantity { get; set; }       // Iade tesliminde depoya saglam giren miktar.
    public int DamagedQuantity { get; set; }        // Iade tesliminde hasarli/kirik olarak ayrilan miktar.
    public int LostQuantity { get; set; }           // Iade tesliminde kayip olarak isaretlenen miktar.
    public int ConsumedQuantity { get; set; }       // Gorevde tuketildigi bildirilen miktar.
    public string? ReceiveNote { get; set; }        // Satir bazli teslim alma/kontrol notu.
}
