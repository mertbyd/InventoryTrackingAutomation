using AutoMapper;
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
using InventoryTrackingAutomation.Managers.Inventory;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Envanter hareketi application servisi - is kurallari InventoryTransactionManager'da kalir.
//işlevi: InventoryTransaction iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class InventoryTransactionAppService : InventoryTrackingAutomationAppService, IInventoryTransactionAppService
{
    private readonly IInventoryTransactionRepository _repository;
    private readonly InventoryTransactionManager _manager;
    private readonly IValidator<CreateInventoryTransactionDto> _createValidator;
    private readonly IValidator<UpdateInventoryTransactionDto> _updateValidator;
    private readonly IMapper _mapper;

    public InventoryTransactionAppService(
        IInventoryTransactionRepository repository,
        InventoryTransactionManager manager,
        IValidator<CreateInventoryTransactionDto> createValidator,
        IValidator<UpdateInventoryTransactionDto> updateValidator,
        IMapper mapper)
    {
        _repository = repository;
        _manager = manager;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _mapper = mapper;
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(entity);
    }

//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<InventoryTransactionDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<InventoryTransactionDto>(totalCount, _mapper.Map<List<InventoryTransaction>, List<InventoryTransactionDto>>(entities));
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> CreateAsync(CreateInventoryTransactionDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.Map<CreateInventoryTransactionDto, CreateInventoryTransactionModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<List<InventoryTransactionDto>> CreateManyAsync(List<CreateInventoryTransactionDto> inputs)
    {
        var entities = new List<InventoryTransaction>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            var model = _mapper.Map<CreateInventoryTransactionDto, CreateInventoryTransactionModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<InventoryTransaction>, List<InventoryTransactionDto>>(inserted);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<InventoryTransactionDto> UpdateAsync(Guid id, UpdateInventoryTransactionDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateInventoryTransactionDto, UpdateInventoryTransactionModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<InventoryTransaction, InventoryTransactionDto>(saved);
    }

    [UnitOfWork]
//işlevi: İlgili iş senaryosunu (use-case) yürütür.
//sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
