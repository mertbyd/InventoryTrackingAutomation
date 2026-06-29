using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// TaskLine domain manager'i - gorev kalem kurallarini yonetir.
/// </summary>
public class TaskLineManager : BaseManager<TaskLine>
{
    private IProductRepository _productRepository => LazyGetRequiredService<IProductRepository>();
    private IInventoryTaskRepository _inventoryTaskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();

    public TaskLineManager(ITaskLineRepository repository, IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// Gorev kalemi olusturmak icin kullanilir.
    public async Task<TaskLine> CreateAsync(Guid taskId, CreateTaskLineModel model)
    {
        await EnsureExistsInAsync(_inventoryTaskRepository, taskId);
        await EnsureExistsInAsync(_productRepository, model.ProductId);
        await EnsureUniqueAsync(x => x.TaskId == taskId && x.ProductId == model.ProductId);

        var entity = new TaskLine(GuidGenerator.Create())
        {
            TaskId = taskId,
            ProductId = model.ProductId,
            Quantity = model.Quantity
        };
        return entity;
    }

    /// Gorev kalemini guncellemek icin kullanilir. Miktar, VehicleTaskLine toplam tahsisinin altina dusulemez.
    public async Task<TaskLine> UpdateAsync(TaskLine existing, UpdateTaskLineModel model)
    {
        await EnsureExistsInAsync(_productRepository, model.ProductId);
        var allocatedQuantity = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(existing.Id);

        if (existing.ProductId != model.ProductId)
        {
            if (allocatedQuantity > 0)
            {
                throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.CannotChangeProductAllocated)
                    .WithData("TaskLineId", existing.Id)
                    .WithData("AllocatedQuantity", allocatedQuantity);
            }

            await EnsureUniqueAsync(x => x.TaskId == existing.TaskId && x.ProductId == model.ProductId);
        }

        if (model.Quantity < allocatedQuantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.QuantityBelowAllocated)
                .WithData("TaskLineId", existing.Id)
                .WithData("Quantity", model.Quantity)
                .WithData("AllocatedQuantity", allocatedQuantity);
        }

        existing.ProductId = model.ProductId;
        existing.Quantity = model.Quantity;
        return existing;
    }

    /// Tek bir arac satirinin hedef miktarini gorev kalemi kapasitesine gore dogrular.
    public async Task EnsureAllocationFitsAsync(TaskLine line, int vehicleLineQuantity, Guid? excludedVehicleTaskLineId = null)
    {
        if (vehicleLineQuantity <= 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.InsufficientRemaining)
                .WithData("TaskLineId", line.Id);
        }

        // AllocatedQuantity TaskLine'da tutulmaz; tek dogru kaynak VehicleTaskLine toplamidir.
        var allocatedExceptCurrent = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(
            line.Id,
            excludedVehicleTaskLineId);

        if (allocatedExceptCurrent + vehicleLineQuantity > line.Quantity)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.InsufficientRemaining)
                .WithData("TaskLineId", line.Id)
                .WithData("Requested", vehicleLineQuantity)
                .WithData("Remaining", line.Quantity - allocatedExceptCurrent);
        }
    }

    /// Gorev kalemini silmek icin kullanilir. Tahsis varsa silme engellenir.
    // islevi: Tahsis edilmemis taslak gorev kalemini fiziksel olarak kaldirir.
    // sistemdeki gorevi: VehicleTaskLine baglantisi olusan operasyonel satirlarin silinmesini engelleyerek sureci korur.
    public async Task DeleteAsync(Guid lineId)
    {
        var line = await EnsureExistsAsync(lineId);
        var allocatedQuantity = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(line.Id);
        if (allocatedQuantity > 0)
        {
            throw new BusinessException(InventoryTrackingAutomationErrorCodes.TaskLines.CannotDeleteAllocated)
                .WithData("TaskLineId", lineId)
                .WithData("AllocatedQuantity", allocatedQuantity);
        }

        await Repository.DeleteAsync(line);
    }

    /// Gorev icin tum kalemleri getirmek icin kullanilir.
    public Task<List<TaskLine>> GetByTaskIdAsync(Guid taskId)
    {
        return _taskLineRepository.GetByTaskIdAsync(taskId);
    }

    /// TaskId ve ProductId ile gorev kalemini bulmak veya olusturmak icin kullanilir.
    public async Task<TaskLine> FindOrCreateAsync(Guid taskId, Guid productId, int quantity)
    {
        var existing = await _taskLineRepository.FindByTaskAndProductAsync(taskId, productId);
        if (existing != null)
        {
            return existing;
        }

        var entity = new TaskLine(GuidGenerator.Create())
        {
            TaskId = taskId,
            ProductId = productId,
            Quantity = quantity
        };
        return await Repository.InsertAsync(entity, autoSave: true);
    }
}
