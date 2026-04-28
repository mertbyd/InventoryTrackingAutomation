using AutoMapper;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Models.Stock;
using InventoryTrackingAutomation.Models.Tasks;

namespace InventoryTrackingAutomation.Application.ObjectMapping.Stock;

/// <summary>
/// PITON stok/gorev okuma modellerini response DTO'lara map eder.
/// </summary>
public class InventoryQueryMappingProfile : Profile
{
    public InventoryQueryMappingProfile()
    {
        CreateMap<ProductStockLocationSummaryModel, ProductStockLocationSummaryDto>();
        CreateMap<ProductStockSummaryModel, ProductStockSummaryDto>();
        CreateMap<VehicleInventoryModel, VehicleInventoryDto>();
        CreateMap<TaskVehicleModel, TaskVehicleDto>();
        CreateMap<TaskInventoryModel, TaskInventoryDto>();
    }
}
