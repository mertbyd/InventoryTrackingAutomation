using System;
using Volo.Abp.Domain.Repositories;
using InventoryTrackingAutomation.Entities.Lookups;

namespace InventoryTrackingAutomation.Interface.Lookups;

/// <summary>
/// UnitType (Ölçü Birimi) entity'si için özel veritabanı işlemlerini tanımlayan repository arayüzü.
/// </summary>
public interface IUnitTypeRepository : IBaseRepository<UnitType>
{
}
