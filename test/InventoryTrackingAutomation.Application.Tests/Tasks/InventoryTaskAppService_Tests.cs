using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Services.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using InventoryTrackingAutomation;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Tasks;

/*
 * APPLICATION LAYER TESTS — InventoryTaskAppService CRUD ve Is Akisi.
 */
public abstract class InventoryTaskAppService_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IInventoryTaskAppService _inventoryTaskAppService;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;

    protected InventoryTaskAppService_Tests()
    {
        _inventoryTaskAppService = GetRequiredService<IInventoryTaskAppService>();
        _warehouseRepository = GetRequiredService<IRepository<Warehouse, Guid>>();
    }

    [Fact]
    public async Task Should_Create_Inventory_Task()
    {
        // ARRANGE
        var sourceWarehouse = await _warehouseRepository.InsertAsync(new Warehouse(Guid.NewGuid())
        {
            Code = $"APP-SRC-{Guid.NewGuid():N}"[..30],
            Name = "App source depo",
            IsActive = true
        }, autoSave: true);

        var input = new CreateInventoryTaskDto
        {
            Code = "TSK-APP-001",
            Name = "App Service Test Görevi",
            Type = InventoryTaskTypeEnum.FieldOperation,
            Status = TaskStatusEnum.Draft,
            SourceWarehouseId = sourceWarehouse.Id,
            StartDate = DateTime.UtcNow
        };

        // ACT
        var result = await _inventoryTaskAppService.CreateAsync(input);

        // ASSERT
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(input.Code, result.Code);
    }
}
