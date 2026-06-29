using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;

namespace InventoryTrackingAutomation.Repository.Tasks;

/// <summary>
/// VehicleTaskLine entity'si icin EF Core repository implementasyonu.
/// </summary>
public class VehicleTaskLineRepository : BaseRepository<VehicleTaskLine>, IVehicleTaskLineRepository
{
    public VehicleTaskLineRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// Arac gorevine ait allocation satirlarini getirir.
    /// </summary>
    // islevi: VehicleTaskId uzerinden arac-gorev kalemlerini listeler.
    // sistemdeki gorevi: VehicleTaskLine artik soft-delete tasimadigi icin sorguda yalnizca is filtresini tutar.
    public async Task<List<VehicleTaskLine>> GetByVehicleTaskIdAsync(Guid vehicleTaskId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x => x.VehicleTaskId == vehicleTaskId)
            .ToListAsync();
    }

    /// <summary>
    /// Verilen task line kayitlarina bagli arac allocation satirlarini getirir.
    /// </summary>
    // islevi: Birden fazla TaskLineId icin arac kalemi dagilimlarini listeler.
    // sistemdeki gorevi: Sorguda yalnizca ilgili TaskLine filtresini tutar.
    public async Task<List<VehicleTaskLine>> GetByTaskLineIdsAsync(IReadOnlyCollection<Guid> taskLineIds)
    {
        if (taskLineIds.Count == 0)
        {
            return new List<VehicleTaskLine>();
        }

        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x => taskLineIds.Contains(x.TaskLineId))
            .ToListAsync();
    }

    /// <summary>
    /// Arac gorevi ve gorev kalemi kombinasyonuna ait allocation satirini bulur.
    /// </summary>
    // islevi: Ayni arac gorevine ayni task line tekrar baglanmis mi kontrol eder.
    // sistemdeki gorevi: Benzersizlik kontrolu icin sadece arac-gorev ve gorev-kalemi filtresini tasir.
    public async Task<VehicleTaskLine?> FindByVehicleTaskAndTaskLineAsync(Guid vehicleTaskId, Guid taskLineId)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .FirstOrDefaultAsync(x => x.VehicleTaskId == vehicleTaskId && x.TaskLineId == taskLineId);
    }

    /// <summary>
    /// Bir task line icin araclara ayrilmis toplam miktari hesaplar.
    /// </summary>
    // islevi: Allocation limit kontrolleri icin mevcut ayrilmis miktari toplar.
    // sistemdeki gorevi: Fiziksel olarak mevcut allocation satirlarinin toplam tahsis miktarini verir.
    public async Task<int> GetAllocatedQuantityByTaskLineIdAsync(Guid taskLineId, Guid? excludedVehicleTaskLineId = null)
    {
        var dbContext = await GetDbContextAsync();
        return await dbContext.VehicleTaskLines
            .Where(x =>
                x.TaskLineId == taskLineId &&
                (!excludedVehicleTaskLineId.HasValue || x.Id != excludedVehicleTaskLineId.Value))
            .SumAsync(x => x.AllocatedQuantity);
    }
}
