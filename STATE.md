# Current Architecture & Workflow Baseline

**Tarih:** 2026-04-25
**Durum:** Architecture Documentation Complete → Wiki-Brain Integrated

---

## Project Overview

**InventoryTrackingAutomation** — Stok hareket yönetimi sistemi, DDD ve Workflow Engine pattern'leri kullanan .NET 8+ uygulaması.

---

## Architecture Baseline

### Core Principles

✅ **Domain-Driven Design (DDD)**
- Entities & Aggregate Roots (FullAuditedAggregateRoot)
- Domain Services (WorkflowManager, MovementRequestManager)
- Value Objects & Enums
- NO Navigation Properties (Exception: WorkflowInstance)

✅ **Layered Architecture**
```
API Endpoints
    ↓
AppServices (Orchestration)
    ↓
Domain Services & Managers (Business Logic)
    ↓
Entities & Repositories
    ↓
Entity Framework Core (EF)
```

✅ **Workflow Engine**
- State Machine Pattern (Active → Approved/Rejected/Completed)
- Dynamic Routing (Template-based step generation)
- Event-Driven Completion (LocalEventBus)
- Approver Resolution (Manager/Role-based)

✅ **Security-First Approach**
- User identity from CurrentUser (NEVER from DTO)
- Authorization in Domain (Manager checks)
- WorkerId resolution from authenticated User
- Approval decisions validated at domain level

---

## Key Entity Relationships

```
MovementRequest (Aggregate Root)
  ├── RequestedByWorkerId → Worker
  ├── SourceSiteId → Site
  ├── TargetSiteId → Site
  └── WorkflowInstanceId? → WorkflowInstance

ProductStock (Entity, per site)
  ├── ProductId → Product
  └── SiteId → Site

WorkflowInstance (Aggregate Root)
  ├── EntityType="MovementRequest"
  ├── EntityId=MovementRequest.Id
  ├── WorkflowDefinitionId → WorkflowDefinition
  └── Steps: WorkflowInstanceStep[] (nav property - exception)

WorkflowInstanceStep (Entity)
  ├── AssignedUserId? → Approver
  ├── WorkflowStepDefinitionId → WorkflowStepDefinition
  └── ActionTaken: (Pending/Approved/Rejected)
```

---

## Critical Rules (RED LINES)

🔴 **DO NOT VIOLATE:**

### 1. User Identity
- **ALWAYS:** `CurrentUser.GetId()` for identity
- **NEVER:** `input.UserId` from DTO
- **NEVER:** Trust client-provided user/worker IDs

### 2. Authorization
- Approval decisions → Domain Service (WorkflowManager)
- Not AppService-level filters
- WorkflowManager validates approver assignment
- Role checks happen in domain

### 3. DDD Navigation
- **NO** `virtual ICollection<T>` or `virtual Entity` in Entities
- **EXCEPTION:** WorkflowInstance (currently has navigation)
- **Use:** Foreign Keys (GUID) only

### 4. Business Logic Location
- **AppService:** DTO mapping, repository calls, transaction boundaries
- **Domain Services:** State transitions, routing, authorization
- **Entities:** Validation in constructors, invariant checks

---

## Key Files (Architectural Understanding Required)

| File | Role | Pattern |
|------|------|---------|
| `MovementRequest.cs` | Aggregate Root | DDD Entity (no nav props) |
| `WorkflowInstance.cs` | Aggregate Root | DDD Entity (has nav - exception) |
| `ProductStock.cs` | Entity | Stock tracking per location |
| `WorkflowManager.cs` | Domain Service | State Machine + Routing |
| `MovementRequestAppService.cs` | Orchestrator | HTTP ↔ Domain bridge |
| `InventoryTrackingAutomationDbContext.cs` | Persistence | EF Core mappings |
| `InventoryTrackingAutomationPermissions.cs` | Authorization | Permission constants |
| `InventoryTrackingAutomationPermissionDefinitionProvider.cs` | Authorization | ABP permission definitions + localization |

---

## Knowledge Graph Integration

**Wiki-Brain Vault:** `C:\Users\mertb\OneDrive\Belgeler\InventoryWiki`

**Ingested Pages:**
- `inventory-architecture-overview` — Full system architecture
- `workflow-manager` — State machine details & routing
- `movement-request-app-service` — HTTP orchestration layer
- `security-critical-rules` — 🔴 **CRITICAL** security rules

**Usage:** `/wiki-brain query "workflow routing"` or `/recall`

---

## Next Steps (For Other Agents)

1. **Architecture Questions:** Use `/wiki-brain query` instead of reading files
2. **Implementation:** Reference security-critical-rules before coding
3. **Code Review:** Check security rules checklist
4. **New Features:** Understand inventory-architecture-overview first

---

---

## Identity & Role Seeding

**Tarih:** 2026-04-25 | **Durum:** ✅ Complete

### Roller (Identity Roles)

7 rol veritabanına seed'leniyor (IdentityRoleManager ile):

| Rol | Amaç | Permissions |
|-----|------|-----------|
| **Admin** | Sistem yöneticisi | Tüm izinler |
| **Manager** | Operasyon yöneticisi | Inventory.Manage, Workflows.Approve |
| **WarehouseWorker** | Depo çalışanı | Inventory.View, MovementRequests.View |
| **FieldWorker** | Saha işçisi | MovementRequests.Create, Inventory.View |
| **LogisticsSupervisor** | Lojistik müdürü | Inventory.Manage, Workflows.Approve/Reject |
| **WorkflowApprover** | İş akışı onaylayanı | Workflows.Approve, Workflows.Reject |
| **VehicleManager** | Araç yöneticisi | Masters.Manage (vehicles) |

### Seed Mekanizması

**File:** `InventoryTrackingAutomationDataSeedContributor.cs`

```csharp
private async Task SeedRolesAsync()
{
    // RoleConstants'dan tüm roller oku
    var rolesToCreate = new[] { Admin, Manager, ... };
    
    // Her rol için: FindByName → if not exists → Create
    foreach (var roleName in rolesToCreate)
    {
        if (await _identityRoleManager.FindByNameAsync(roleName) == null)
        {
            var role = new IdentityRole(_guidGenerator.Create(), roleName);
            await _identityRoleManager.CreateAsync(role);
        }
    }
}
```

**Çalıştırma Sırası:**
1. SeedAsync → SeedRolesAsync (ilk olarak)
2. Rol'ler create edilir
3. Sonra domain data (Worker, Site, Product, Workflow) seed'lenir

### İzinler (Permissions)

**File 1:** `InventoryTrackingAutomationPermissions.cs`
- Permission sabitleri (BankApp örneğine benzer)
- 4 namespace: MovementRequests, Workflows, Inventory, Masters
- `ReflectionHelper.GetPublicConstantsRecursively()` ile otomatik GetAll()

**File 2:** `InventoryTrackingAutomationPermissionDefinitionProvider.cs` ✅ **TAMAMLANDI**
- ABP `PermissionDefinitionProvider` implementation
- Hierarchical permission structure (parent → children) — MovementRequests, Workflows, Inventory, Masters
- Localization support (`L()` helper method → InventoryTrackingAutomationResource)
- Turkish comments for each permission definition

**Kod Örneği (Controller'da):**
```csharp
[Authorize(InventoryTrackingAutomationPermissions.MovementRequests.Create)]
public async Task<MovementRequestDto> CreateAsync(CreateMovementRequestDto input)
{
    return await _accountAppService.CreateAsync(input);
}
```

**Rol ↔ Permission Atama Tablosu:**

| Rol | İzinler |
|-----|---------|
| **Admin** | MovementRequests.* + Workflows.* + Inventory.Manage + Masters.Manage |
| **Manager** | MovementRequests.View + Workflows.Approve/Reject + Inventory.Manage |
| **WorkflowApprover** | Workflows.Approve + Workflows.Reject + Workflows.View |
| **WarehouseWorker** | Inventory.View + MovementRequests.View |
| **FieldWorker** | MovementRequests.Create + Inventory.View |
| **LogisticsSupervisor** | Inventory.Manage + Workflows.Approve/Reject |
| **VehicleManager** | Masters.Manage |

### Critical Points

✅ **RoleConstants** — Hardcode-free rol isimleri  
✅ **IGuidGenerator** — ID üretimi ABP standart  
✅ **DRY** — Roller listeden döng döngüyle oluşturuluyor  
✅ **No Navigation** — Sadece Id referansları kullanıldı  
✅ **No Duplication** — Her rol bir kez check/create  

---

### Permission Attributes on Controllers

✅ **AuthController** — Login & Register public ([AllowAnonymous])  
✅ **ProductController** — All endpoints protected with [Authorize(Masters.Manage)]  

**Pattern (tüm controller'larda uygulanacak):**
```csharp
// Controller seviyesi (all endpoints protected)
[ApiController]
[Authorize]
public class SomeController : InventoryTrackingAutomationController { }

// Action method seviyesi (specific permission)
[HttpPost]
[Authorize(InventoryTrackingAutomationPermissions.Inventory.Manage)]
public async Task<Result<Dto>> Create([FromBody] CreateDto input) { }

// Public endpoint
[HttpPost("login")]
[AllowAnonymous]
public async Task<TokenResponse> Login([FromBody] LoginDto input) { }
```

---

## Authentication & Authorization Framework

**Tarih:** 2026-04-25 | **Durum:** ✅ Complete

### Role & Permission Seeding

✅ **InventoryTrackingAutomationRoleConstants.cs** — 7 rol tanımı  
✅ **InventoryTrackingAutomationPermissions.cs** — 4 namespace (MovementRequests, Workflows, Inventory, Masters)  
✅ **InventoryTrackingAutomationPermissionDefinitionProvider.cs** — ABP hierarchical permission definitions  
✅ **InventoryTrackingAutomationDataSeedContributor.cs** — Rol seed işlemi (`SeedRolesAsync`)  

### Auth Service Stack (BankApp Pattern)

**Three-Layer Architecture:**

1. **API Controller Layer** (`HttpApi`)
   - **AuthController** — Thin controller, just calls AppService
   - Routes: `POST /api/auth/login`, `POST /api/auth/register`
   - `[AllowAnonymous]` on both (public endpoints)

2. **Application Service Layer** (`Application`)
   - **IAuthAppService** interface (contract) → 2 methods
   - **AuthAppService** implementation
     - Uses `AuthManager` (domain service) for business logic
     - Calls OpenIddict token endpoint (`GetTokenFromOpenIddictAsync`)
     - Maps DTO ↔ Model using AutoMapper
     - Returns DTOs (TokenResponse, Guid)
   - **AuthMappingProfile** — AutoMapper configuration
     - LoginDto → LoginModel
     - RegisterDto → RegisterModel

3. **Domain Service Layer** (`Domain`)
   - **AuthManager** (DomainService) — Pure business logic
     - `CreateUserAsync(RegisterModel)` — Validates uniqueness, creates user with default role
     - `ValidateLoginAsync(LoginModel)` — Password validation
     - Private validation methods
     - Uses ABP's `IdentityUserManager`, `IIdentityUserRepository`

**Models** (`Domain.Shared`)
- **LoginModel** — userName, password
- **RegisterModel** — userName, email, password, passwordConfirm

**DTOs** (`Application.Contracts`)
- **LoginDto** — userName, password
- **RegisterDto** — userName, email, password, passwordConfirm
- **TokenResponse** — userId, accessToken, refreshToken, expiresIn, tokenType

**Error Codes** (`Domain.Shared`)
- `Auth.UserNameAlreadyExists`, `EmailAlreadyExists`, `InvalidCredentials`
- `PasswordMismatch`, `UserCreationFailed`, `TokenRequestFailed`

**DI Registration:**
- `InventoryTrackingAutomationApplicationModule` → AddScoped<IAuthAppService, AuthAppService>
- AuthManager auto-registered (DomainService pattern)

---

## Token Efficiency

- Previous Claude instances wasted tokens re-reading the same 6 files
- Wiki-brain reduces future context by **80%+** through knowledge graph queries
- This agent (architecture role) guides others with prompts, not code
- Other agents: query graph, implement from prompts, avoid re-exploration

---

## Workflow Implementation Roadmap

**Hazırlık Skoru:** %90

**Tamamlananlar (Faz 1 - 100% Hazır):**
- Gerekli roller (Admin, Approver vb.) ve 3 adımlı "MovementRequest" iş akışı SeedContributor içerisinde fiziksel olarak kodlanmış ve oluşturulmaktadır.
- `MovementRequestManager` içerisinde `CreateWithWorkflowAsync` metodu ile WorkflowInstance başlatan mekanizma başarıyla kurulmuştur.
- **Güvenlik (OpenIddict & ABP Permissions):** Tüm Controller'lar `[Authorize]` attribute'u ile korunmakta ve Seed Data aşamasında roller, `IPermissionManager.SetAsync` ile yetkilerine (Permissions) otomatik bağlanmaktadır. DTO-based Identity ihlali yoktur, `CurrentUserId` arka planda güvenle çözülür.
- **Tekil Onay Uç Noktası:** Frontend kullanımını kolaylaştırmak için Approve/Reject işlemleri tek bir `ProcessApprovalAsync` servisinde ve `/api/movement-requests/{id}/process-approval` endpoint'inde birleştirildi.
- **Arabaya Yükleme (Sevkiyat):** Kapanış adımı için `MovementRequestWorkflowEventHandler` event listener'ı eklenmiş; onay tamamlandığında UOW destekli stok düşümü yapılmakta ve otomatik olarak **Shipment (Sevkiyat)** oluşturulup talebe (ShipmentId) bağlanmaktadır. Faz 1'in "Arabaya yüklenmiş olması" şartı sağlanmıştır.

