using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Stock;
using InventoryTrackingAutomation.Entities.Stock;
using InventoryTrackingAutomation.Interface.Stock;
using InventoryTrackingAutomation.Managers.Stock;
using InventoryTrackingAutomation.Models.Stock;
using InventoryTrackingAutomation.Services.Stock;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;

namespace InventoryTrackingAutomation.Application.Services.Stock;

// Ürün stok application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları ProductStockManager'da.
public class ProductStockAppService : InventoryTrackingAutomationAppService, IProductStockAppService
{
    // Read/list/persist için ana repository.
    private readonly IProductStockRepository _repository;
    // Domain manager — (ProductId, SiteId) unique ve FK kontrolleri.
    private readonly ProductStockManager _manager;

    // Tüm bağımlılıkları DI ile alır.
    private readonly IMapper _mapper;
    public ProductStockAppService(
        IProductStockRepository repository,
        ProductStockManager manager,
        IMapper mapper)
    {
        _mapper = mapper;
        _repository = repository;
        _manager = manager;
    }

    // Id ile stok kaydını getirir; yoksa EntityNotFoundException.
    public async Task<ProductStockDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.Map<ProductStock, ProductStockDto>(entity);
    }

    // Stok kayıtlarını sayfalı listeler.
    public async Task<PagedResultDto<ProductStockDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<ProductStockDto>(
            totalCount,
            _mapper.Map<List<ProductStock>, List<ProductStockDto>>(entities));
    }

    // Yeni stok kaydı oluşturur — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductStockDto> CreateAsync(CreateProductStockDto input)
    {
        var model = _mapper.Map<CreateProductStockDto, CreateProductStockModel>(input);
        var entity = await _manager.CreateAsync(model);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.Map<ProductStock, ProductStockDto>(inserted);
    }

    // Birden fazla stok kaydını toplu oluşturur.
    [UnitOfWork]
    public async Task<List<ProductStockDto>> CreateManyAsync(List<CreateProductStockDto> inputs)
    {
        var entities = new List<ProductStock>();
        foreach (var dto in inputs)
        {
            var model = _mapper.Map<CreateProductStockDto, CreateProductStockModel>(dto);
            entities.Add(await _manager.CreateAsync(model));
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.Map<List<ProductStock>, List<ProductStockDto>>(inserted);
    }

    // Stok kaydını günceller — manager iş kurallarını uygular, repository persist eder.
    [UnitOfWork]
    public async Task<ProductStockDto> UpdateAsync(Guid id, UpdateProductStockDto input)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.Map<UpdateProductStockDto, UpdateProductStockModel>(input);
        var updated = await _manager.UpdateAsync(existing, model);
        var saved = await _repository.UpdateAsync(updated, autoSave: true);
        return _mapper.Map<ProductStock, ProductStockDto>(saved);
    }

    // Stok kaydını soft delete ile siler.
    [UnitOfWork]
    public async Task DeleteAsync(Guid id)
    {
        await _manager.EnsureExistsAsync(id);
        await _repository.SoftDeleteAsync(id);
    }
}
