using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
    protected abstract Task<TCreateModel> CreateModelAsync(TCreateModel model);
    protected abstract Task<List<TCreateModel>> CreateModelsAsync(List<TCreateModel> models);
    protected abstract TEntity CreateEntity(TCreateModel model);
    protected abstract Task<TEntity> UpdateEntityAsync(TEntity entity, TUpdateModel model);

    protected IValidator<TCreateDto> CreateValidator => LazyGetRequiredService<IValidator<TCreateDto>>();
    protected IValidator<TUpdateDto> UpdateValidator => LazyGetRequiredService<IValidator<TUpdateDto>>();
    protected abstract TDto MapToDto(TEntity entity);
    protected abstract List<TDto> MapToDto(List<TEntity> entities);
    protected abstract TCreateModel MapToCreateModel(TCreateDto input);
    protected abstract TUpdateModel MapToUpdateModel(TUpdateDto input);

    public virtual async Task<TDto> GetAsync(Guid id)
    {
        var entity = await EnsureExistsAsync(id);
        return MapToDto(entity);
    }

    public virtual async Task<PagedResultDto<TDto>> GetListAsync(PagedResultRequestDto input)
    {
        var totalCount = await Repository.GetCountAsync();
        var entities = await Repository.GetPagedListAsync(input.SkipCount, input.MaxResultCount, sorting: string.Empty);
        return new PagedResultDto<TDto>(totalCount, MapToDto(entities));
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto input)
    {
        await CreateValidator.ValidateAndThrowAsync(input);
        var model = MapToCreateModel(input);
        var entity = await CreateEntityAsync(model);
        var inserted = await Repository.InsertAsync(entity, autoSave: true);
        return MapToDto(inserted);
    }

    protected virtual async Task<TEntity> CreateEntityAsync(TCreateModel model)
    {
        var validatedModel = await CreateModelAsync(model);
        return CreateEntity(validatedModel);
    }

    protected virtual async Task<List<TEntity>> CreateEntitiesAsync(List<TCreateModel> models)
    {
        var validatedModels = await CreateModelsAsync(models);
        var entities = new List<TEntity>(validatedModels.Count);
        foreach (var model in validatedModels)
        {
            entities.Add(CreateEntity(model));
        }

        return entities;
    }

    public virtual async Task<List<TDto>> CreateManyAsync(List<TCreateDto> inputs)
    {
        var models = new List<TCreateModel>();
        foreach (var dto in inputs)
        {
            await CreateValidator.ValidateAndThrowAsync(dto);
            models.Add(MapToCreateModel(dto));
        }

        var entities = await CreateEntitiesAsync(models);

        await Repository.InsertManyAsync(entities, autoSave: true);
        return MapToDto(entities);
    }

    public virtual async Task<TDto> UpdateAsync(Guid id, TUpdateDto input)
    {
        await UpdateValidator.ValidateAndThrowAsync(input);
        var existing = await EnsureExistsAsync(id);
        var model = MapToUpdateModel(input);
        var updated = await UpdateEntityAsync(existing, model);
        var saved = await Repository.UpdateAsync(updated, autoSave: true);
        return MapToDto(saved);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var existing = await EnsureExistsAsync(id);
        await Repository.DeleteAsync(existing, autoSave: true);
    }
}
