using System.Linq;
using InventoryTrackingAutomation.Application.DisplayReferences;
using InventoryTrackingAutomation.Dtos.Masters;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Lookups;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Entities.Movements;
using Xunit;

namespace InventoryTrackingAutomation.DisplayReferences;

/// <summary>
/// Sifir-config display reference convention motorunun FK tespiti, entity eslesmesi,
/// display alan secimi ve cache davranisini uctan uca dogrular.
/// </summary>
public class DisplayReferenceConvention_Tests
{
    private readonly DisplayReferenceConvention _convention = new();

    [Fact]
    public void Should_Detect_Guid_Id_Foreign_Keys_And_Skip_Base_Id()
    {
        var names = _convention.GetReferences(typeof(ProductDto))
            .Select(reference => reference.PropertyName)
            .ToList();

        Assert.Contains(nameof(ProductDto.CategoryId), names);
        Assert.Contains(nameof(ProductDto.UnitTypeId), names);
        Assert.DoesNotContain(nameof(ProductDto.Id), names);
    }

    [Fact]
    public void Should_Map_Code_And_Name_When_Both_Exist()
    {
        var category = _convention.GetReferences(typeof(ProductDto))
            .Single(reference => reference.PropertyName == nameof(ProductDto.CategoryId));

        Assert.Equal(nameof(ProductCategory), category.EntityType.Name);
        Assert.Equal(nameof(ProductCategory.Code), category.CodeProperty?.Name);
        Assert.Equal(nameof(ProductCategory.Name), category.NameProperty?.Name);
    }

    [Fact]
    public void Should_Resolve_Prefixed_Fk_Stem_To_Base_Entity_Via_Suffix_Match()
    {
        var manager = _convention.GetReferences(typeof(WarehouseDto))
            .Single(reference => reference.PropertyName == nameof(WarehouseDto.ManagerWorkerId));

        Assert.Equal(nameof(Worker), manager.EntityType.Name);
    }

    [Fact]
    public void Should_Prefer_Exact_Entity_Match_Over_Suffix()
    {
        var unitType = _convention.GetReferences(typeof(ProductDto))
            .Single(reference => reference.PropertyName == nameof(ProductDto.UnitTypeId));

        Assert.Equal(nameof(UnitType), unitType.EntityType.Name);
    }

    [Fact]
    public void Should_Use_Number_Suffix_As_Code_When_No_Name_Or_Code()
    {
        var parent = _convention.GetReferences(typeof(MovementRequestDto))
            .Single(reference => reference.PropertyName == nameof(MovementRequestDto.ParentMovementRequestId));

        Assert.Equal(nameof(MovementRequest), parent.EntityType.Name);
        Assert.Equal(nameof(MovementRequest.RequestNumber), parent.CodeProperty?.Name);
        Assert.Null(parent.NameProperty);
    }

    [Fact]
    public void Should_Skip_Fk_When_Target_Entity_Has_No_Display_Field()
    {
        // VehicleTask sadece FK ve tarih alanlari tasir; display alani yok -> atlanir.
        var names = _convention.GetReferences(typeof(MovementRequestDto))
            .Select(reference => reference.PropertyName)
            .ToList();

        Assert.DoesNotContain(nameof(MovementRequestDto.VehicleTaskId), names);
    }

    [Fact]
    public void Should_Skip_Fk_When_Target_Is_Outside_Domain_Catalog()
    {
        // UserId -> Identity modulu; domain katalogunda yok -> atlanir.
        var names = _convention.GetReferences(typeof(WorkerDto))
            .Select(reference => reference.PropertyName)
            .ToList();

        Assert.DoesNotContain(nameof(WorkerDto.UserId), names);
    }

    [Fact]
    public void Should_Cache_Result_Per_Dto_Type()
    {
        var first = _convention.GetReferences(typeof(ProductDto));
        var second = _convention.GetReferences<ProductDto>();

        Assert.Same(first, second);
    }
}
