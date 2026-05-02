# Current Architecture And Workflow Baseline

Last updated: 2026-05-01

This file summarizes the current working-tree model. The old `MovementRequestLine` API/entity layer has been removed from the production surface. Movement quantities now come from task and vehicle-task line entities.

## Project Overview

InventoryTrackingAutomation is an ABP 10.3 / .NET 10 inventory workflow system for warehouse transfers and field operations.

## Core Entity Shape

```text
InventoryTask
  -> TaskLine
  -> VehicleTask
      -> VehicleTaskLine
      -> MovementRequest
          -> InventoryTransaction
```

Responsibilities:

- `InventoryTask`: operation/job, process type, warehouse route, return warehouse and task status.
- `TaskLine`: product and total requested quantity for the task.
- `VehicleTask`: vehicle assignment for an inventory task.
- `VehicleTaskLine`: allocation and return reconciliation for a task line on a specific vehicle task.
- `MovementRequest`: approval, dispatch and receive ticket linked by `VehicleTaskId`.
- `InventoryTransaction`: append-only stock ledger linked by `RelatedMovementRequestId`.

Important normalization rule:

- Product lives on `TaskLine.ProductId`.
- Vehicle allocation lives on `VehicleTaskLine.TaskLineId + AllocatedQuantity`.
- `VehicleTaskLine` does not duplicate `ProductId`.
- `MovementRequest` does not own product lines and has no `with-lines` creation endpoint.

## Critical Rules

- Process type comes from `InventoryTask.Type`.
- Warehouse route comes from `InventoryTask.SourceWarehouseId`, `TargetWarehouseId`, and `ReturnWarehouseId`.
- Vehicle context comes from `MovementRequest.VehicleTaskId -> VehicleTask.VehicleId`.
- Task context comes from `MovementRequest.VehicleTaskId -> VehicleTask.TaskId`.
- Product context comes from `VehicleTaskLine.TaskLineId -> TaskLine.ProductId`.
- Return flow is `MovementRequest.ParentMovementRequestId != null`.
- Ledger context is `InventoryTransaction.RelatedMovementRequestId`.

Removed production surface:

- `MovementRequestLine` entity, DTOs, validators, AppService, controller, repository and EF configuration
- `POST /api/movement-requests/with-lines`
- `CreateMovementRequestWithLinesDto`
- `MovementRequestManager.CreateWithLinesAndWorkflowAsync`

Do not reintroduce:

- `MovementRequest.Type`
- `MovementRequest.RequestedVehicleId`
- `MovementRequest.AssignedTaskId`
- `MovementRequest.TaskId`
- `MovementRequest.SourceWarehouseId`
- `MovementRequest.TargetWarehouseId`
- `InventoryTransaction.RelatedTaskId`
- `MovementRequestTypeEnum`
- movement-owned product line APIs

## Workflow

Workflow definition is selected in `MovementRequestManager.AssignWorkflowAsync` from `InventoryTask.Type`:

- `WarehouseTransfer` -> `MovementRequest`
- `FieldOperation` -> `TaskMovementRequest`

Approval changes movement state. Stock does not move during approval.

## Stock Lifecycle

Dispatch:

```text
InventoryTask.SourceWarehouseId -> VehicleTask.VehicleId
```

Dispatch reads transfer lines from:

```text
MovementRequest.VehicleTaskId
  -> VehicleTaskLine
      -> TaskLine.ProductId
```

Receive warehouse transfer:

```text
VehicleTask.VehicleId -> InventoryTask.TargetWarehouseId
```

Field operation main receive:

- completes the main movement
- keeps stock on the vehicle
- task remains active until explicitly completed

Return receive:

- reads expected return lines from `VehicleTaskLine`
- validates `received + damaged + lost + consumed == VehicleTaskLine.AllocatedQuantity`
- received quantity moves `Vehicle -> ReturnWarehouse`
- damaged/lost/consumed quantities are vehicle stock adjustments
- reconciliation values are written back to `VehicleTaskLine`
- vehicle assignment is released

## Key Files

| File | Role |
| --- | --- |
| `src/InventoryTrackingAutomation.Domain/Entities/Tasks/InventoryTask.cs` | Operation and route aggregate |
| `src/InventoryTrackingAutomation.Domain/Entities/Tasks/TaskLine.cs` | Task product demand line |
| `src/InventoryTrackingAutomation.Domain/Entities/Tasks/VehicleTask.cs` | Vehicle assignment aggregate |
| `src/InventoryTrackingAutomation.Domain/Entities/Tasks/VehicleTaskLine.cs` | Vehicle allocation and return reconciliation line |
| `src/InventoryTrackingAutomation.Domain/Entities/Movements/MovementRequest.cs` | Movement lifecycle ticket |
| `src/InventoryTrackingAutomation.Domain/Models/Movements/MovementRequestOperationalContextModel.cs` | Joined movement context |
| `src/InventoryTrackingAutomation.Domain/Managers/Movements/MovementRequestManager.cs` | Movement orchestration |
| `src/InventoryTrackingAutomation.Domain/Managers/Movements/TaskReturnRequestManager.cs` | Return request generation |
| `src/InventoryTrackingAutomation.Domain/Managers/Tasks/TaskLineManager.cs` | Task line rules |
| `src/InventoryTrackingAutomation.Domain/Managers/Tasks/VehicleTaskLineManager.cs` | Vehicle task line allocation/reconciliation rules |
| `src/InventoryTrackingAutomation.EntityFrameworkCore/Repository/Movements/MovementRequestRepository.cs` | Operational context query |
| `test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/EntityFrameworkCore/Movements/MovementFlow_Integration_Tests.cs` | Full movement process tests and table logs |

## Test Baseline

Latest verified targeted process test command:

```bash
dotnet test test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/InventoryTrackingAutomation.EntityFrameworkCore.Tests.csproj --no-restore --filter MovementFlow_Integration_Tests
```

Result: `4 passed, 0 failed, 0 skipped`.

Process test logs are written to:

```text
test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/TestResults/movement-flow-logs
```

Generated log files:

- `wt.md`: warehouse transfer lifecycle
- `fo.md`: field operation dispatch, task completion and return reconciliation
- `wt-ins.md`: insufficient source stock regression
- `fo-dup.md`: duplicate open return request regression

Important scenarios:

- warehouse transfer full lifecycle
- field operation main movement
- field operation return reconciliation
- real workflow approvals through `MovementApprovalManager`
- insufficient source stock blocks dispatch
- duplicate open return request prevention
- removed movement line API stays absent

External wiki:

`C:\Users\mertb\OneDrive\Belgeler\InventoryWiki\wiki`

