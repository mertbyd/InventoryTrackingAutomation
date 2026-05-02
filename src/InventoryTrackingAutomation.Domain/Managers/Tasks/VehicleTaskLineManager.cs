using AutoMapper;
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
    public async Task<VehicleTaskLine> CreateAsync(Guid vehicleTaskId, CreateVehicleTaskLineModel model)
    {
        await EnsureVehicleTaskExistsAsync(vehicleTaskId);

        var taskLine = await _taskLineRepository.FindAsync(model.TaskLineId);
        if (taskLine == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.NotFound)
                .WithData("TaskLineId", model.TaskLineId);
        }

        if (taskLine.TaskId != (await _vehicleTaskRepository.GetAsync(vehicleTaskId)).TaskId)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.NotFound)
                .WithData("VehicleTaskId", vehicleTaskId)
                .WithData("TaskLineId", model.TaskLineId);
        }

        await EnsureUniqueAsync(x => x.VehicleTaskId == vehicleTaskId && x.TaskLineId == model.TaskLineId);
        await _taskLineManager.EnsureAllocationFitsAsync(taskLine, model.AllocatedQuantity);

        var entity = new VehicleTaskLine(GuidGenerator.Create())
        {
            VehicleTaskId = vehicleTaskId,
            TaskLineId = taskLine.Id,
            AllocatedQuantity = model.AllocatedQuantity
        };
        return entity;
    }

    /// <summary>
    /// Birden fazla arac-gorev kalemi toplu olusturmak icin kullanilir.
    /// </summary>
    public async Task<List<VehicleTaskLine>> CreateManyAsync(Guid vehicleTaskId, List<CreateVehicleTaskLineModel> models)
    {
        var result = new List<VehicleTaskLine>();
        foreach (var model in models)
        {
            result.Add(await CreateAsync(vehicleTaskId, model));
        }

        return result;
    }

    /// <summary>
    /// Iade teslim uzlasmasi icin arac-gorev kalemini guncellemek icin kullanilir.
    /// </summary>
    public async Task<VehicleTaskLine> ReceiveAsync(VehicleTaskLine existing, ReceiveVehicleTaskLineModel model)
    {
        ValidateReceiveQuantities(existing, model);

        existing.ReceivedQuantity = model.ReceivedQuantity;
        existing.DamagedQuantity = model.DamagedQuantity;
        existing.LostQuantity = model.LostQuantity;
        existing.ConsumedQuantity = model.ConsumedQuantity;
        existing.ReceiveNote = model.ReceiveNote;
        return existing;
    }

    /// <summary>
    /// Arac-gorev kaleminin tahsis miktarini guncellemek icin kullanilir.
    /// </summary>
    public async Task<VehicleTaskLine> UpdateAsync(VehicleTaskLine existing, UpdateVehicleTaskLineModel model)
    {
        if (existing.ReceivedQuantity > 0 ||
            existing.DamagedQuantity > 0 ||
            existing.LostQuantity > 0 ||
            existing.ConsumedQuantity > 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTaskLines.CannotDeleteReceived)
                .WithData("VehicleTaskLineId", existing.Id);
        }

        var taskLine = await _taskLineRepository.FindAsync(existing.TaskLineId);
        if (taskLine == null)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.NotFound)
                .WithData("TaskLineId", existing.TaskLineId);
        }

        await _taskLineManager.EnsureAllocationFitsAsync(taskLine, model.AllocatedQuantity, existing.Id);

        existing.AllocatedQuantity = model.AllocatedQuantity;
        return existing;
    }

    /// <summary>
    /// Arac-gorev kalemini silmek icin kullanilir. Teslim alinmis satir silinemez.
    /// </summary>
    public async Task DeleteAsync(Guid lineId)
    {
        var line = await EnsureExistsAsync(lineId);
        if (line.ReceivedQuantity > 0 || line.DamagedQuantity > 0 || line.LostQuantity > 0 || line.ConsumedQuantity > 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTaskLines.CannotDeleteReceived)
                .WithData("VehicleTaskLineId", lineId);
        }

        await Repository.DeleteAsync(line);
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
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTasks.NotFound)
                .WithData("VehicleTaskId", vehicleTaskId);
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
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTaskLines.NotFound)
                .WithData("VehicleTaskId", vehicleTaskId)
                .WithData("VehicleTaskLineId", lineId);
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
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.NotFound)
                .WithData("TaskLineId", taskLineId);
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
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTaskLines.QuantityMismatch)
                .WithData("VehicleTaskLineId", line.Id);
        }

        var total = model.ReceivedQuantity + model.DamagedQuantity + model.LostQuantity + model.ConsumedQuantity;
        if (total != line.AllocatedQuantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.VehicleTaskLines.QuantityMismatch)
                .WithData("VehicleTaskLineId", line.Id)
                .WithData("Expected", line.AllocatedQuantity)
                .WithData("Actual", total);
        }
    }
}
