# Proje Refactoring Durumu

## Son Tamamlanan: Faz 5.17 — AutoMapper Validation: MovementRequest AggregateRoot ExtraProperties+ConcurrencyStamp

### Tamamlanan Fazlar

| Faz | Açıklama | Durum |
|-----|----------|-------|
| 5.2 | DTO Temizliği — TenantId, EmployeeId ve Data Annotation'lar silindi | ✅ |
| 5.4 | Mapping Uyumsuzlukları — LoadedQuantity→Quantity, RequestedBy→RequestedByWorkerId | ✅ |
| 5.5 | IMapper Enjeksiyonu — ObjectMapper tamamen kaldırıldı, AutoMapper.IMapper eklendi | ✅ |
| 5.7 | IMapper → IObjectMapper — AutoMapper.IMapper kaldırıldı, ABP IObjectMapper iade edildi | ✅ |
| 5.8 | AutoMapper DI Hatası — WorkflowMappingProfile eksik audit yoksayma kuralları eklendi | ✅ |
| 5.9 | AutoMapper Validation Temizliği — eşleşmeyen DTO/Model alanları açıkça Ignore edildi | ✅ |
| 5.10 | AutoMapper Validation: ProductStock — Create/Update mapping'lerinde Id Ignore edildi | ✅ |
| 5.11 | AutoMapper Validation: StockMovement — Create/Update mapping'lerinde Id Ignore edildi | ✅ |
| 5.12 | AutoMapper Validation: ShipmentLine — audit+Id Ignore eklendi, DamageNote Ignore eklendi | ✅ |
| 5.13 | AutoMapper Validation: Shipment — audit+Id+DepartureTime+DeliveryTime Ignore eklendi | ✅ |
| 5.14 | AutoMapper Validation: Masters+Lookups — 6 profilde Id Ignore eklendi | ✅ |
| 5.15 | AutoMapper Validation: MovementRequest+Line — Id+audit Ignore, Status/WorkerId/ShipmentId Ignore | ✅ |
| 5.16 | AutoMapper Validation: Statik analiz — CancellationNote/WorkflowInstanceId/MinimumStockLevel/Site alan adı uyumsuzlukları | ✅ |
| 5.17 | AutoMapper Validation: MovementRequest FullAuditedAggregateRoot — ExtraProperties+ConcurrencyStamp Ignore eklendi | ✅ |

### Faz 5.7 Yapılan Değişiklikler

**BaseManager:**
- `AutoMapper.IMapper` → `Volo.Abp.ObjectMapping.IObjectMapper` constructor injection
- `MapAndAssignId` ve `MapForUpdate` helper'ları `IObjectMapper` ile çalışacak şekilde güncellendi

**Managers (AutoMapper.IMapper → IObjectMapper):**
- DepartmentManager, ProductCategoryManager
- ProductManager, SiteManager, VehicleManager, WorkerManager
- MovementRequestManager (+ gereksiz `_mapper` field'ı silindi)
- MovementRequestLineManager
- ShipmentManager, ShipmentLineManager
- ProductStockManager, StockMovementManager

**AppServices (IMapper → ObjectMapper property):**
- DepartmentAppService, ProductCategoryAppService
- ProductAppService, SiteAppService, VehicleAppService, WorkerAppService
- ProductStockAppService, StockMovementAppService
- ShipmentAppService, ShipmentLineAppService
- MovementRequestAppService, MovementRequestLineAppService
- WorkflowAppService

**Mapping formatı (AppServices):**
- `_mapper.Map<TDest>(source)` → `ObjectMapper.Map<TSource, TDest>(source)`
- Constructor'dan `IMapper mapper` parametresi ve `private readonly IMapper _mapper` field'ı tamamen kaldırıldı

**EF Core Migration:**
- Baseline migration oluşturuldu (boş Up/Down), DB'ye uygulandı

### Mevcut Durum
- Build: ✅ Başarılı (Faz 5.5 sonrası, Faz 5.7 değişiklikleri aynı derleme durumunu koruyor)
- AutoMapper.IMapper kullanımı (AppService/Manager): ✅ Sıfır
- ABP IObjectMapper kullanımı: ✅ Managers ve BaseManager'da doğru enjeksiyon
- ABP ObjectMapper property kullanımı: ✅ Tüm AppService'lerde iki-parametre formatı
- IMultiTenant / TenantId: ✅ Sıfır (entity, DTO, model)
- EF Migration baseline: ✅ Uygulandı

### Faz 5.16 Yapılan Değişiklikler (Statik Analiz)

Tüm entity, DTO ve Model dosyaları okunarak runtime'a bırakmadan tespit edilen sorunlar:

**MovementRequestMappingProfile** (`ObjectMapping/Movements/`):
- `CreateMovementRequestDto → MovementRequest`: `CancellationNote`, `WorkflowInstanceId` Ignore eklendi (entity'de var, DTO'da yok)
- `UpdateMovementRequestDto → MovementRequest`: aynısı
- `CreateMovementRequestModel → MovementRequest`: `CancellationNote`, `WorkflowInstanceId` Ignore eklendi (Model'de bu alanlar yok)
- `UpdateMovementRequestModel → MovementRequest`: aynısı

**ProductMappingProfile** (`ObjectMapping/Masters/`):
- `CreateProductDto → CreateProductModel`: `MinimumStockLevel` Ignore eklendi (Model'de var, DTO'da yok — AppService'de çözümlenir)
- `UpdateProductDto → UpdateProductModel`: aynısı

**SiteMappingProfile** (`ObjectMapping/Masters/`):
- `CreateSiteDto → CreateSiteModel`: `LinkedVehicleId`, `LinkedWorkerId` Ignore eklendi (Model bu adlarla, entity `ManagerWorkerId` adıyla tutar)
- `UpdateSiteDto → UpdateSiteModel`: aynısı
- `CreateSiteModel → Site`: `ManagerWorkerId` Ignore eklendi (Model'de karşılık yok, entity'deki bu alan ayrıca AppService'de set edilir)
- `UpdateSiteModel → Site`: aynısı

### Faz 5.13–5.15 Yapılan Değişiklikler

**ShipmentMappingProfile** (`ObjectMapping/Shipments/ShipmentMappingProfile.cs`) — Faz 5.13:
- `CreateShipmentDto → Shipment`: `Id`, `DepartureTime`, `DeliveryTime` Ignore eklendi
- `UpdateShipmentDto → Shipment`: aynısı
- `CreateShipmentDto → CreateShipmentModel`: `DepartureTime`, `DeliveryTime` Ignore eklendi
- `CreateShipmentModel → Shipment`: `Id` Ignore eklendi
- `UpdateShipmentDto → UpdateShipmentModel`: `DepartureTime`, `DeliveryTime` Ignore eklendi
- `UpdateShipmentModel → Shipment`: `Id` Ignore eklendi

**Masters + Lookups** — Faz 5.14 (6 profil):
- `ProductMappingProfile`, `SiteMappingProfile`, `VehicleMappingProfile`, `WorkerMappingProfile`
- `ProductCategoryMappingProfile`, `DepartmentMappingProfile`
- Tüm `→ Entity` mapping'lerine `ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi (4 mapping × 6 profil)

**MovementRequestMappingProfile** (`ObjectMapping/Movements/`) — Faz 5.15:
- `CreateMovementRequestDto → MovementRequest`: `Id` Ignore eklendi
- `UpdateMovementRequestDto → MovementRequest`: `Id` Ignore eklendi
- `CreateMovementRequestModel → MovementRequest`: `Id` Ignore eklendi
- `UpdateMovementRequestModel → MovementRequest`: `Id` Ignore eklendi
- `CreateMovementRequestDto → CreateMovementRequestModel`: `RequestedByWorkerId`, `ShipmentId`, `Status` Ignore eklendi
- `UpdateMovementRequestDto → UpdateMovementRequestModel`: aynısı

**MovementRequestLineMappingProfile** (`ObjectMapping/Movements/`) — Faz 5.15:
- `MovementRequestLine : FullAuditedEntity<Guid>` olduğu halde yanlış yorumla `IgnoreFullAuditedObjectProperties()` hiç eklenmemişti
- Tüm 4 `→ MovementRequestLine` mapping'ine `IgnoreFullAuditedObjectProperties()` + `ForMember(Id, Ignore())` eklendi
- `using Volo.Abp.AutoMapper;` eklendi

### Faz 5.12 Yapılan Değişiklikler

**ShipmentLineMappingProfile** (`src/InventoryTrackingAutomation.Application/ObjectMapping/Shipments/ShipmentLineMappingProfile.cs`):
- `CreateShipmentLineDto → ShipmentLine`: `.IgnoreFullAuditedObjectProperties()` ve `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `UpdateShipmentLineDto → ShipmentLine`: `.IgnoreFullAuditedObjectProperties()` ve `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `CreateShipmentLineModel → ShipmentLine`: `.IgnoreFullAuditedObjectProperties()` ve `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `UpdateShipmentLineModel → ShipmentLine`: `.IgnoreFullAuditedObjectProperties()` ve `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `CreateShipmentLineDto → CreateShipmentLineModel`: `.ForMember(dest => dest.DamageNote, opt => opt.Ignore())` eklendi (Model'de var, DTO'da yok)
- `UpdateShipmentLineDto → UpdateShipmentLineModel`: `.ForMember(dest => dest.DamageNote, opt => opt.Ignore())` eklendi (Model'de var, DTO'da yok)
- Yanlış yorum `// Child entity — IgnoreFullAuditedObjectProperties uygulanmaz` silindi; `ShipmentLine : FullAuditedEntity<Guid>` dolayısıyla uygulanması zorunlu
- `using Volo.Abp.AutoMapper;` eklendi

### Faz 5.10 Yapılan Değişiklikler

**ProductStockMappingProfile** (`src/InventoryTrackingAutomation.Application/ObjectMapping/Stock/ProductStockMappingProfile.cs`):
- `CreateProductStockDto → ProductStock`: `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `UpdateProductStockDto → ProductStock`: `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `CreateProductStockModel → ProductStock`: `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `UpdateProductStockModel → ProductStock`: `.ForMember(dest => dest.Id, opt => opt.Ignore())` eklendi
- `IgnoreFullAuditedObjectProperties()` zaten mevcuttu, korundu

### Faz 5.9 Yapılan Değişiklikler

**WorkflowMappingProfile** (`src/InventoryTrackingAutomation.Application/Workflows/WorkflowMappingProfile.cs`):
- `CreateWorkflowDefinitionDto → WorkflowDefinition`: `Id`, `ConcurrencyStamp`, `ExtraProperties`, `Description` alanları Ignore edildi
- `CreateWorkflowStepDefinitionDto → WorkflowStepDefinition`: `Id`, `WorkflowDefinitionId`, `ResolverKey`, `WorkflowDefinition` alanları Ignore edildi
- `StartWorkflowDto → StartWorkflowModel`: `InitiatorUserId`, `InitiatorsManagerUserId` Ignore edildi (AppService'de CurrentUser context'inden doldurulacak)
- `ProcessApprovalDto → ProcessApprovalModel`: `CurrentUserId`, `CurrentUserRoles` Ignore edildi (AppService'de CurrentUser context'inden doldurulacak)

### Faz 5.8 Yapılan Değişiklikler

**WorkflowMappingProfile** (`src/InventoryTrackingAutomation.Application/Workflows/WorkflowMappingProfile.cs`):
- `CreateWorkflowDefinitionDto → WorkflowDefinition` mapping'ine `.IgnoreFullAuditedObjectProperties()` eklendi (`FullAuditedAggregateRoot<Guid>` miras aldığı için)
- `CreateWorkflowStepDefinitionDto → WorkflowStepDefinition` mapping'ine `.IgnoreAuditedObjectProperties()` eklendi (`AuditedEntity<Guid>` miras aldığı için)
- Kök neden: `validate: true` ile AutoMapper, public-settable audit alanlarını (`CreationTime`, `CreatorId`, vb.) eşleştirilmemiş destination üye olarak görüyor ve `MapperAccessor` DI zincirini çöktürüyordu.

### Mapping Profilleri (Değişmedi)
`*MappingProfile.cs` dosyaları `AutoMapper.Profile`'dan türemekte, bu doğru — sadece `IMapper` *injection* kaldırıldı.
