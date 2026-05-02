using AutoMapper;
using InventoryTrackingAutomation.Dtos.Movements;
using InventoryTrackingAutomation.Entities.Movements;
using Shouldly;
using Xunit;

namespace InventoryTrackingAutomation.Mappings.Movements;

/*
 * TEST DIZINI: test/InventoryTrackingAutomation.Application.Tests/Mappings/Movements/
 * ACIKLAMA: 'MovementRequest' ile DTO'lar arasindaki AutoMapper donusumlerini test eder.
 * NEDEN BURADA?: Mapping islemleri genellikle Application katmaninda gerceklesir ve 
 * AutoMapper konfigurasyonunun (Profile) dogrulugundan emin olmak gerekir.
 */
public abstract class MovementRequestMapping_Tests<TStartupModule> : InventoryTrackingAutomationApplicationTestBase<TStartupModule>
    where TStartupModule : Volo.Abp.Modularity.IAbpModule
{
    private readonly IMapper _mapper;

    protected MovementRequestMapping_Tests()
    {
        _mapper = GetRequiredService<IMapper>();
    }

    /*
     * SENARYO: MovementRequest Entity'si MovementRequestDto'ya dogru haritalanmalidir.
     */
    [Fact]
    public void Should_Map_MovementRequest_To_Dto()
    {
        // ARRANGE
        var entity = new MovementRequest(Guid.NewGuid())
        {
            RequestNumber = "REQ-456",
            Status = Enums.MovementStatusEnum.Shipped,
            Priority = Enums.MovementPriorityEnum.Normal
        };

        // ACT
        var dto = _mapper.Map<MovementRequest, MovementRequestDto>(entity);

        // ASSERT
        dto.Id.ShouldBe(entity.Id);
        dto.RequestNumber.ShouldBe(entity.RequestNumber);
        dto.Status.ShouldBe(entity.Status);
    }

    /*
     * SENARYO: CreateMovementRequestDto, MovementRequest Entity'sine donusturulurken 
     * kritik alanlar (Id, Status vb.) ezilmemelidir (Ignore edilmelidir).
     */
    [Fact]
    public void Should_Map_CreateDto_To_Entity_Safely()
    {
        // ARRANGE
        var dto = new CreateMovementRequestDto
        {
            RequestNumber = "NEW-REQ",
            VehicleTaskId = Guid.NewGuid(),
            RequestNote = "Initial note"
        };

        // ACT
        var entity = _mapper.Map<CreateMovementRequestDto, MovementRequest>(dto);

        // ASSERT
        entity.RequestNumber.ShouldBe(dto.RequestNumber);
        entity.VehicleTaskId.ShouldBe(dto.VehicleTaskId);
        entity.RequestNote.ShouldBe(dto.RequestNote);
        entity.Id.ShouldBe(Guid.Empty); // Profile'da Ignore edildigi icin bos kalmali.
    }
}
