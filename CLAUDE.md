# CLAUDE.md

This file provides guidance to Claude Code when working with this repository. It mirrors `AGENTS.md` so all local agents share the same current model.

## Build And Run

```bash
dotnet build InventoryTrackingAutomation.sln
dotnet run --project host/InventoryTrackingAutomation.HttpApi.Host
dotnet run --project host/InventoryTrackingAutomation.AuthServer
dotnet test
```

Angular commands run inside `angular/`:

```bash
ng serve
ng build
ng test
```

## Current Core Model

The system is normalized around task demand lines and vehicle assignment lines:

```text
InventoryTask
  -> TaskLine
  -> VehicleTask
      -> VehicleTaskLine
      -> MovementRequest
          -> InventoryTransaction
```

Meaning:

- `InventoryTask.Type` decides whether the process is `WarehouseTransfer` or `FieldOperation`.
- `InventoryTask.SourceWarehouseId`, `TargetWarehouseId`, and `ReturnWarehouseId` own warehouse route context.
- `TaskLine.TaskId`, `ProductId`, and `Quantity` own the product demand for the task.
- `VehicleTask.TaskId`, `VehicleId`, and `ResponsibleWorkerId` own the vehicle assignment.
- `VehicleTaskLine.VehicleTaskId`, `TaskLineId`, and `AllocatedQuantity` own how much of a task line goes to that vehicle assignment.
- `VehicleTaskLine` also stores return reconciliation fields: `ReceivedQuantity`, `DamagedQuantity`, `LostQuantity`, `ConsumedQuantity`, `ReceiveNote`.
- `MovementRequest.VehicleTaskId` points to the task/vehicle context.
- `MovementRequest.ParentMovementRequestId` identifies return movements.
- `InventoryTransaction.RelatedMovementRequestId` is the ledger link.

Important: `ProductId` is not duplicated on `VehicleTaskLine`. Resolve product through `VehicleTaskLine.TaskLineId -> TaskLine.ProductId`.

Never add these back:

- `MovementRequest.Type`
- `MovementRequest.RequestedVehicleId`
- `MovementRequest.AssignedTaskId`
- `MovementRequest.TaskId`
- `MovementRequest.SourceWarehouseId`
- `MovementRequest.TargetWarehouseId`
- `InventoryTransaction.RelatedTaskId`
- `MovementRequestTypeEnum`
- production `MovementRequestLine` API/entity/service/controller/repository surface
- `POST /api/movement-requests/with-lines`

## Layer Conventions

- Domain rules live in `src/InventoryTrackingAutomation.Domain/Managers/`.
- AppServices stay thin: resolve current user/worker, map DTOs, call managers, publish cache events.
- Use repository interfaces from `Domain/Interface/`.
- Register EF repositories in `InventoryTrackingAutomationEntityFrameworkCoreModule`.
- Keep entities FK-only; do not add navigation properties.
- Use `MovementRequestOperationalContextModel` for movement decisions.
- Use `TaskLine` and `VehicleTaskLine` for product quantities; do not make movement request own lines again.

## Workflow And Stock Rules

Workflow selection comes from `InventoryTask.Type`, not `MovementRequest.Type`.

Dispatch:

- allowed only from `Approved`
- blocked for return flows
- reads transfer quantities from `VehicleTaskLine`
- resolves product through `TaskLine`
- moves `Warehouse -> Vehicle`
- starts a `Draft` task as `InProgress`

Receive:

- warehouse transfer moves `Vehicle -> TargetWarehouse`, completes task, releases vehicle
- field operation main receive completes the movement but leaves stock on vehicle
- return receive splits `VehicleTaskLine` quantities into received/damaged/lost/consumed, writes reconciliation back to `VehicleTaskLine`, then releases vehicle

## Database Notes

Current FK shape:

- `operation.task_lines.TaskId -> operation.tasks.Id`
- `operation.task_lines.ProductId -> master.products.Id`
- `operation.vehicle_task_lines.VehicleTaskId -> operation.vehicle_tasks.Id`
- `operation.vehicle_task_lines.TaskLineId -> operation.task_lines.Id`
- `movement.movement_requests.VehicleTaskId -> operation.vehicle_tasks.Id`
- `movement.movement_requests.ParentMovementRequestId -> movement.movement_requests.Id`
- `operation.vehicle_tasks.TaskId -> operation.tasks.Id`
- `operation.tasks.SourceWarehouseId/TargetWarehouseId/ReturnWarehouseId -> master.warehouses.Id`
- `stock.inventory_transactions.RelatedMovementRequestId -> movement.movement_requests.Id`

Recent migrations in this worktree:

- `20260429111119_NormalizeTaskVehicleMovementFlow`
- `20260430110525_MoveMovementRouteToTask`
- `20260501094502_AddTaskLineAndVehicleTaskLine`
- `20260501132731_RemoveMovementRequestLine`
- `20260501142000_NormalizeTaskLineVehicleTaskLine`

## Test Guidance

- Create real FK rows in integration tests.
- Run query/repository logic inside UnitOfWork.
- Keep SQLite EFCore tests non-parallel.
- Regression searches for removed movement fields should only hit migrations or historical warning docs.

Critical process tests:

`test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/EntityFrameworkCore/Movements/MovementFlow_Integration_Tests.cs`

Run:

```bash
dotnet test test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/InventoryTrackingAutomation.EntityFrameworkCore.Tests.csproj --no-restore --filter MovementFlow_Integration_Tests
```

Latest verified result: `4 passed, 0 failed, 0 skipped`.

The process tests write step-by-step markdown logs to:

`test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/TestResults/movement-flow-logs`

Expected log files:

- `wt.md`
- `fo.md`
- `wt-ins.md`
- `fo-dup.md`

Each log shows what happened in that step and snapshots these tables:

- `operation.tasks`
- `operation.task_lines`
- `operation.vehicle_tasks`
- `operation.vehicle_task_lines`
- `movement.movement_requests`
- `movement.movement_approvals`
- `workflow.workflow_instances`
- `workflow.workflow_instance_steps`
- `stock.stock_locations`
- `stock.inventory_transactions`

## Wiki

External wiki path:

`C:\Users\mertb\OneDrive\Belgeler\InventoryWiki\wiki`

Start with `current-system-state-for-claude.md`.

