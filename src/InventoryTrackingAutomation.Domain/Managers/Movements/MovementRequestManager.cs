using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Movements;
using InventoryTrackingAutomation.Interface.Masters;
using InventoryTrackingAutomation.Interface.Movements;
using InventoryTrackingAutomation.Interface.Shipments;
using InventoryTrackingAutomation.Interface.Workflows;
using InventoryTrackingAutomation.Models.Movements;

namespace InventoryTrackingAutomation.Managers.Movements;

/// <summary>
/// Hareket talebi domain manager'ı — MovementRequest entity'si için iş kuralları ve validasyonları.
/// </summary>
public class MovementRequestManager : BaseManager<MovementRequest>
{
    private readonly ISiteRepository _siteRepository;        // SourceSiteId / TargetSiteId FK validasyonu için
    private readonly IWorkerRepository _workerRepository;    // RequestedByWorkerId FK validasyonu için
    private readonly IShipmentRepository _shipmentRepository; // ShipmentId FK validasyonu için
    private readonly Managers.Workflows.WorkflowManager _workflowManager;
    private readonly IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IWorkflowInstanceRepository _workflowInstanceRepository;

    /// <summary>
    /// MovementRequestManager constructor'ı.
    /// </summary>
    public MovementRequestManager(
        IMovementRequestRepository repository,
        ISiteRepository siteRepository,
        IWorkerRepository workerRepository,
        IShipmentRepository shipmentRepository,
        Managers.Workflows.WorkflowManager workflowManager,
        IWorkflowDefinitionRepository workflowDefinitionRepository,
        IWorkflowInstanceRepository workflowInstanceRepository)
        : base(repository)
    {
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
        _shipmentRepository = shipmentRepository;
        _workflowManager = workflowManager;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _workflowInstanceRepository = workflowInstanceRepository;
    }

    /// <summary>
    /// Yeni hareket talebi oluşturur — RequestNumber unique, FK varlık kontrolleri yapar ve Status'u Pending olarak atar.
    /// </summary>
    public async Task<MovementRequest> CreateAsync(CreateMovementRequestModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.RequestNumber))
        {
            await EnsureUniqueAsync(
                x => x.RequestNumber == model.RequestNumber,
                InventoryTrackingAutomationDomainErrorCodes.MovementRequests.RequestNumberNotUnique);
        }

        await EnsureExistsInAsync(
            _workerRepository,
            model.RequestedByWorkerId,
            InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);

        await EnsureExistsInAsync(
            _siteRepository,
            model.SourceSiteId,
            InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);

        await EnsureExistsInAsync(
            _siteRepository,
            model.TargetSiteId,
            InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);

        await EnsureExistsInAsync(
            _shipmentRepository,
            model.ShipmentId,
            InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);

        await EnsureValidEnumAsync(model.Priority, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
        await EnsureValidEnumAsync(model.Status, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses);

        var entity = MapAndAssignId(model);
        entity.Status = InventoryTrackingAutomation.Enums.MovementStatusEnum.Pending;  // Yeni talep her zaman Pending başlar
        return entity;
    }

    /// <summary>
    /// Hareket talebini günceller — RequestNumber unique (self hariç) ve FK varlık kontrolleri yapar.
    /// </summary>
    public async Task<MovementRequest> UpdateAsync(MovementRequest existing, UpdateMovementRequestModel model)
    {
        if (!string.IsNullOrWhiteSpace(model.RequestNumber) && existing.RequestNumber != model.RequestNumber)
        {
            await EnsureUniqueAsync(
                x => x.RequestNumber == model.RequestNumber,
                existing.Id,
                InventoryTrackingAutomationDomainErrorCodes.MovementRequests.RequestNumberNotUnique);
        }
        if (existing.RequestedByWorkerId != model.RequestedByWorkerId)
        {
            await EnsureExistsInAsync(
                _workerRepository,
                model.RequestedByWorkerId,
                InventoryTrackingAutomationDomainErrorCodes.Workers.NotFound);
        }
        if (existing.SourceSiteId != model.SourceSiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.SourceSiteId,
                InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        }
        if (existing.TargetSiteId != model.TargetSiteId)
        {
            await EnsureExistsInAsync(
                _siteRepository,
                model.TargetSiteId,
                InventoryTrackingAutomationDomainErrorCodes.Sites.NotFound);
        }
        if (model.ShipmentId.HasValue && existing.ShipmentId != model.ShipmentId)
        {
            await EnsureExistsInAsync(
                _shipmentRepository,
                model.ShipmentId.Value,
                InventoryTrackingAutomationDomainErrorCodes.Shipments.NotFound);
        }
        await EnsureValidEnumAsync(model.Priority, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementPriorities);
        await EnsureValidEnumAsync(model.Status, InventoryTrackingAutomation.Settings.InventoryTrackingAutomationSettings.Movements.AllowedMovementStatuses);
        return MapForUpdate(model, existing);
    }

    /// <summary>
    /// Hareket talebini oluşturur, iş akışını başlatır ve veritabanına kaydeder.
    /// </summary>
    public async Task<MovementRequest> CreateWithWorkflowAsync(CreateMovementRequestModel model, System.Guid currentUserId)
    {
        var entity = await CreateAsync(model);
        await AssignWorkflowAsync(entity, currentUserId);
        return await Repository.InsertAsync(entity, autoSave: true);
    }

    /// <summary>
    /// Toplu hareket taleplerini oluşturur, iş akışlarını başlatır ve veritabanına kaydeder.
    /// </summary>
    public async Task<System.Collections.Generic.List<MovementRequest>> CreateManyWithWorkflowAsync(System.Collections.Generic.List<CreateMovementRequestModel> models, System.Guid currentUserId)
    {
        var entities = new System.Collections.Generic.List<MovementRequest>();
        foreach (var model in models)
        {
            var entity = await CreateAsync(model);
            await AssignWorkflowAsync(entity, currentUserId);
            entities.Add(entity);
        }

        return await Repository.InsertManyAndGetListAsync(entities);
    }

    private async Task AssignWorkflowAsync(MovementRequest entity, System.Guid currentUserId)
    {
        var workflowDef = await _workflowDefinitionRepository.FindAsync(w => w.Name == "MovementRequest" && w.IsActive);
        if (workflowDef != null)
        {
            var worker = await _workerRepository.FindAsync(w => w.UserId == currentUserId);
            System.Guid? managerUserId = null;
            if (worker?.ManagerId != null)
            {
                var managerWorker = await _workerRepository.FindAsync(w => w.Id == worker.ManagerId.Value);
                managerUserId = managerWorker?.UserId;
            }
            var startModel = new InventoryTrackingAutomation.Models.Workflows.StartWorkflowModel
            {
                WorkflowDefinitionId = workflowDef.Id,
                EntityType = "MovementRequest",
                EntityId = entity.Id,
                InitiatorUserId = currentUserId,
                InitiatorsManagerUserId = managerUserId
            };

            var workflowInstance = await _workflowManager.StartWorkflowAsync(startModel);
            await _workflowInstanceRepository.InsertAsync(workflowInstance);
            entity.WorkflowInstanceId = workflowInstance.Id;
        }
    }
}
