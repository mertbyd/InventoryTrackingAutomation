using InventoryTrackingAutomation.Application.Mappers.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Events.Cache;
using InventoryTrackingAutomation.Interface.Tasks;
using InventoryTrackingAutomation.Managers.Tasks;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using FluentValidation;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.EventBus.Local;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Tasks;

// Arac-gorev atamasi application servisi - is kurallari VehicleTaskManager ve VehicleTaskLineManager'da kalir.
public class VehicleTaskAppService : InventoryTrackingAutomationAppService, IVehicleTaskAppService
{
    public VehicleTaskAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IVehicleTaskRepository _repository => LazyGetRequiredService<IVehicleTaskRepository>();
    private IVehicleTaskLineRepository _vehicleTaskLineRepository => LazyGetRequiredService<IVehicleTaskLineRepository>();
    private VehicleTaskManager _manager => LazyGetRequiredService<VehicleTaskManager>();
    private VehicleTaskLineManager _vehicleTaskLineManager => LazyGetRequiredService<VehicleTaskLineManager>();
    private IVehicleTaskLineAppService _vehicleTaskLineAppService => LazyGetRequiredService<IVehicleTaskLineAppService>();
    // Task-arac cache anahtarlari degisince local event ile temizlenir.
    private ILocalEventBus _localEventBus => LazyGetRequiredService<ILocalEventBus>();
    private IValidator<CreateVehicleTaskDto> _createValidator => LazyGetRequiredService<IValidator<CreateVehicleTaskDto>>();
    private IValidator<UpdateVehicleTaskDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateVehicleTaskDto>>();
    private static readonly VehicleTaskMapper _mapper = new VehicleTaskMapper();

    public async Task<VehicleTaskDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return await MapVehicleTaskWithLinesAsync(entity);
    }

    public async Task<PagedResultDto<VehicleTaskDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<VehicleTaskDto>(totalCount, _mapper.MapToDto(entities));
    }

    [UnitOfWork]
    public async Task<VehicleTaskDto> CreateAsync(CreateVehicleTaskDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new VehicleTask(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);

        if (input.Lines is { Count: > 0 })
        {
            foreach (var lineDto in input.Lines)
            {
                await _vehicleTaskLineAppService.CreateForVehicleTaskAsync(inserted.Id, lineDto);
            }
        }

        await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(CacheKeys.TaskVehicles(inserted.TaskId)));
        return await MapVehicleTaskWithLinesAsync(inserted);
    }

    [UnitOfWork]
    public async Task<List<VehicleTaskDto>> CreateManyAsync(List<CreateVehicleTaskDto> inputs)
    {
        var entities = new List<VehicleTask>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.MapToModel(dto);
            var validatedModel = await _manager.CreateAsync(model);
            var entity = new VehicleTask(GuidGenerator.Create());
            _mapper.MapToEntity(validatedModel, entity);
            var inserted = await _repository.InsertAsync(entity, autoSave: true);

            if (dto.Lines is { Count: > 0 })
            {
                foreach (var lineDto in dto.Lines)
                {
                    await _vehicleTaskLineAppService.CreateForVehicleTaskAsync(inserted.Id, lineDto);
                }
            }

            entities.Add(inserted);
        }

        var taskKeys = entities.Select(e => CacheKeys.TaskVehicles(e.TaskId)).Distinct().ToArray();
        await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(taskKeys));
        return _mapper.MapToDto(entities);
    }

    [UnitOfWork]
    public async Task<VehicleTaskDto> UpdateAsync(Guid id, UpdateVehicleTaskDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(CacheKeys.TaskVehicles(saved.TaskId)));
        return _mapper.MapToDto(saved);
    }

    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        // islevi: Satir baglanmis arac-gorev atamasini fiziksel silmeye izin vermez.
        // sistemdeki gorevi: VehicleTaskLine ve MovementRequest gecmisini korurken soft-delete kolonlarina olan ihtiyaci kaldirir.
        var hasLines = (await _vehicleTaskLineRepository.GetByVehicleTaskIdAsync(id)).Any();
        if (hasLines)
        {
            throw new BusinessException(VehicleTaskExceptionCodes.CannotDeleteWithLines)
                .WithData("VehicleTaskId", id);
        }

        await _repository.DeleteAsync(existing, autoSave: true);
        await _localEventBus.PublishAsync(CacheInvalidationEto.ForKeys(CacheKeys.TaskVehicles(existing.TaskId)));
    }

    // ────────────────────── Lines ──────────────────────

    /// Arac-gorev kalemlerini getirmek icin kullanilir.
    public async Task<List<VehicleTaskLineDto>> GetLinesAsync(Guid vehicleTaskId)
    {
        return await _vehicleTaskLineAppService.GetByVehicleTaskAsync(vehicleTaskId);
    }

    [UnitOfWork]
    /// Arac-goreve yeni kalem eklemek icin kullanilir.
    public async Task<VehicleTaskLineDto> AddLineAsync(Guid vehicleTaskId, CreateVehicleTaskLineDto input)
    {
        return await _vehicleTaskLineAppService.CreateForVehicleTaskAsync(vehicleTaskId, input);
    }

    [UnitOfWork]
    /// Arac-gorev kalemini guncellemek icin kullanilir.
    public async Task<VehicleTaskLineDto> UpdateLineAsync(Guid vehicleTaskId, Guid lineId, UpdateVehicleTaskLineDto input)
    {
        return await _vehicleTaskLineAppService.UpdateAsync(vehicleTaskId, lineId, input);
    }

    [UnitOfWork]
    /// Arac-gorev kalemini silmek icin kullanilir.
    public async Task DeleteLineAsync(Guid vehicleTaskId, Guid lineId)
    {
        await _vehicleTaskLineAppService.DeleteAsync(vehicleTaskId, lineId);
    }

    private async Task<VehicleTaskDto> MapVehicleTaskWithLinesAsync(VehicleTask entity)
    {
        var dto = _mapper.MapToDto(entity);
        dto.Lines = await _vehicleTaskLineAppService.GetByVehicleTaskAsync(entity.Id);
        return dto;
    }
}
