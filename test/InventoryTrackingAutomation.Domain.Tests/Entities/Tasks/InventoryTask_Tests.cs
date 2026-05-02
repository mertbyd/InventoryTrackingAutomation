using System;
using InventoryTrackingAutomation.Entities.Tasks;
using InventoryTrackingAutomation.Enums.Tasks;
using Shouldly;
using Xunit;

namespace InventoryTrackingAutomation.Entities.Tasks;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Domain.Tests/Entities/Tasks/
 * ACIKLAMA: Bu sinif, 'InventoryTask' entity'sinin temel birim testlerini icerir.
 * NEDEN BURADA?: Entity'ler Domain katmaninin kalbidir ve herhangi bir bagimlilik (database, service vs.) 
 * gerektirmeden test edilebilirler (Unit Test). Bu yuzden Domain.Tests projesindedir.
 */
public class InventoryTask_Tests
{
    /*
     * SENARYO: Yeni bir InventoryTask olusturuldugunda alanlar dogru set edilmelidir.
     * Bu test, constructor ve property atamalarini dogrular.
     */
    [Fact]
    public void Should_Initialize_Correctly()
    {
        // ARRANGE: Test verilerini hazirla.
        var id = Guid.NewGuid();
        var code = "TASK-001";
        var name = "Test Gorevi";
        
        // ACT: Entity'yi olustur.
        var task = new InventoryTask(id)
        {
            Code = code,
            Name = name,
            Type = InventoryTaskTypeEnum.FieldOperation,
            Status = TaskStatusEnum.Draft
        };

        // ASSERT: Degerlerin dogrulugunu kontrol et.
        task.Id.ShouldBe(id);
        task.Code.ShouldBe(code);
        task.Name.ShouldBe(name);
        task.Type.ShouldBe(InventoryTaskTypeEnum.FieldOperation);
        task.Status.ShouldBe(TaskStatusEnum.Draft);
    }

    /*
     * SENARYO: Gorev tipi 'WarehouseTransfer' ise bolge (Region) bos olabilir, 
     * ancak veri atanabiliyor olmalidir.
     */
    [Fact]
    public void Should_Allow_Setting_Optional_Fields()
    {
        // ARRANGE
        var task = new InventoryTask(Guid.NewGuid());
        var region = "Istanbul-Anadolu";
        var description = "Acil sevkiyat gorevi";

        // ACT
        task.Region = region;
        task.Description = description;
        task.ReturnWarehouseId = Guid.NewGuid();

        // ASSERT
        task.Region.ShouldBe(region);
        task.Description.ShouldBe(description);
        task.ReturnWarehouseId.ShouldNotBeNull();
    }
}
