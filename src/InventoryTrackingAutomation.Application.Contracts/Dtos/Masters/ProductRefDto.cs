using System;
using Volo.Abp.Application.Dtos;

namespace InventoryTrackingAutomation.Dtos.Masters;

/// <summary>
/// Nested navigation'larda ürünü temsil eden küçük referans DTO'su (Id + Code + Name).
/// </summary>
public class ProductRefDto : EntityDto<Guid>
{
    public string Code { get; set; }
    public string Name { get; set; }
}
