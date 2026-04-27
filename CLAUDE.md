# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

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

## Database Migrations (EF Core)

Run these from `src/InventoryTrackingAutomation.EntityFrameworkCore/`:

```bash
dotnet ef migrations add "MigrationName" --startup-project ../../../host/InventoryTrackingAutomation.HttpApi.Host
dotnet ef database update --startup-project ../../../host/InventoryTrackingAutomation.HttpApi.Host
```

Local PostgreSQL is available via Docker:

```bash
docker-compose up -d
```

## Architecture Overview

**Framework:** ABP Framework 10.3.0, .NET 10 (`net10.0`), PostgreSQL (Npgsql 10.0.0), AutoMapper 12.0.1

All projects target `net10.0` (Domain.Shared dahil — eski `netstandard2.1` hedefi kaldırıldı). SDK pin: `global.json` → `10.0.203` (`rollForward: latestFeature`).

### Layer Hierarchy

```
Domain.Shared       → Enums, error codes, localization (net10.0)
Domain              → Entities, Manager classes, repository interfaces, EventHandlers
Application.Contracts → DTOs, IAppService interfaces
Application         → AppService implementations, AutoMapper profiles
EntityFrameworkCore → DbContext, FluentAPI configs, custom repositories, Migrations
HttpApi             → REST controller module configuration
HttpApi.Host        → Entry point (Serilog, Autofac, Swagger, OpenIddict)
AuthServer          → Standalone OpenIddict authorization server
Blazor / Blazor.Server / Blazor.WebAssembly  → Blazor UI varyantları (opsiyonel)
Web / Web.Host / Web.Unified                 → MVC tabanlı Web UI (opsiyonel)
MongoDB / Installer                          → Alternatif provider + setup tooling
```

**SystemStandards bağımlılığı:** `..\..\..\SystemStandards\src\` altından `ProjectReference` olarak eklenmiş (Core, Validation, AspNetCore, Abp). NuGet paketine dönüştürme planı için bkz. wiki `plan-cicd-enterprise-upgrade.md` Faz 2.

### Domain Layer Conventions

**Entity base classes:** `FullAuditedAggregateRoot<Guid>` for aggregate roots, `Entity<Guid>` for child entities. All implement `IMultiTenant` with `public Guid? TenantId { get; set; }`.

**Constructor pattern** (required for every entity):
```csharp
protected EntityName() { }
public EntityName(Guid id) : base(id) { }
```

**No navigation properties** — only `XxxId` FK references. Collections are not included.

**Business logic** lives in `Domain/Managers/` (e.g., `DepartmentManager`, `ProductManager`, `WorkflowManager`, `MovementApprovalManager`). AppServices delegate to managers; they do not contain domain logic themselves.

**BaseManager helpers** (paylaşılan): `EnsureExistsAsync(id)`, `EnsureExistsInAsync(otherRepo, id)`, `EnsureUniqueAsync(predicate)`, `EnsureValidEnumAsync(value, settingName)`. Custom error code parametresi yok — otomatik `EntityNotFoundException` ve `InventoryTrackingAutomation:{TEntity}.AlreadyExists` üretir.

**Mapping:** Manager ve AppService'ler ctor'a `IMapper` (AutoMapper) inject eder. Kalıp: `var e = new X(GuidGenerator.Create()); _mapper.Map(model, e);` — `IObjectMapper`/`MapAndAssignId`/`MapForUpdate` artık kullanılmıyor.

**Workflow approver çözümleme:** Strategy pattern. `IApproverStrategy` impl'leri (`InitiatorManagerApproverStrategy`, `SourceSiteManagerApproverStrategy`, `TargetSiteManagerApproverStrategy`) `DefaultWorkflowApproverResolver` içinde DI'dan toplanır, `ResolverKey`'e göre dispatch edilir. Yeni resolver eklemek için sadece yeni `IApproverStrategy` impl'i yeterli (OCP).

**Repository interfaces** are declared in `Domain/Interface/` and implemented in `EntityFrameworkCore/Repository/`. Register custom repos in `InventoryTrackingAutomationEntityFrameworkCoreModule.ConfigureServices`.

### Application Layer Conventions

- AppServices inherit `InventoryTrackingAutomationAppService`
- Standard CRUD methods: `GetAsync(id)`, `GetListAsync(PagedResultRequestDto)`, `CreateAsync(dto)`, `CreateManyAsync(List<dto>)`, `UpdateAsync(id, dto)`, `DeleteAsync(id)`
- DTOs follow naming: `Create{Entity}Dto`, `Update{Entity}Dto`; responses use `{Entity}Dto`
- AutoMapper profiles are auto-discovered from the module assembly (no manual registration needed)
- **Controller pattern:** Class-level `[Authorize]`, endpoint-level permission attribute (örn. `[Authorize(InventoryTrackingAutomationPermissions.MovementRequests.View)]`). Auth endpoint'leri (`login`/`register`) `[AllowAnonymous]`.
- **Return type:** `Task<Result<T>>` veya `Task<Result>` (SystemStandards.Results paketi). Ham DTO döndürülmez.

### Database Schemas

The DbContext partitions tables by functional area:

| Schema | Contains |
|--------|----------|
| `abp` | ABP framework tables (identity, permissions, audit, settings) |
| `openiddict` | OAuth2/OIDC tables |
| `lookup` | Department, ProductCategory |
| `master` | Product, Site, Vehicle, Worker |
| `stock` | ProductStock, StockMovement |
| `movement` | MovementRequest, MovementRequestLine, MovementApproval |
| `shipment` | Shipment, ShipmentLine |
| `public` (default) | WorkflowDefinition, WorkflowStepDefinition, WorkflowInstance, WorkflowInstanceStep — **dikkat:** custom schema atanmamış, varsayılan `public` schema'da kalıyor |

### Error Codes

Centralized in `Domain.Shared/InventoryTrackingAutomationDomainErrorCodes.cs`. Pattern:

```csharp
public const string ProductNotFound = "InventoryTracking:Products.NotFound";
public const string ProductCodeNotUnique = "InventoryTracking:Products.CodeNotUnique";
```

Managers expose `EnsureExistsAsync(id, errorCode)` and `EnsureUniqueAsync(predicate, errorCode)` helpers.

### Module Registration

ABP modules use `[DependsOn(...)]` and `ConfigureServices`/`OnApplicationInitialization`. When adding a new feature:
1. Define entity in Domain, add to DbContext
2. Add DbSet and FluentAPI config in `InventoryTrackingAutomationDbContext`
3. Register custom repository in `InventoryTrackingAutomationEntityFrameworkCoreModule`
4. Create manager in Domain, DTO + interface in Application.Contracts, service in Application
5. Add error codes to `InventoryTrackingAutomationDomainErrorCodes`

## Key Conventions

- **Language version:** `latest`, **Nullable:** `enabled` — enforce non-nullable by default
- **Comments:** XML `<summary>` on all classes/enums; `//` inline comments on properties with example values. Domain comments are written in Turkish.
- **File-scoped namespaces:** `namespace InventoryTrackingAutomation.X.Y;`
- **Shared build props:** All `.csproj` files import `common.props` — do not duplicate version or language settings per-project
- **Secrets:** Connection strings go in `appsettings.secrets.json` (gitignored), not `appsettings.json`

---

## Geçmiş Sürümler & Değişiklikler

### v1.1.0 (2026-04-25)
- **Serialization Fix**: `System.Type` kaynaklı crash sorunu `SystemStandards` kütüphanesinde `[JsonIgnore]` ile çözüldü.
- **Logging Middleware**: Tüm request/response döngüsünü loglayan `RequestResponseLoggingMiddleware` pipeline'a eklendi.
- **Fluent API**: Result sınıflarına profesyonel zincirlenebilir metodlar (`WithCorrelationId`, `WithLocation`) eklendi.
- **Dinamik Mapping**: `appsettings.json` üzerinden mapping kuralları ve DI kayıtları `SystemStandards` tarafında optimize edildi.
- **Modern Altyapı**: Tüm projeler .NET 10 ve ABP 10.3.0 standartlarına yükseltildi.
- **Sürüm Güncellemesi**: `common.props` ve ilgili paket referansları `v1.1.0` olarak güncellendi.

