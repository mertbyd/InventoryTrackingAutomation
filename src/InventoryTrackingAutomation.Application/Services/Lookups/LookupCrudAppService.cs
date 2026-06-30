using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentValidation;
using Volo.Abp.Application.Dtos;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;

namespace InventoryTrackingAutomation.Application.Services.Lookups;

// islevi: Tum lookup (referans) tablolari icin ortak CRUD operasyonlarini saglar.
// sistemdeki gorevi: AppService katmanindaki kod tekrarini onleyerek, UnitType, VehicleType gibi lookup entity'lerinin standart CRUD akisini tek merkezden yonetir.
public abstract class LookupCrudAppService<TEntity, TDto, TCreateDto, TUpdateDto, TCreateModel, TUpdateModel> 
    : InventoryTrackingAutomationAppService
    where TEntity : class, IEntity<Guid>
    where TDto : class
    where TCreateDto : class
    where TUpdateDto : class
{
    protected readonly IRepository<TEntity, Guid> Repository;

    protected LookupCrudAppService(
        IAbpLazyServiceProvider abpLazyServiceProvider,
        IRepository<TEntity, Guid> repository) 
        : base(abpLazyServiceProvider)
    {
        Repository = repository;
    }
    
    protected abstract Task<TEntity> EnsureExistsAsync(Guid id);
    protected abstract Task<TEntity> CreateEntityAsync(TCreateModel model);
    protected abstract Task<TEntity> UpdateEntityAsync(TEntity entity, TUpdateModel model);

    protected IValidator<TCreateDto> CreateValidator => LazyGetRequiredService<IValidator<TCreateDto>>();
    protected IValidator<TUpdateDto> UpdateValidator => LazyGetRequiredService<IValidator<TUpdateDto>>();
    protected IMapper Mapper => LazyGetRequiredService<IMapper>();

    public virtual async Task<TDto> GetAsync(Guid id)
    {
        var entity = await EnsureExistsAsync(id);
        return Mapper.Map<TEntity, TDto>(entity);
    }

    public virtual async Task<PagedResultDto<TDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await Repository.GetCountAsync();
        var entities = await Repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<TDto>(totalCount, Mapper.Map<List<TEntity>, List<TDto>>(entities));
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto input)
    {
        await CreateValidator.ValidateAndThrowAsync(input);
        var model = Mapper.Map<TCreateDto, TCreateModel>(input);
        var entity = await CreateEntityAsync(model);
        var inserted = await Repository.InsertAsync(entity, autoSave: true);
        return Mapper.Map<TEntity, TDto>(inserted);
    }

    public virtual async Task<List<TDto>> CreateManyAsync(List<TCreateDto> inputs)
    {
        var entities = new List<TEntity>();
        foreach (var dto in inputs)
        {
            await CreateValidator.ValidateAndThrowAsync(dto);
            var model = Mapper.Map<TCreateDto, TCreateModel>(dto);
            entities.Add(await CreateEntityAsync(model));
        }

        await Repository.InsertManyAsync(entities, autoSave: true);
        return Mapper.Map<List<TEntity>, List<TDto>>(entities);
    }

    public virtual async Task<TDto> UpdateAsync(Guid id, TUpdateDto input)
    {
        await UpdateValidator.ValidateAndThrowAsync(input);
        var existing = await EnsureExistsAsync(id);
        var model = Mapper.Map<TUpdateDto, TUpdateModel>(input);
        var updated = await UpdateEntityAsync(existing, model);
        var saved = await Repository.UpdateAsync(updated, autoSave: true);
        return Mapper.Map<TEntity, TDto>(saved);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var existing = await EnsureExistsAsync(id);
        await Repository.DeleteAsync(existing, autoSave: true);
    }
}
