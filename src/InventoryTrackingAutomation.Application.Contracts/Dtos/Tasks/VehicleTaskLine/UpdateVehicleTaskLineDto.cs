using System;

namespace InventoryTrackingAutomation.Dtos.Tasks;

/// <summary>
/// Araç-görev kalemi tahsis miktarı güncelleme verisini tanımlar.
/// </summary>
//işlevi: VehicleTaskLine tahsis miktarının istemci ile uygulama katmanı arasında taşınmasını sağlar.
//sistemdeki görevi: Araç üzerindeki görev kalemi tahsis miktarını yeni VehicleTaskLine modeliyle günceller.
public class UpdateVehicleTaskLineDto
{
    /// <summary>
    /// Bu araca tahsis edilecek yeni miktar.
    /// </summary>
    public int AllocatedQuantity { get; set; } // Yeni araç tahsis miktarı.
}
