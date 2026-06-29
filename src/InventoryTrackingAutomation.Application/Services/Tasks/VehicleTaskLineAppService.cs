using AutoMapper;
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
    private IMapper _mapper => LazyGetRequiredService<IMapper>();

    /// <summary>
    /// Araç-görev kalemini getirir.
    /// </summary>
    public async Task<VehicleTaskLineDto> GetAsync(Guid id)
    {
        var model = await _manager.GetWithProductAsync(id);
        return MapToDto(model);
    }

    /// <summary>
    /// Bir araç-görev atamasına bağlı tüm kalemleri getirir.
    /// </summary>
    public async Task<List<VehicleTaskLineDto>> GetByVehicleTaskAsync(Guid vehicleTaskId)
    {
        await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);
        var models = await _manager.GetListWithProductByVehicleTaskAsync(vehicleTaskId);
        return models.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Araç-görev atamasına ürün tahsisi ekler.
    /// </summary>
    [UnitOfWork]
    public async Task<VehicleTaskLineDto> CreateForVehicleTaskAsync(Guid vehicleTaskId, CreateVehicleTaskLineDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var vehicleTask = await _manager.EnsureVehicleTaskExistsAsync(vehicleTaskId);

        var model = _mapper.Map<CreateVehicleTaskLineDto, CreateVehicleTaskLineModel>(input);
        var entity = await _manager.CreateAsync(vehicleTaskId, model);
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

        var model = _mapper.Map<UpdateVehicleTaskLineDto, UpdateVehicleTaskLineModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        await _repository.UpdateAsync(updated, autoSave: true);
        
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
        
        await _manager.DeleteAsync(lineId);
        await InvalidateCachesAsync(vehicleTask);
    }

    /// <summary>
    /// Modelden DTO'ya haritalama yapar.
    /// </summary>
    private VehicleTaskLineDto MapToDto(VehicleTaskLineWithProductModel model)
    {
        var dto = _mapper.Map<VehicleTaskLine, VehicleTaskLineDto>(model.Line);
        dto.ProductId = model.ProductId;
        return dto;
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
