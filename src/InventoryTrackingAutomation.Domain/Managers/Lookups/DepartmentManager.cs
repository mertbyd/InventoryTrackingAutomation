using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Interface.Lookups;
using InventoryTrackingAutomation.Models.Lookups;

namespace InventoryTrackingAutomation.Managers.Lookups;

/// <summary>
/// Departman domain manager'ı — Department entity'si için iş kuralları ve validasyonları.
/// </summary>
public class DepartmentManager : BaseManager<Department>
{
    /// <summary>
    /// DepartmentManager constructor'ı.
    /// </summary>
    public DepartmentManager(
        IDepartmentRepository repository)
        : base(repository)
    {
    }

    /// <summary>
    /// Yeni departman oluşturur — Code unique kontrolü yapar.
    /// </summary>
    public async Task<Department> CreateAsync(CreateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code))
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                InventoryTrackingAutomationDomainErrorCodes.Departments.CodeNotUnique);
        }
        return MapAndAssignId(model);
    }

    /// <summary>
    /// Departmanı günceller — Code unique (self hariç) kontrolü yapar.
    /// </summary>
    public async Task<Department> UpdateAsync(Department existing, UpdateDepartmentModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.Code) && existing.Code != model.Code)
        {
            await EnsureUniqueAsync(
                x => x.Code == model.Code,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.Departments.CodeNotUnique);
        }
        return MapForUpdate(model, existing);
    }
}
