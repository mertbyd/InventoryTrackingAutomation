using System;

namespace InventoryTrackingAutomation.Dtos.Inventory;

public class InventoryGridItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string Name { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; }
    public string WarehouseLocation { get; set; }
    public decimal Quantity { get; set; }

    public string ProductName { get; set; }
}

