using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.Managers.Lookups;

/// <summary>
/// Departman domain manager'ı — Department entity'si için iş kuralları ve validasyonları.
/// </summary>
//işlevi: Department etki alanı (domain) kurallarını ve karmaşık veri bütünlüğünü sağlar.
//sistemdeki görevi: Domain katmanındaki iş kurallarının merkezi yönetimini ve validasyonunu sağlar.
public class DepartmentManager : BaseManager<Department>
{
    /// <summary>
    /// DepartmentManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public DepartmentManager(
        IDepartmentRepository repository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
    }

    /// <summary>
    /// Yeni departman oluşturur — Code unique kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Department> CreateAsync(CreateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code);
        }
        var entity = new Department(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Departmanı günceller — Code unique (self hariç) kontrolü yapar.
    /// </summary>
//işlevi: Etki alanı kuralını veya validasyonunu işletir.
//sistemdeki görevi: Veri bütünlüğünü ve domain mantığını garanti altına alan düşük seviyeli operasyondur.
    public async Task<Department> UpdateAsync(Department existing, UpdateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id);
        }
        _mapper.Map(model, existing);
        return existing;
    }
}

