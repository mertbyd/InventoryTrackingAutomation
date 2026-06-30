using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Tasks;

/// <summary>
/// TaskLine domain manager'i - gorev malzemeleri is kurallari.
/// </summary>
public class TaskLineManager : BaseManager<TaskLine>
{
    protected override string AlreadyExistsErrorCode => TaskLineExceptionCodes.AlreadyExists;

    private IProductRepository _productRepository => LazyGetRequiredService<IProductRepository>();
    private IInventoryTaskRepository _inventoryTaskRepository => LazyGetRequiredService<IInventoryTaskRepository>();
    private ITaskLineRepository _taskLineRepository => LazyGetRequiredService<ITaskLineRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();

    public TaskLineManager(ITaskLineRepository repository, IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni görev satırı oluşturmak için kullanılır.
    /// </summary>
    public async Task<CreateTaskLineModel> CreateAsync(Guid taskId, CreateTaskLineModel model)
    {
        await EnsureExistsInAsync(_inventoryTaskRepository, taskId);
        await EnsureExistsInAsync(_productRepository, model.ProductId);
        ValidateQuantity(model.Quantity);
        await EnsureUniqueAsync(x => x.TaskId == taskId && x.ProductId == model.ProductId);

        return model;
    }

    /// Toplu gorev kalemi olusturma kurallarini minimum DB sorgusuyla uygular.
    public async Task<List<CreateTaskLineModel>> CreateManyAsync(List<CreateTaskLineModel> models)
    {
        if (models.Count == 0)
        {
            return models;
        }

        foreach (var model in models)
        {
            ValidateQuantity(model.Quantity);
        }

        var taskIds = models.Select(x => x.TaskId).Distinct().ToList();
        await EnsureAllExistInAsync(_inventoryTaskRepository, taskIds);

        var productIds = models.Select(x => x.ProductId).Distinct().ToList();
        await EnsureAllExistInAsync(_productRepository, productIds);

        var duplicateInput = models
            .GroupBy(x => new { x.TaskId, x.ProductId })
            .FirstOrDefault(x => x.Count() > 1);
        if (duplicateInput != null)
        {
            throw new BusinessException(TaskLineExceptionCodes.AlreadyExists);
        }

        var existingLines = await _taskLineRepository.GetListAsync(x =>
            taskIds.Contains(x.TaskId) &&
            productIds.Contains(x.ProductId));

        if (existingLines.Any(existing => models.Any(model =>
                model.TaskId == existing.TaskId &&
                model.ProductId == existing.ProductId)))
        {
            throw new BusinessException(TaskLineExceptionCodes.AlreadyExists);
        }

        return models;
    }

    /// <summary>
    /// Mevcut görev satırını güncellemek için kullanılır.
    /// </summary>
    public async Task<UpdateTaskLineModel> UpdateAsync(TaskLine existing, UpdateTaskLineModel model)
    {
        await EnsureExistsInAsync(_productRepository, model.ProductId);
        ValidateQuantity(model.Quantity);
        var allocatedQuantity = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(existing.Id);

        if (existing.ProductId != model.ProductId)
        {
            if (allocatedQuantity > 0)
            {
                throw new BusinessException(TaskLineExceptionCodes.CannotChangeProductAllocated);
            }
            await EnsureUniqueAsync(x => x.TaskId == existing.TaskId && x.ProductId == model.ProductId);
        }

        if (model.Quantity < allocatedQuantity)
        {
            throw new BusinessException(TaskLineExceptionCodes.QuantityBelowAllocated);
        }
        return model;
    }

    /// <summary>
    /// İstenen miktarı doğrulamak için kullanılır.
    /// </summary>
    private static void ValidateQuantity(decimal quantity)
    {
        if (quantity <= 0)
        {
            throw new BusinessException(TaskLineExceptionCodes.NotFound);
        }
    }
    /// Tek bir arac satirinin hedef miktarini gorev kalemi kapasitesine gore dogrular.
    public async Task EnsureAllocationFitsAsync(TaskLine line, int vehicleLineQuantity, Guid? excludedVehicleTaskLineId = null)
    {
        if (vehicleLineQuantity <= 0)
        {
            throw new BusinessException(TaskLineExceptionCodes.InsufficientRemaining);
        }
        // AllocatedQuantity TaskLine'da tutulmaz; tek dogru kaynak VehicleTaskLine toplamidir.
        var allocatedExceptCurrent = await _vehicleTaskLineRepository.GetAllocatedQuantityByTaskLineIdAsync(
            line.Id,
            excludedVehicleTaskLineId);
        if (allocatedExceptCurrent + vehicleLineQuantity > line.Quantity)
        {
            throw new BusinessException(TaskLineExceptionCodes.InsufficientRemaining);
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
            throw new BusinessException(TaskLineExceptionCodes.CannotDeleteAllocated);
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

