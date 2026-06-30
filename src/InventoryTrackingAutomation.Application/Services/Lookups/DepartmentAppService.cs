using InventoryTrackingAutomation.Application.Mappers.Lookups;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Lookups;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Managers.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using InventoryTrackingAutomation.Services.Lookups;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Uow;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// Departman application servisi — HTTP endpoint'leri için ince orkestra katmanı; iş kuralları DepartmentManager'da.
//işlevi: Department iş mantığını koordine eder ve DTO dönüşümlerini yönetir.
//sistemdeki görevi: Uygulama katmanındaki kullanım senaryolarını (use-case) gerçekleştiren ana servis birimidir.
public class DepartmentAppService : InventoryTrackingAutomationAppService, IDepartmentAppService
{
    public DepartmentAppService(IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(abpLazyServiceProvider)
    {
    }

    // Read/list/persist için ana repository.
    private IDepartmentRepository _repository => LazyGetRequiredService<IDepartmentRepository>();
    // Domain manager — Code uniqueness ve diğer iş kuralları.
    private DepartmentManager _manager => LazyGetRequiredService<DepartmentManager>();
    private IValidator<CreateDepartmentDto> _createValidator => LazyGetRequiredService<IValidator<CreateDepartmentDto>>();
    private IValidator<UpdateDepartmentDto> _updateValidator => LazyGetRequiredService<IValidator<UpdateDepartmentDto>>();

    // Tüm bağımlılıkları DI ile alır.
    private static readonly DepartmentMapper _mapper = new DepartmentMapper();


    // Id ile departmanı getirir; yoksa EntityNotFoundException.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<DepartmentDto> GetAsync(Guid id)
    {
        var entity = await _manager.EnsureExistsAsync(id);
        return _mapper.MapToDto(entity);
    }

    // Departmanları sayfalı listeler.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<PagedResultDto<DepartmentDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await _repository.GetCountAsync();
        var entities = await _repository.GetPagedListAsync(
            input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<DepartmentDto>(
            totalCount,
            _mapper.MapToDto(entities));
    }

    // Yeni departman oluşturur — manager iş kurallarını uygular, repository persist eder.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<DepartmentDto> CreateAsync(CreateDepartmentDto input)
    {
        await _createValidator.ValidateAndThrowAsync(input);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.CreateAsync(model);
        var entity = new Department(GuidGenerator.Create());
        _mapper.MapToEntity(validatedModel, entity);
        var inserted = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(inserted);
    }

    public async Task<List<DepartmentDto>> CreateManyAsync(List<CreateDepartmentDto> inputs)
    {
        var models = new List<CreateDepartmentModel>();
        foreach (var dto in inputs)
        {
            await _createValidator.ValidateAndThrowAsync(dto);
            models.Add(_mapper.MapToModel(dto));
        }

        var validatedModels = await _manager.CreateManyAsync(models);
        
        var entities = new List<Department>();
        foreach (var model in validatedModels)
        {
            var entity = new Department(GuidGenerator.Create());
            _mapper.MapToEntity(model, entity);
            entities.Add(entity);
        }

        var inserted = await _repository.InsertManyAndGetListAsync(entities);
        return _mapper.MapToDto(inserted);
    }

    // Departmanı günceller — manager iş kurallarını uygular, repository persist eder.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task<DepartmentDto> UpdateAsync(Guid id, UpdateDepartmentDto input)
    {
        await _updateValidator.ValidateAndThrowAsync(input);
        var existing = await _manager.EnsureExistsAsync(id);
        var model = _mapper.MapToModel(input);
        var validatedModel = await _manager.UpdateAsync(existing, model);
        _mapper.MapToEntity(validatedModel, existing);
        var saved = await _repository.UpdateAsync(existing, autoSave: true);
        return _mapper.MapToDto(saved);
    }

    // Departmanı soft delete ile siler.
    //işlevi: İlgili iş senaryosunu (use-case) yürütür.
    //sistemdeki görevi: Uygulama katmanındaki bir operasyonu atomik olarak gerçekleştirir.
    public async Task DeleteAsync(Guid id)
    {
        var existing = await _manager.EnsureExistsAsync(id);
        // islevi: Lookup entity artik ISoftDelete tasimadigi icin soft-delete yerine fiziksel delete kullanilir.
        // sistemdeki gorevi: Kullanilan departmanlarin FK ile korunmasini, bos referanslarin ise temizlenebilmesini saglar.
        await _repository.DeleteAsync(existing, autoSave: true);
    }
}

