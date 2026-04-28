# InventoryTrackingAutomation

Saha ve depo operasyonları için geliştirilmiş kurumsal envanter yönetim sistemi. Stok hareketlerini, çok adımlı onay süreçlerini ve araç/görev bazlı stok takibini tek bir platformda yönetir.

**Stack:** .NET 10 · ABP Framework 10.3.0 · PostgreSQL 17 · Angular 15 · SignalR · OpenIddict

---

## Veri Modeli

![Entity Diagram](docs/entity-diagram.png)

### Master / Lookup Tabloları

| Tablo | İşlev |
|-------|-------|
| **Product** | Stok kalemi tanımları. Her ürün bir kategoriye, birim tipine ve seri numaralanabilirlik bilgisine sahiptir. |
| **ProductCategory** | Ürün kategori hiyerarşisi. `ParentId` ile çok seviyeli kategori yapısı desteklenir. |
| **Warehouse** | Depo lokasyonları. Her deponun bir sorumlu yöneticisi (`ManagerWorkerId`) vardır. |
| **Vehicle** | Araç kaydı. Tip bilgisi ve aktiflik durumu tutulur; stok taşıyıcı olarak hareket akışına dahil olur. |
| **Worker** | Personel kaydı. ABP Identity kullanıcısına (`UserId`) bağlıdır; rol, departman, varsayılan depo ve yönetici ilişkilerini taşır. |
| **Department** | Organizasyon birimi. Worker'a atanır. |

### Stok Tabloları

| Tablo | İşlev |
|-------|-------|
| **StockLocation** | Anlık stok bakiyesi. Her kayıt bir ürünün belirli bir lokasyondaki (depo veya araç) mevcut ve rezerve miktarını tutar. Doğrudan güncellenmez; yalnızca `InventoryTransaction` üzerinden değişir. |
| **InventoryTransaction** | Değiştirilemez stok defteri. Her stok hareketi (depodan araca, araçtan depoya, düzeltme) burada bir satır olarak kayıt altına alınır. Kaynak ve hedef lokasyon tipi + ID çifti ile tam izlenebilirlik sağlanır. |

### Hareket Tabloları

| Tablo | İşlev |
|-------|-------|
| **MovementRequest** | Stok transfer talebi. Üç tipi vardır: `WarehouseToWarehouse`, `WarehouseToTask`, `TaskReturnToWarehouse`. Kendi yaşam döngüsü (`Pending → Approved → Shipped → Completed`) ve bağlı bir workflow instance'ı vardır. |
| **MovementRequestLine** | Talep kalemleri. Her ürün için istenilen miktar, teslim alınan/hasarlı/kayıp/tüketilen miktarlar ayrı ayrı tutulur. İade akışında kalem bazlı kalite doğrulaması buradan yapılır. |
| **MovementApproval** | Onay adımı kayıtları. Hangi onaylayıcının hangi adımda ne zaman ne kararı verdiğini saklar. |

### Operasyon Tabloları

| Tablo | İşlev |
|-------|-------|
| **InventoryTask** | Saha görevi. Başlangıç/bitiş tarihi, bölge ve iade deposu bilgisini taşır. Tamamlandığında veya iptal edildiğinde otomatik olarak `TaskReturnToWarehouse` talebi oluşturulur. |
| **VehicleTask** | Göreve araç ve sürücü ataması. Bir göreve birden fazla araç atanabilir. |

---

## Dinamik Workflow Motoru

![Workflow Diagram](docs/workflow-diagram.png)

Sistemin en kritik bileşeni: **kod değişikliği gerektirmeden yeni onay akışı tanımlanabilen** dinamik bir workflow motorudur.

### Nasıl Çalışır?

```
WorkflowDefinition          →  "Bu tip talep için N adımlı onay gerekir"
  └── WorkflowStepDefinition  →  "Adım 2'yi kim onaylayacak?" (ResolverKey)
        ↓  tetiklenince
WorkflowInstance            →  Belirli bir talebin aktif onay süreci
  └── WorkflowInstanceStep    →  Adım bazında kimin, ne zaman, ne kararı verdiği
```

### WorkflowDefinition & WorkflowStepDefinition

Onay akışının şablonu. Veritabanında saklandığı için **deploy olmadan** değiştirilebilir.

- `Version` ile birden fazla aktif versiyon yan yana çalışabilir.
- Her adım (`WorkflowStepDefinition`) bir `ResolverKey` taşır. Bu key, o adımı kimin onaylayacağını belirleyen **Strategy**'yi işaret eder.

### Strategy Pattern ile Onaylayıcı Çözümleme

`ResolverKey` değerine göre runtime'da doğru onaylayıcı bulunur:

| ResolverKey | Kim onaylar |
|-------------|-------------|
| `InitiatorManager` | Talebi açan kişinin yöneticisi |
| `SourceWarehouseManager` | Kaynak depo sorumlusu |
| `TargetWarehouseManager` | Hedef depo sorumlusu |
| `LogisticsManager` | Lojistik yöneticisi |

Yeni bir onaylayıcı tipi eklemek için **sadece yeni bir `IApproverStrategy` implementasyonu** yazmak yeterlidir. Mevcut koda dokunulmaz.

### WorkflowInstance & WorkflowInstanceStep

Bir talep onaya girdiğinde şablon'dan türetilen runtime kaydı oluşur:

- `EntityType` + `EntityId` ile workflow hangi talebe bağlı olduğunu bilir — aynı motor farklı entity tipleri için kullanılabilir.
- `State`: `Pending`, `InProgress`, `Approved`, `Rejected`
- Her adım tamamlandığında `WorkflowActionType` (Approve/Reject) ve karar tarihi kayıt altına alınır.
- Tüm adımlar onaylandığında `WorkflowInstance.State = Approved` olur ve talep `MovementRequest.Status = Approved`'a geçer. **Stok bu noktada hareket etmez.**

### Stok Ne Zaman Hareket Eder?

```
Approved → [Dispatch endpoint] → Shipped    : Stok kaynaktan araca geçer
Shipped  → [Receive endpoint]  → Completed  : Stok araçtan hedefe geçer
```

Onay ve stok transferi birbirinden tam anlamıyla ayrılmıştır. Onay iş akışını yönetir; fiziksel teslimat ayrı bir adımdır.

---

## Hareket Tipleri ve Tam Akış

### 1. WarehouseToWarehouse — Depolar Arası Transfer

```
1. Talep oluşturulur (Pending)
2. Workflow başlar → onaylayıcılar sırayla bildirim alır (InReview)
3a. Reddedilirse → Rejected (stok hiç hareket etmez)
3b. Onaylanırsa → Approved (stok hâlâ hareket etmez)
4. Sevkiyat başlatılır [Dispatch] → stok kaynak depodan araca geçer (Shipped)
5. Araç hedefe ulaşır, teslim alınır [Receive] → stok araçtan hedef depoya geçer (Completed)
```

### 2. WarehouseToTask — Sahaya Malzeme Çıkışı

```
1. Saha görevi (InventoryTask) açılır, araca şoför atanır (VehicleTask)
2. Depodan göreve malzeme talebi oluşturulur (Pending)
3. Workflow onay süreci işler (InReview)
3a. Reddedilirse → Rejected
3b. Onaylanırsa → Approved
4. [Dispatch] → stok depodan araca yüklenir (Shipped)
5. [Receive] → görev lokasyonuna teslim edilir (Completed)
```

### 3. TaskReturnToWarehouse — Görev Sonu İade

```
1. Görev tamamlandı veya iptal edildi
   → Sistem otomatik olarak TaskReturnToWarehouse talebi açar
   → ReturnWarehouseId göreve önceden tanımlanmış olmalıdır
2. Her kalem için iade miktarları girilir:
   ├── ReceivedQuantity   → depoya geri dönen sağlam miktar
   ├── DamagedQuantity    → hasarlı (stok düşümü yapılır)
   ├── LostQuantity       → kayıp (stok düşümü yapılır)
   └── ConsumedQuantity   → sahada kullanılan (stok düşümü yapılır)
3. Workflow onay süreci işler
4. [Dispatch] → araçtan iade yüklenir
5. [Receive] → depo teslim alır, stok güncellenir (Completed)
```

**Genel durum makinesi:**
```
Pending → InReview → Approved → Shipped → Completed
                 ↘ Rejected     ↘ Cancelled
```

> Stok hiçbir zaman onay adımında hareket etmez. Fiziksel hareket yalnızca Dispatch ve Receive adımlarında gerçekleşir.

---

## Mimari

```
Domain.Shared       Enum, hata kodları, ETO event'leri
Domain              Entity'ler, Manager'lar, repository interface'leri, event handler'lar
Application.Contracts  DTO'lar, IAppService interface'leri, FluentValidation kuralları
Application         AppService implementasyonları, AutoMapper profilleri
EntityFrameworkCore DbContext, Fluent API konfigürasyonları, migration'lar
HttpApi             REST controller'lar
HttpApi.Host        Startup, middleware pipeline, SignalR hub, Swagger
```

**Temel kurallar:**
- İş mantığı `Manager` sınıflarındadır. `AppService` sadece orkestrasyon yapar.
- Entity'ler arasında navigation property yoktur; yalnızca `XxxId` FK referansı kullanılır.
- Tüm stok hareketleri `InventoryTransaction` üzerinden geçer — `StockLocation` doğrudan güncellenmez.
- Controller'lar `Result<T>` döner, ham DTO dönmez.

---

## Kurulum

### Gereksinimler
- .NET 10 SDK
- Docker (PostgreSQL için)
- Node.js 18+

### Environment Variables

`appsettings.secrets.json` dosyası oluşturulmalıdır (git'e dahil değildir):

```json
{
  "ConnectionStrings": {
    "Default": "Host=localhost;Port=5432;Database=InventoryTracking;Username=postgres;Password=postgres"
  },
  "AuthServer": {
    "Authority": "https://localhost:44322"
  }
}
```

### Ayağa Kaldırma

```bash
# 1. PostgreSQL başlat
docker-compose up -d

# 2. Migration + seed data uygula (ilk çalıştırmada otomatik)
dotnet run --project host/InventoryTrackingAutomation.HttpApi.Host

# 3. Auth server (ayrı terminal)
dotnet run --project host/InventoryTrackingAutomation.AuthServer
```

Swagger UI: `https://localhost:44300/swagger`

**Hazır gelen seed kullanıcılar** (şifre: `123456aA@`):

| Kullanıcı | Rol |
|-----------|-----|
| `admin` | Admin |
| `warehouse.manager` | WarehouseManager |
| `logistics` | LogisticsSupervisor |
| `driver1` | Driver |

---

## Teknik Tercihler

### ABP Framework
Kimlik yönetimi, yetkilendirme, multi-tenancy, audit logging ve modüler yapı için tercih edildi. Sıfırdan yazmak yerine kanıtlanmış altyapı üzerine iş mantığına odaklanmayı sağlar.

### Dinamik Workflow — Neden Custom?
Piyasadaki workflow motorları (Elsa, Hangfire vb.) genel amaçlıdır. Bu projede onaylayıcının **iş rolüne ve organizasyon yapısına** göre çözülmesi gerekiyordu. Strategy Pattern ile `ResolverKey` tabanlı custom motor, bu ihtiyacı deploy gerektirmeden karşılar.

### OpenIddict
ASP.NET Core ile native entegrasyon, ayrı bir auth servisi çalıştırabilme ve OAuth2/OIDC standartlarına tam uyum için seçildi.

### Navigation Property Yok
Entity'ler arası navigation property kullanılmadı. Sadece `XxxId` FK referansları var. Bu sayede N+1 sorgu riski ortadan kalkar, aggregate boundary'ler net tutulur.

### InventoryTransaction — Append-Only Ledger
`StockLocation` hiçbir zaman doğrudan güncellenmez. Her hareket `InventoryTransaction`'a yeni satır ekler, bakiye bundan türetilir. Bu sayede tam stok geçmişi ve audit trail korunur.

### SignalR
Onay adımı atandığında ilgili kullanıcıya gerçek zamanlı bildirim gönderilir. Polling yerine push-based yapı tercih edildi.

---

## CI/CD

| Workflow | Tetikleyici | Görev |
|----------|-------------|-------|
| .NET CI | push / PR | Build + test |
| CodeQL | push / PR / haftalık | C# güvenlik analizi |
| Docker Build | push | GHCR'a image push |
