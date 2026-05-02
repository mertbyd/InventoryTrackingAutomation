# InventoryTrackingAutomation

InventoryTrackingAutomation is an ABP Framework 10.3 / .NET 10 inventory workflow system for warehouse transfers, field operations, vehicle assignments and append-only stock ledger tracking.

## Stack

- .NET 10
- ABP Framework 10.3.0
- PostgreSQL
- OpenIddict
- SignalR
- Angular

## Current Domain Model

The movement system is normalized around this entity chain:

```text
InventoryTask
  -> VehicleTask
      -> MovementRequest
          -> MovementRequestLine
              -> InventoryTransaction
```

| Entity | Purpose |
| --- | --- |
| `InventoryTask` | Operation/job. Owns process type and warehouse route. |
| `VehicleTask` | Vehicle-to-task assignment. Owns active vehicle context. |
| `MovementRequest` | Approval, dispatch and receive ticket. |
| `MovementRequestLine` | Requested product quantities and return reconciliation. |
| `InventoryTransaction` | Immutable stock ledger. |
| `StockLocation` | Current product balance by warehouse or vehicle. |

## Core Rules

- `InventoryTask.Type` selects the process family.
- `InventoryTask.SourceWarehouseId` and `InventoryTask.TargetWarehouseId` own the route.
- `MovementRequest.VehicleTaskId -> VehicleTask.TaskId` resolves task context.
- `MovementRequest.VehicleTaskId -> VehicleTask.VehicleId` resolves vehicle context.
- `MovementRequest.ParentMovementRequestId` identifies return flow.
- `InventoryTransaction.RelatedMovementRequestId` links ledger rows to movement requests.

Removed fields must stay removed:

- `MovementRequest.Type`
- `MovementRequest.RequestedVehicleId`
- `MovementRequest.AssignedTaskId`
- `MovementRequest.TaskId`
- `MovementRequest.SourceWarehouseId`
- `MovementRequest.TargetWarehouseId`
- `InventoryTransaction.RelatedTaskId`
- `MovementRequestTypeEnum`

## Movement Lifecycles

### Warehouse Transfer

1. Create `InventoryTask` with `type = WarehouseTransfer`, source warehouse and target warehouse.
2. Create movement with `taskId`, `vehicleId` and lines.
3. Workflow approves the movement.
4. Dispatch moves stock `Warehouse -> Vehicle`.
5. Receive moves stock `Vehicle -> TargetWarehouse`.
6. Movement and task complete; vehicle assignment is released.

### Field Operation

1. Create `InventoryTask` with `type = FieldOperation`, source warehouse and return warehouse.
2. Create movement with `taskId`, `vehicleId` and lines.
3. Workflow approves the movement.
4. Dispatch moves stock `Warehouse -> Vehicle`.
5. Main receive completes the movement, while stock remains on the vehicle.
6. Completing the task creates return request(s) for remaining vehicle stock.
7. Return receive moves good stock back to warehouse and adjusts damaged/lost/consumed quantities.

## Architecture

```text
Domain.Shared          Enums, constants, error codes, events, localization
Domain                 Entities, managers, repository interfaces, event handlers
Application.Contracts  DTOs, service interfaces, validators, permissions
Application            AppServices and AutoMapper profiles
EntityFrameworkCore    DbContext, configurations, repositories, migrations
HttpApi                REST controllers
HttpApi.Host           Startup, middleware, Swagger, OpenIddict, SignalR
```

Business logic belongs in domain managers. AppServices resolve current user/worker, map DTOs, coordinate managers and publish application events.

## Database Schemas

| Schema | Contains |
| --- | --- |
| `abp` | ABP identity, permissions, audit and settings |
| `openiddict` | OAuth2/OIDC tables |
| `lookup` | Department, ProductCategory |
| `master` | Product, Warehouse, Vehicle, Worker |
| `stock` | StockLocation, InventoryTransaction |
| `operation` | InventoryTask, VehicleTask |
| `movement` | MovementRequest, MovementRequestLine, MovementApproval |

Latest route normalization migration:

`src/InventoryTrackingAutomation.EntityFrameworkCore/Migrations/20260430110525_MoveMovementRouteToTask.cs`

## Setup

Create `appsettings.secrets.json` files for local secrets. Do not commit secrets.

Start PostgreSQL:

```bash
docker-compose up -d
```

Run the API host:

```bash
dotnet run --project host/InventoryTrackingAutomation.HttpApi.Host
```

Run the auth server:

```bash
dotnet run --project host/InventoryTrackingAutomation.AuthServer
```

Build and test:

```bash
dotnet build InventoryTrackingAutomation.sln
dotnet test
```

## External Wiki

The operational wiki lives at:

`C:\Users\mertb\OneDrive\Belgeler\InventoryWiki\wiki`

Start with `current-system-state-for-claude.md`.
