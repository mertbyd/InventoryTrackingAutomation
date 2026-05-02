# AGENTS.md

This file provides guidance to Codex when working with this repository.

## Build And Run

```bash
# Build entire solution
dotnet build InventoryTrackingAutomation.sln

# Run the main API host
dotnet run --project host/InventoryTrackingAutomation.HttpApi.Host

# Run the auth server
dotnet run --project host/InventoryTrackingAutomation.AuthServer

# Run all tests
dotnet test

# Run a specific test project
dotnet test test/InventoryTrackingAutomation.Application.Tests

# Angular frontend (inside /angular)
ng serve
ng build
ng test
```

## Database Migrations

Run EF Core migrations from `src/InventoryTrackingAutomation.EntityFrameworkCore/`:

```bash
dotnet ef migrations add MigrationName --startup-project ../../../host/InventoryTrackingAutomation.HttpApi.Host --context InventoryTrackingAutomationDbContext
dotnet ef database update --startup-project ../../../host/InventoryTrackingAutomation.HttpApi.Host --context InventoryTrackingAutomationDbContext
```

Local PostgreSQL is available with:

```bash
docker-compose up -d
```

## Current System Shape

Framework: ABP Framework 10.3.0, .NET 10, PostgreSQL, OpenIddict, SignalR.

The current movement system is entity-chain based:

```text
InventoryTask
  -> TaskLine
  -> VehicleTask
      -> VehicleTaskLine
      -> MovementRequest
          -> InventoryTransaction
```

Core rules:

- `InventoryTask.Type` is the process selector.
- `InventoryTask.SourceWarehouseId` and `InventoryTask.TargetWarehouseId` own the route.
- `TaskLine` owns the task product and requested quantity.
- `VehicleTask` owns task/vehicle/responsible worker assignment.
- `VehicleTaskLine` owns how much of a `TaskLine` is allocated to a vehicle assignment and stores return reconciliation.
- `MovementRequest` stores `VehicleTaskId` and optional `ParentMovementRequestId`.
- `MovementRequest` does not store direct task, route or product line fields.
- Product is resolved through `VehicleTaskLine.TaskLineId -> TaskLine.ProductId`; do not duplicate `ProductId` on `VehicleTaskLine`.
- `InventoryTransaction` links to the ledger source through `RelatedMovementRequestId`.

Do not reintroduce:

- `MovementRequest.Type`
- `MovementRequest.RequestedVehicleId`
- `MovementRequest.AssignedTaskId`
- `MovementRequest.TaskId`
- `MovementRequest.SourceWarehouseId`
- `MovementRequest.TargetWarehouseId`
- `InventoryTransaction.RelatedTaskId`
- `MovementRequestTypeEnum`
- production `MovementRequestLine` entity/DTO/service/controller/repository surface
- `POST /api/movement-requests/with-lines`

## Domain Conventions

- Entity constructors use:

```csharp
protected EntityName() { }
public EntityName(Guid id) : base(id) { }
```

- No navigation properties in domain entities unless an existing workflow entity explicitly uses them.
- Business logic lives in `src/InventoryTrackingAutomation.Domain/Managers/`.
- AppServices are thin orchestration and mapping layers.
- Repository interfaces live under `Domain/Interface/`; EF implementations live under `EntityFrameworkCore/Repository/`.
- Use `IMovementRequestRepository.GetOperationalContextAsync` for movement decisions instead of rebuilding joins in managers or app services.

## Layer Map

```text
Domain.Shared          Enums, constants, error codes, events, localization
Domain                 Entities, managers, repository interfaces, event handlers
Application.Contracts  DTOs, service interfaces, validators, permissions
Application            AppServices and AutoMapper profiles
EntityFrameworkCore    DbContext, configurations, repositories, migrations
HttpApi                Controllers
HttpApi.Host           Startup, middleware, Swagger, OpenIddict, SignalR
```

## Database Schemas

| Schema | Contains |
| --- | --- |
| `abp` | ABP identity, permissions, audit, settings |
| `openiddict` | OAuth2/OIDC tables |
| `lookup` | Department, ProductCategory |
| `master` | Product, Warehouse, Vehicle, Worker |
| `stock` | StockLocation, InventoryTransaction |
| `operation` | InventoryTask, TaskLine, VehicleTask, VehicleTaskLine |
| `movement` | MovementRequest, MovementApproval |

## Movement Lifecycle

Create movement:

1. Client creates/updates `InventoryTask` and its `TaskLine` records.
2. Client creates/updates `VehicleTask` and its `VehicleTaskLine` allocations.
3. Client creates `MovementRequest` with `VehicleTaskId`.
4. Manager validates worker, task route warehouses, active vehicle task and transfer lines.
5. Workflow is selected from `InventoryTask.Type`.

Dispatch:

1. Movement must be `Approved`.
2. Return movements cannot be dispatched.
3. Operational context is loaded through the repository.
4. Transfer lines are read from `VehicleTaskLine`, product is resolved through `TaskLine`.
5. Stock moves `InventoryTask.SourceWarehouseId -> VehicleTask.VehicleId`.
6. Movement becomes `Shipped`.

Receive:

- Warehouse transfer: stock moves `Vehicle -> InventoryTask.TargetWarehouseId`, movement and task complete, vehicle assignment is released.
- Field operation main movement: movement completes, stock remains on vehicle.
- Return movement: received stock moves back to warehouse; damaged/lost/consumed quantities become vehicle stock adjustments.

## Testing Guidance

- Integration tests must create real FK rows; do not use random GUID placeholders.
- Repository/query logic should run inside an active UnitOfWork.
- EFCore tests use SQLite shared in-memory database.
- Keep movement regression coverage around removed fields and route ownership.
- Critical movement tests live in `test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/EntityFrameworkCore/Movements/MovementFlow_Integration_Tests.cs`.
- Run `dotnet test test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/InventoryTrackingAutomation.EntityFrameworkCore.Tests.csproj --no-restore --filter MovementFlow_Integration_Tests`.
- Movement flow tests write step-by-step table logs under `test/InventoryTrackingAutomation.EntityFrameworkCore.Tests/TestResults/movement-flow-logs`.

## External Wiki

The external working wiki is:

`C:\Users\mertb\OneDrive\Belgeler\InventoryWiki\wiki`

Read first:

1. `current-system-state-for-claude.md`
2. `10-entity-first-system-map.md`
3. `02-domain-model.md`
4. `03-workflow-engine.md`
5. `test-plan.md`
