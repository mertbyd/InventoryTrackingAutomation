using InventoryTrackingAutomation.Application.Mappers.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using Volo.Abp;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

/// <summary>
/// Araç-görev kalemi uygulama servisi.
/// </summary>
public class VehicleTaskLineAppService : InventoryTrackingAutomationAppService, IVehicleTaskLineAppService
{
    public VehicleTaskLineAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTaskLineRepository _repository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private VehicleTaskLineManager _manager => LazyGetRequiredService<VehicleTaskLineManager>();
    private IValidator<CreateVehicleTaskLineDto> _createValidator => LazyGetRequiredService<IValidator<CreateVehicleTaskLineDto>>();
    private IValidator<UpdateVehicleTaskLineDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateVehicleTaskLineDto>>();
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    private static readonly VehicleTaskLineMapper _mapper = new VehicleTaskLineMapper();

    /// <summary>
    /// Araç-görev kalemini getirir.
    /// </summary>
    public async Task<VehicleTaskLineDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id, includeDetails: true);
        return _mapper.MapToDto(entity);
    }

    /// <summary>
    /// Bir araç-görev atamasına bağlı tüm kalemleri getirir.
    /// </summary>
    public async Task<List<VehicleTaskLineDto>> GetByVehicleTaskAsync(Guid vehicleTaskId)
    {
        await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);
        var entities = await _repository.GetListAsync(x => x.VehicleTaskId == vehicleTaskId, includeDetails: true);
        return entities.Select(_mapper.MapToDto).ToList();
    }

    /// <summary>
    /// Araç-görev atamasına ürün tahsisi ekler.
    /// </summary>
    [UnitOfWork]
    public async Task<VehicleTaskLineDto> CreateForVehicleTaskAsync(Guid vehicleTaskId, CreateVehicleTaskLineDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var vehicleTask = await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);

        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(vehicleTaskId, model);
        var entity = new VehicleTaskLine(GuidGenerator.Create());
        entity.VehicleTaskId = vehicleTaskId;
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);

        await InvalidateCachesAsync(vehicleTask);

        // ProductId bilgisini almak için manager'dan tekrar model olarak çekiyoruz.
        return await GetAsync(inserted.Id);
    }

    /// <summary>
    /// Araç-görev kaleminin tahsis miktarını günceller.
    /// </summary>
    [UnitOfWork]
    public async Task<VehicleTaskLineDto> UpdateAsync(Guid vehicleTaskId, Guid lineId, UpdateVehicleTaskLineDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var vehicleTask = await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);
        var existing = await _manager.EnsureBelongsToVehicleTaskAsync(vehicleTaskId, lineId);

        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        await _repository.UpdateAsync(existing, autoSave: true);

        await InvalidateCachesAsync(vehicleTask);

        return await GetAsync(lineId);
    }

    /// <summary>
    /// Teslim/iade uzlasmasi almamis arac-gorev kalemini siler.
    /// </summary>
    [UnitOfWork]
    public async Task DeleteAsync(Guid vehicleTaskId, Guid lineId)
    {
        var vehicleTask = await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);
        await _manager.EnsureBelongsToVehicleTaskAsync(vehicleTaskId, lineId);

        await _manager.EnsureCanDeleteAsync(lineId);
        await _repository.DeleteAsync(lineId);
        await InvalidateCachesAsync(vehicleTask);
    }

    /// <summary>
    /// İlgili cache kayıtlarını geçersiz kılar.
    /// </summary>
    private Task InvalidateCachesAsync(VehicleTask vehicleTask)
    {
        return _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(
            CacheKeys.TaskInventory(vehicleTask.TaskId),
            CacheKeys.TaskVehicles(vehicleTask.TaskId),
            CacheKeys.VehicleInventories(vehicleTask.VehicleId)));
    }
}
