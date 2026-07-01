using InventoryTrackingAutomation.Application.Mappers.Inventory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Inventory;
using InventoryTrackingAutomation.Entities.Inventory;
using InventoryTrackingAutomation.Interface.Inventory;
using InventoryTrackingAutomation.Managers.Inventory;
using InventoryTrackingAutomation.Models.Inventory;
using InventoryTrackingAutomation.Services.Inventory;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Envanter hareketi application servisi - is kurallari InventoryTransactionManager'da kalir.
//işlevi: InventoryTransaction iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class InventoryTransactionAppService : InventoryTrackingAutomationAppService, IInventoryTransactionAppService
{
    public InventoryTransactionAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    private IInventoryTransactionRepository _repository => LazyGetRequiredService<IInventoryTransactionRepository>();
    private InventoryTransactionManager _manager => LazyGetRequiredService<InventoryTransactionManager>();
    private IValidator<CreateInventoryTransactionDto> _createValidator => LazyGetRequiredService<IValidator<CreateInventoryTransactionDto>>();
    private static readonly InventoryTransactionMapper _mapper = new InventoryTransactionMapper();



    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTransactionDto>(totalCount, _mapper.MapToDto(entities));
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> CreateAsync(CreateInventoryTransactionDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var entity = await _manager.CreateAsync(model);
        _mapper.MapToEntity(model, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<InventoryTransactionDto>> CreateManyAsync(List<CreateInventoryTransactionDto> inputs)
    {
        var models = new List<CreateInventoryTransactionModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<InventoryTransaction>();
        foreach (var model in validatedModels)
        {
            var entity = new InventoryTransaction(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> UpdateAsync(Guid id, UpdateInventoryTransactionDto input)
    {
        // InventoryTransaction append-only defter oldugu icin AppService sadece domain kuralini calistirir.
        var rejected = await _manager.RejectUpdateAsync(id);
        return _mapper.MapToDto(rejected);
    }

    [UnitOfWork]
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        // Soft delete kapatildigi icin silme istegi domain manager'da is kurali olarak reddedilir.
        await _manager.RejectDeleteAsync(id);
    }
}
