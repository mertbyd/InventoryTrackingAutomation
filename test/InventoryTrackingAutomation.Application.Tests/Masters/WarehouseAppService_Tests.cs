using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Services.Masters;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp;
using Xunit;
using InventoryTrackingAutomation;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Masters;

/*
 * APPLICATION LAYER TESTS — WarehouseAppService CRUD ve is akisi.
 * 
 * Bu testler AppService katmanini test eder. AppService; DTO dogrulama, 
 * Mapping ve Manager yonlendirmelerini koordine eden katmandir.
 */
public abstract class WarehouseAppService_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IWarehouseAppService _warehouseAppService;
    private readonly IWarehouseRepository _warehouseRepository;

    protected WarehouseAppService_Tests()
    {
        _warehouseAppService = GetRequiredService<IWarehouseAppService>();
        _warehouseRepository = GetRequiredService<IWarehouseRepository>();
    }

    /*
     * SENARYO 1: Yeni bir depo olusturma (CreateAsync) basariyla calismalidir.
     */
    [Fact]
    public async Task Should_Create_A_Valid_Warehouse()
    {
        // ARRANGE
        var input = new CreateWarehouseDto
        {
            Code = "WH-NEW-001",
            Name = "Yeni Merkez Depo",
            Address = "Ankara",
            IsActive = true
        };

        // ACT
        var result = await _warehouseAppService.CreateAsync(input);

        // ASSERT
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.Equal(input.Code, result.Code);
        
        // Veritabaninda gercekten var mi kontrol et
        var dbWarehouse = await _warehouseRepository.GetAsync(result.Id);
        Assert.NotNull(dbWarehouse);
    }

    /*
     * SENARYO 2: Olmayan bir depoyu getirmeye calismak EntityNotFoundException fırlatmalıdır.
     */
    [Fact]
    public async Task Should_Not_Get_Non_Existing_Warehouse()
    {
        await Assert.ThrowsAsync<Volo.Abp.Domain.Entities.EntityNotFoundException>(async () =>
        {
            await _warehouseAppService.GetAsync(Guid.NewGuid());
        });
    }

    /*
     * SENARYO 3: Depo bilgilerini guncelleme (UpdateAsync) basarili olmalidir.
     */
    [Fact]
    public async Task Should_Update_Warehouse()
    {
        // ARRANGE: Once bir depo yarat
        var initial = await _warehouseAppService.CreateAsync(new CreateWarehouseDto
        {
            Code = "WH-UP-001",
            Name = "Eski Isim",
            IsActive = true
        });

        var updateInput = new UpdateWarehouseDto
        {
            Code = "WH-UP-001",
            Name = "Guncellenmis Isim", // Isim degisiyor
            IsActive = false
        };

        // ACT
        var result = await _warehouseAppService.UpdateAsync(initial.Id, updateInput);

        // ASSERT
        Assert.Equal("Guncellenmis Isim", result.Name);
        Assert.False(result.IsActive);
    }
}
