using System;
using Volo.Abp.Domain.Repositories;
using InventoryTrackingAutomation.Entities.Lookups;

namespace InventoryTrackingAutomation.Interface.Lookups;

/// <summary>
/// VehicleType (Araç Tipi) entity'si için özel veritabanı işlemlerini tanımlayan repository arayüzü.
/// </summary>
public interface IVehicleTypeRepository : IBaseRepository<VehicleType>
{
}
