using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// VehicleTaskLine domain manager'i - arac-gorev urun tahsis ve iade uzlasma kurallarini yonetir.
/// </summary>
public class VehicleTaskLineManager : BaseManager<VehicleTaskLine>
{
    protected override string AlreadyExistsErrorCode => VehicleTaskLineExceptionCodes.AlreadyExists;

    private IVehicleTaskRepository _vehicleTaskRepository => LazyGetRequiredService<IVehicleTaskRepository>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private TaskLineManager _taskLineManager => LazyGetRequiredService<TaskLineManager>();

    public VehicleTaskLineManager(IVehicleTaskLineRepository repository, IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Arac-gorev kalemi olusturmak icin kullanilir. Urun bilgisi TaskLine iliskisinden cozulur.
    /// </summary>
    public async Task<CreateVehicleTaskLineModel> CreateAsync(Guid vehicleTaskId, CreateVehicleTaskLineModel model)
    {
        await EnsureVehicleTaskExistsAsync(vehicleTaskId);

        var taskLine = await _taskLineRepository.FindAsync(model.TaskLineId);
        if (taskLine == null)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        if (taskLine.TaskId != (await _vehicleTaskRepository.GetAsync(vehicleTaskId)).TaskId)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        await EnsureUniqueAsync(x => x.VehicleTaskId == vehicleTaskId && x.TaskLineId == model.TaskLineId);
        await _taskLineManager.EnsureAllocationFitsAsync(taskLine, model.AllocatedQuantity);

        return model;
    }

    /// Toplu arac-gorev kalemi olusturma kurallarini minimum DB sorgusuyla uygular.
    public async Task<List<CreateVehicleTaskLineModel>> CreateManyAsync(List<CreateVehicleTaskLineModel> models)
    {
        if (models.Count == 0)
        {
            return models;
        }

        var vehicleTaskIds = models.Select(x => x.VehicleTaskId).Distinct().ToList();
        var taskLineIds = models.Select(x => x.TaskLineId).Distinct().ToList();
        var vehicleTasks = await _vehicleTaskRepository.GetListAsync(x => vehicleTaskIds.Contains(x.Id));
        var vehicleTaskById = vehicleTasks.ToDictionary(x => x.Id);
        var missingVehicleTaskId = vehicleTaskIds.FirstOrDefault(id => !vehicleTaskById.ContainsKey(id));
        if (missingVehicleTaskId != Guid.Empty || vehicleTaskById.Count != vehicleTaskIds.Count)
        {
            throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
        }

        var taskLines = await _taskLineRepository.GetListAsync(x => taskLineIds.Contains(x.Id));
        var taskLineById = taskLines.ToDictionary(x => x.Id);
        var missingTaskLineId = taskLineIds.FirstOrDefault(id => !taskLineById.ContainsKey(id));
        if (missingTaskLineId != Guid.Empty || taskLineById.Count != taskLineIds.Count)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        var duplicateInput = models
            .GroupBy(x => new { x.VehicleTaskId, x.TaskLineId })
            .FirstOrDefault(x => x.Count() > 1);
        if (duplicateInput != null)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.AlreadyExists);
        }

        foreach (var model in models)
        {
            if (model.AllocatedQuantity <= 0)
            {
                throw new BusinessException(TaskLineExceptionCodes.InsufficientRemaining);
            }

            var vehicleTask = vehicleTaskById[model.VehicleTaskId];
            var taskLine = taskLineById[model.TaskLineId];
            if (taskLine.TaskId != vehicleTask.TaskId)
            {
                throw new BusinessException(TaskLineExceptionCodes.NotFound);
            }
        }

        var existingLines = await _vehicleTaskLineRepository.GetListAsync(x =>
            vehicleTaskIds.Contains(x.VehicleTaskId) &&
            taskLineIds.Contains(x.TaskLineId));

        if (existingLines.Any(existing => models.Any(model =>
                model.VehicleTaskId == existing.VehicleTaskId &&
                model.TaskLineId == existing.TaskLineId)))
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.AlreadyExists);
        }

        var existingAllocations = await _vehicleTaskLineRepository.GetByTaskLineIdsAsync(taskLineIds);
        var existingAllocatedByTaskLineId = existingAllocations
            .GroupBy(x => x.TaskLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.AllocatedQuantity));

        var requestedByTaskLineId = models
            .GroupBy(x => x.TaskLineId)
            .ToDictionary(x => x.Key, x => x.Sum(y => y.AllocatedQuantity));

        foreach (var requested in requestedByTaskLineId)
        {
            var taskLine = taskLineById[requested.Key];
            var existingAllocated = existingAllocatedByTaskLineId.GetValueOrDefault(requested.Key);
            if (existingAllocated + requested.Value > taskLine.Quantity)
            {
                throw new BusinessException(TaskLineExceptionCodes.InsufficientRemaining);
            }
        }

        return models;
    }



    /// <summary>
    /// Iade teslim uzlasmasi icin arac-gorev kalemini guncellemek icin kullanilir.
    /// </summary>
    public async Task<ReceiveVehicleTaskLineModel> ReceiveAsync(VehicleTaskLine existing, ReceiveVehicleTaskLineModel model)
    {
        ValidateReceiveQuantities(existing, model);
        return model;
    }

    /// <summary>
    /// Arac-gorev kaleminin tahsis miktarini guncellemek icin kullanilir.
    /// </summary>
    public async Task<UpdateVehicleTaskLineModel> UpdateAsync(VehicleTaskLine existing, UpdateVehicleTaskLineModel model)
    {
        EnsureNotReceived(existing);

        var taskLine = await _taskLineRepository.FindAsync(existing.TaskLineId);
        if (taskLine == null)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        await _taskLineManager.EnsureAllocationFitsAsync(taskLine, model.AllocatedQuantity, existing.Id);

        return model;
    }

    public async Task EnsureCanDeleteAsync(Guid lineId)
    {
        var line = await EnsureExistsAsync(lineId);
        EnsureNotReceived(line);
    }

    private static void EnsureNotReceived(VehicleTaskLine line)
    {
        if (line.ReceivedQuantity > 0 || line.DamagedQuantity > 0 || line.LostQuantity > 0 || line.ConsumedQuantity > 0)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.CannotDeleteReceived);
        }
    }

    /// <summary>
    /// Urun bilgisi ile birlikte tek bir arac-gorev kalemini getirir.
    /// </summary>
    public async Task<VehicleTaskLineWithProductModel> GetWithProductAsync(Guid id)
    {
        var entity = await EnsureExistsAsync(id);
        var taskLine = await _taskLineRepository.GetAsync(entity.TaskLineId);

        return new VehicleTaskLineWithProductModel(entity, taskLine.ProductId);
    }

    /// <summary>
    /// Urun bilgileri ile birlikte arac-goreve bagli tum kalemleri getirir.
    /// </summary>
    public async Task<List<VehicleTaskLineWithProductModel>> GetListWithProductByVehicleTaskAsync(Guid vehicleTaskId)
    {
        var entities = await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(vehicleTaskId);
        if (!entities.Any()) return new List<VehicleTaskLineWithProductModel>();

        var taskLineIds = entities.Select(x => x.TaskLineId).Distinct().ToList();
        var taskLines = await _taskLineRepository.GetListAsync(x => taskLineIds.Contains(x.Id));
        var productByTaskLineId = taskLines.ToDictionary(x => x.Id, x => x.ProductId);

        return entities.Select(e => new VehicleTaskLineWithProductModel(
            e,
            productByTaskLineId.TryGetValue(e.TaskLineId, out var pid) ? pid : Guid.Empty)
        ).ToList();
    }

    /// <summary>
    /// Arac-gorev atamasinin varligini dogrular.
    /// </summary>
    public async Task<VehicleTask> EnsureVehicleTaskExistsAsync(Guid vehicleTaskId)
    {
        var vehicleTask = await _vehicleTaskRepository.FindAsync(vehicleTaskId);
        if (vehicleTask == null)
        {
            throw new BusinessException(VehicleTaskExceptionCodes.NotFound);
        }
        return vehicleTask;
    }

    /// <summary>
    /// Satirin belirli bir arac-gorev atamasina ait oldugunu dogrular.
    /// </summary>
    public async Task<VehicleTaskLine> EnsureBelongsToVehicleTaskAsync(Guid vehicleTaskId, Guid lineId)
    {
        var existing = await EnsureExistsAsync(lineId);
        if (existing.VehicleTaskId != vehicleTaskId)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.NotFound);
        }
        return existing;
    }

    /// <summary>
    /// VehicleTaskId ve TaskLineId ile kalemi bulmak veya olusturmak icin kullanilir.
    /// </summary>
    public async Task<VehicleTaskLine> FindOrCreateAsync(Guid vehicleTaskId, Guid taskLineId, int allocatedQuantity)
    {
        var existing = await _vehicleTaskLineRepository.FindByVehicleTaskAndTaskLineAsync(vehicleTaskId, taskLineId);
        if (existing != null)
        {
            return existing;
        }

        var taskLine = await _taskLineRepository.FindAsync(taskLineId);
        if (taskLine == null)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }

        await _taskLineManager.EnsureAllocationFitsAsync(taskLine, allocatedQuantity);

        var entity = new VehicleTaskLine(GuidGenerator.Create())
        {
            VehicleTaskId = vehicleTaskId,
            TaskLineId = taskLineId,
            AllocatedQuantity = allocatedQuantity
        };
        return await Repository.InsertAsync(entity, autoSave: true);
    }

    /// <summary>
    /// Iade miktarlarinin gecerliligini dogrulamak icin kullanilir.
    /// </summary>
    private static void ValidateReceiveQuantities(VehicleTaskLine line, ReceiveVehicleTaskLineModel model)
    {
        if (model.ReceivedQuantity < 0 || model.DamagedQuantity < 0 || model.LostQuantity < 0 || model.ConsumedQuantity < 0)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.QuantityMismatch);
        }

        var total = model.ReceivedQuantity + model.DamagedQuantity + model.LostQuantity + model.ConsumedQuantity;
        if (total != line.AllocatedQuantity)
        {
            throw new BusinessException(VehicleTaskLineExceptionCodes.QuantityMismatch);
        }
    }
}


