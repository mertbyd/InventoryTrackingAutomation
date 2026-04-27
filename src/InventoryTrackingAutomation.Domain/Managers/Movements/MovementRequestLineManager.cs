using AutoMapper;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Models.Movements;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Hareket talebi satırı domain manager'ı — MovementRequestLine entity'si için iş kuralları ve validasyonları.
/// </summary>
public class MovementRequestLineManager : BaseManager<MovementRequestLine>
{
    private readonly IMovementRequestRepository _movementRequestRepository;  // MovementRequestId FK validasyonu için
    private readonly IProductRepository _productRepository;                   // ProductId FK validasyonu için

    /// <summary>
    /// MovementRequestLineManager constructor'ı.
    /// </summary>
    private readonly IMapper _mapper;
    public MovementRequestLineManager(
        IMovementRequestLineRepository repository,
        IMovementRequestRepository movementRequestRepository,
        IProductRepository productRepository,
        IMapper mapper)
        : base(repository)
    {
        _mapper = mapper;
        _movementRequestRepository = movementRequestRepository;
        _productRepository = productRepository;
    }

    /// <summary>
    /// Yeni hareket talebi satırı oluşturur — MovementRequestId ve ProductId varlık kontrolü yapar.
    /// </summary>
    public async Task<MovementRequestLine> CreateAsync(CreateMovementRequestLineModel model)
    {
        await EnsureExistsInAsync(
            _movementRequestRepository,
            model.MovementRequestId);

        await EnsureExistsInAsync(
            _productRepository,
            model.ProductId);

        var entity = new MovementRequestLine(GuidGenerator.Create());
        _mapper.Map(model, entity);
        return entity;
    }

    /// <summary>
    /// Hareket talebi satırını günceller — MovementRequestId ve ProductId varlık kontrolleri yapar.
    /// </summary>
    public async Task<MovementRequestLine> UpdateAsync(MovementRequestLine existing, UpdateMovementRequestLineModel model)
    {
        if (existing.MovementRequestId != model.MovementRequestId)
        {
            await EnsureExistsInAsync(
                _movementRequestRepository,
                model.MovementRequestId);
        }

        if (existing.ProductId != model.ProductId)
        {
            await EnsureExistsInAsync(
                _productRepository,
                model.ProductId);
        }

        _mapper.Map(model, existing);
        return existing;
    }
}

