using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;
using Volo.Abp.DependencyInjection;

namespace InventoryTrackingAutomation.Managers.Lookups;

/// <summary>
/// Departman domain manager'ı — Department entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Department etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class DepartmentManager : BaseManager<Department>
{
    protected override string AlreadyExistsErrorCode => DepartmentExceptionCodes.AlreadyExists;

    /// <summary>
    /// DepartmentManager constructor'ı.
    /// </summary>

    public DepartmentManager(IDepartmentRepository repository,
        IAbpLazyServiceProvider abpLazyServiceProvider)
        : base(repository, abpLazyServiceProvider)
    {
    }

    /// <summary>
    /// Yeni departman oluşturur — Code unique kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<CreateDepartmentModel> CreateAsync(CreateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(x => x.Code == model.Code);
        }
        return model;
    }

    /// <summary>
    /// Birden fazla departman oluşturur — Toplu Code unique kontrolü yapar.
    /// </summary>
    public async Task<List<CreateDepartmentModel>> CreateManyAsync(List<CreateDepartmentModel> models)
    {
        var codes = models.Where(x => !string.IsNullOrWhiteSpace(x.Code)).Select(x => x.Code).ToList();
        if (codes.Any())
        {
            await EnsureUniqueBulkAsync(codes, x => x.Code);
        }
        return models;
    }

    /// <summary>
    /// Departmanı günceller — Code unique (self hariç) kontrolü yapar.
    /// </summary>
    //işlevi: Etki alanı kuralını veya validasyonunu işletir.
    //sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<UpdateDepartmentModel> UpdateAsync(Department existing, UpdateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }
        return model;
    }
}


