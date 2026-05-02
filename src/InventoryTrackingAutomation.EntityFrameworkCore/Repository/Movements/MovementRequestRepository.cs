using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Models.Movements;

namespace InventoryTrackingAutomation.Repository.Movements;

/// <summary>
/// MovementRequest entity'si için EF Core repository implementasyonu.
/// </summary>
public class MovementRequestRepository : BaseRepository<MovementRequest>, IMovementRequestRepository
{
    public MovementRequestRepository(IDbContextProvider<InventoryTrackingAutomationDbContext> dbContextProvider)
        : base(dbContextProvider)
    {
    }

    /// <summary>
    /// MovementRequest kararlarini field tekrari olmadan joinli operasyon context'i ile cozer.
    /// </summary>
    public async Task<MovementRequestOperationalContextModel?> GetOperationalContextAsync(Guid movementRequestId)
    {
        // DbContext sadece repository icinde kullanilir; manager/service DB sorgusu yazmaz.
        var dbContext = await GetDbContextAsync();

        // Task tipi ve arac bilgisi tek sorguda cozulur.
        return await (
            from request in dbContext.MovementRequests
            join vehicleTask in dbContext.VehicleTasks on request.VehicleTaskId equals vehicleTask.Id
            join task in dbContext.InventoryTasks on vehicleTask.TaskId equals task.Id
            where request.Id == movementRequestId
            select new MovementRequestOperationalContextModel
            {
                MovementRequestId = request.Id,
                RequestedByWorkerId = request.RequestedByWorkerId,
                SourceWarehouseId = task.SourceWarehouseId,
                TargetWarehouseId = request.ParentMovementRequestId != null
                    ? task.ReturnWarehouseId ?? task.SourceWarehouseId
                    : task.TargetWarehouseId,
                TaskId = task.Id,
                TaskType = task.Type,
                TaskStatus = task.Status,
                ReturnWarehouseId = task.ReturnWarehouseId,
                VehicleTaskId = vehicleTask.Id,
                VehicleId = vehicleTask.VehicleId,
                ResponsibleWorkerId = vehicleTask.ResponsibleWorkerId,
                ParentMovementRequestId = request.ParentMovementRequestId,
                Status = request.Status
            }).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Task iadesinde ana hareketle iliski kurmak icin en guncel parent movement'i bulur.
    /// </summary>
    public async Task<Guid?> FindLatestMainMovementIdAsync(Guid vehicleTaskId)
    {
        // ParentMovementRequestId bos olan kayitlar ana hareket kabul edilir.
        var dbContext = await GetDbContextAsync();

        return await dbContext.MovementRequests
            .Where(x =>
                x.VehicleTaskId == vehicleTaskId &&
                x.ParentMovementRequestId == null)
            .OrderByDescending(x => x.CreationTime)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync();
    }
}
