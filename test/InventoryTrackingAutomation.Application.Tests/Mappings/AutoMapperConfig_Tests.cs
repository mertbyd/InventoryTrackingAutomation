using System;
using AutoMapper;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Tasks;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Models.Masters;
using InventoryTrackingAutomation.Models.Tasks;
using InventoryTrackingAutomation.Models.Movements;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Enums.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Volo.Abp.Modularity;

namespace InventoryTrackingAutomation.Mappings;

/*
 * AUTOMAPPER TESTLERİ — Application.Tests katmanında abstract yazılır.
 *
 * NEDEN BURADA? AutoMapper profilleri Application katmanında tanımlanır (ObjectMapping klasörü).
 * Bu testler, Entity <-> DTO <-> Model dönüşümlerinin doğru çalıştığını garantiler.
 *
 * ÖNEMİ: Eğer bir property eklenir ama mapping profilde karşılığı yoksa,
 * bu testler başarısız olur ve bize "Şu alan map'lenmiyor!" uyarısı verir.
 */
public abstract class AutoMapperConfig_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{
    private readonly IMapper _mapper;

    protected AutoMapperConfig_Tests()
    {
        // ABP'nin DI konteynerinden AutoMapper instance'ını al.
        _mapper = GetRequiredService<IMapper>();
    }

    // ───────────────────────────────────────────────
    //  MASTERS — Warehouse Mapping
    // ───────────────────────────────────────────────

    /*
     * SENARYO 1: Warehouse entity → WarehouseDto dönüşümü doğru çalışmalıdır.
     * Tüm property'ler eksiksiz map'lenmelidir.
     */
    [Fact]
    public void Warehouse_Entity_To_Dto_Should_Map()
    {
        var entity = new Warehouse(Guid.NewGuid())
        {
            Code = "WH-001",
            Name = "Ana Depo",
            IsActive = true
        };

        // Map işlemini çalıştır.
        var dto = _mapper.Map<Warehouse, WarehouseDto>(entity);

        // Tüm alanlar doğru aktarıldı mı?
        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Code, dto.Code);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.IsActive, dto.IsActive);
    }

    /*
     * SENARYO 2: CreateWarehouseDto → CreateWarehouseModel dönüşümü.
     * AppService'ten Manager'a veri aktarımı sırasında kullanılır.
     */
    [Fact]
    public void CreateWarehouseDto_To_Model_Should_Map()
    {
        var dto = new CreateWarehouseDto
        {
            Code = "WH-002",
            Name = "Yedek Depo",
            IsActive = true
        };

        var model = _mapper.Map<CreateWarehouseDto, CreateWarehouseModel>(dto);

        Assert.Equal(dto.Code, model.Code);
        Assert.Equal(dto.Name, model.Name);
    }

    // ───────────────────────────────────────────────
    //  MASTERS — Product Mapping
    // ───────────────────────────────────────────────

    /*
     * SENARYO 3: Product entity → ProductDto dönüşümü.
     */
    [Fact]
    public void Product_Entity_To_Dto_Should_Map()
    {
        var entity = new Product(Guid.NewGuid())
        {
            Code = "PRD-001",
            Name = "Test Ürün",
            BaseUnit = UnitTypeEnum.Piece,
            IsActive = true
        };

        var dto = _mapper.Map<Product, ProductDto>(entity);

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Code, dto.Code);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.BaseUnit, dto.BaseUnit);
    }

    // ───────────────────────────────────────────────
    //  TASKS — InventoryTask Mapping
    // ───────────────────────────────────────────────

    /*
     * SENARYO 4: InventoryTask entity → InventoryTaskDto dönüşümü.
     */
    [Fact]
    public void InventoryTask_Entity_To_Dto_Should_Map()
    {
        var entity = new InventoryTask(Guid.NewGuid())
        {
            Code = "TSK-001",
            Name = "Saha Operasyonu",
            Type = InventoryTaskTypeEnum.FieldOperation,
            Status = TaskStatusEnum.Draft,
            StartDate = DateTime.UtcNow
        };

        var dto = _mapper.Map<InventoryTask, InventoryTaskDto>(entity);

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.Code, dto.Code);
        Assert.Equal(entity.Name, dto.Name);
        Assert.Equal(entity.Type, dto.Type);
        Assert.Equal(entity.Status, dto.Status);
    }

    /*
     * SENARYO 5: CreateInventoryTaskDto → CreateInventoryTaskModel dönüşümü.
     */
    [Fact]
    public void CreateInventoryTaskDto_To_Model_Should_Map()
    {
        var dto = new CreateInventoryTaskDto
        {
            Code = "TSK-002",
            Name = "Depo Transferi",
            Type = InventoryTaskTypeEnum.WarehouseTransfer,
            Status = TaskStatusEnum.Draft,
            StartDate = DateTime.UtcNow
        };

        var model = _mapper.Map<CreateInventoryTaskDto, CreateInventoryTaskModel>(dto);

        Assert.Equal(dto.Code, model.Code);
        Assert.Equal(dto.Name, model.Name);
        Assert.Equal(dto.Type, model.Type);
    }

    // ───────────────────────────────────────────────
    //  MOVEMENTS — MovementRequest Mapping
    // ───────────────────────────────────────────────

    /*
     * SENARYO 6: MovementRequest entity → MovementRequestDto dönüşümü.
     */
    [Fact]
    public void MovementRequest_Entity_To_Dto_Should_Map()
    {
        var entity = new MovementRequest(Guid.NewGuid())
        {
            RequestNumber = "MR-001",
            Status = MovementStatusEnum.Pending,
            Priority = MovementPriorityEnum.Normal,
            RequestNote = "Test talebi",
            PlannedDate = DateTime.UtcNow,
            RequestedByWorkerId = Guid.NewGuid(),
            VehicleTaskId = Guid.NewGuid()
        };

        var dto = _mapper.Map<MovementRequest, MovementRequestDto>(entity);

        Assert.Equal(entity.Id, dto.Id);
        Assert.Equal(entity.RequestNumber, dto.RequestNumber);
        Assert.Equal(entity.Status, dto.Status);
        Assert.Equal(entity.Priority, dto.Priority);
    }

    // ───────────────────────────────────────────────
    //  GENEL — Tüm Profillerin Konfigürasyon Geçerliliği
    // ───────────────────────────────────────────────

    /*
     * SENARYO 7: AutoMapper konfigürasyonunun tamamen geçerli olduğunu doğrular.
     * Eğer herhangi bir mapping profilinde eksik veya yanlış tanım varsa,
     * bu test BAŞARISIZ olur ve bize hangi mapping'in bozuk olduğunu söyler.
     *
     * Bu TEK TEST, projedeki TÜM mapping profillerini kapsar.
     * Yeni bir entity/dto/model eklendiğinde bu test onu da otomatik doğrular.
     */
    [Fact]
    public void AutoMapper_Configuration_Should_Be_Valid()
    {
        // Bu çağrı, tüm profillerdeki mapping kurallarını kontrol eder.
        // Eksik ForMember, yanlış tür eşleştirmesi vs. varsa anında hata verir.
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}
