using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Enums.Tasks;
using InventoryTrackingAutomation.Enums.Inventory;
using InventoryTrackingAutomation.Enums;
using InventoryTrackingAutomation.Roles;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

using InventoryTrackingAutomation.Permissions;
using Volo.Abp.PermissionManagement;

namespace InventoryTrackingAutomation.Data;

public class InventoryTrackingAutomationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    // ABP'nin RolePermissionValueProvider.ProviderName sabit değeri.
    // Volo.Abp.PermissionManagement.Identity paketi Domain'e bağlı olmadığından sabiti burada karşılıyoruz.
    private const string RolePermissionProviderName = "R";


    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.Department, Guid> _departmentRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.ProductCategory, Guid> _productCategoryRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<Warehouse, Guid> _warehouseRepository;
    private readonly IRepository<Worker, Guid> _workerRepository;
    private readonly IRepository<Vehicle, Guid> _vehicleRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Inventory.StockLocation, Guid> _stockLocationRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction, Guid> _inventoryTransactionRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Tasks.InventoryTask, Guid> _inventoryTaskRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Tasks.VehicleTask, Guid> _vehicleTaskRepository;
    private readonly InventoryTrackingAutomation.Interface.Workflows.IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IdentityRoleManager _identityRoleManager;
    private readonly IdentityUserManager _identityUserManager;
    private readonly IGuidGenerator _guidGenerator;
    private readonly IPermissionManager _permissionManager;

    public InventoryTrackingAutomationDataSeedContributor(
        IRepository<InventoryTrackingAutomation.Entities.Lookups.Department, Guid> departmentRepository,
        IRepository<InventoryTrackingAutomation.Entities.Lookups.ProductCategory, Guid> productCategoryRepository,
        IRepository<Product, Guid> productRepository,
        IRepository<Warehouse, Guid> warehouseRepository,
        IRepository<Worker, Guid> workerRepository,
        IRepository<Vehicle, Guid> vehicleRepository,
        IRepository<InventoryTrackingAutomation.Entities.Inventory.StockLocation, Guid> stockLocationRepository,
        IRepository<InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction, Guid> inventoryTransactionRepository,
        IRepository<InventoryTrackingAutomation.Entities.Tasks.InventoryTask, Guid> inventoryTaskRepository,
        IRepository<InventoryTrackingAutomation.Entities.Tasks.VehicleTask, Guid> vehicleTaskRepository,
        InventoryTrackingAutomation.Interface.Workflows.IWorkflowDefinitionRepository workflowDefinitionRepository,
        IdentityRoleManager identityRoleManager,
        IdentityUserManager identityUserManager,
        IGuidGenerator guidGenerator,
        IPermissionManager permissionManager)
    {
        _departmentRepository = departmentRepository;
        _productCategoryRepository = productCategoryRepository;
        _productRepository = productRepository;
        _warehouseRepository = warehouseRepository;
        _workerRepository = workerRepository;
        _vehicleRepository = vehicleRepository;
        _stockLocationRepository = stockLocationRepository;
        _inventoryTransactionRepository = inventoryTransactionRepository;
        _inventoryTaskRepository = inventoryTaskRepository;
        _vehicleTaskRepository = vehicleTaskRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _identityRoleManager = identityRoleManager;
        _identityUserManager = identityUserManager;
        _guidGenerator = guidGenerator;
        _permissionManager = permissionManager;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 0. Identity Roles & Permissions Seed'leme
        await SeedRolesAndPermissionsAsync();

        // 1. UserId'si olmayan eski orphan worker kayıtlarını temizle
        await CleanupOrphanWorkersAsync();

        // 2. Lookup verileri (Departman ve Kategori)
        Guid lojistikDepId = default;
        Guid sahaDepId = default;
        if (await _departmentRepository.GetCountAsync() == 0)
        {
            lojistikDepId = _guidGenerator.Create();
            await _departmentRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.Department(lojistikDepId)
            {
                Code = "DEP-LOJ",
                Name = "Lojistik Departmanı"
            }, autoSave: true);

            sahaDepId = _guidGenerator.Create();
            await _departmentRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.Department(sahaDepId)
            {
                Code = "DEP-SAH",
                Name = "Saha Operasyonları"
            }, autoSave: true);
        }
        else
        {
            lojistikDepId = (await _departmentRepository.FirstOrDefaultAsync(x => x.Code == "DEP-LOJ"))?.Id ?? default;
            sahaDepId = (await _departmentRepository.FirstOrDefaultAsync(x => x.Code == "DEP-SAH"))?.Id ?? default;
        }

        Guid hammaddeCatId = default;
        Guid ekipmanCatId = default;
        if (await _productCategoryRepository.GetCountAsync() == 0)
        {
            hammaddeCatId = _guidGenerator.Create();
            await _productCategoryRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.ProductCategory(hammaddeCatId)
            {
                Code = "CAT-01",
                Name = "Hammadde ve Yapı Malzemeleri"
            }, autoSave: true);

            ekipmanCatId = _guidGenerator.Create();
            await _productCategoryRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Lookups.ProductCategory(ekipmanCatId)
            {
                Code = "CAT-02",
                Name = "Demirbaş ve Ekipmanlar"
            }, autoSave: true);
        }
        else
        {
            hammaddeCatId = (await _productCategoryRepository.FirstOrDefaultAsync(x => x.Code == "CAT-01"))?.Id ?? default;
            ekipmanCatId = (await _productCategoryRepository.FirstOrDefaultAsync(x => x.Code == "CAT-02"))?.Id ?? default;
        }

        // 3. Warehouse (Lokasyon/Şantiye/Depo) verileri
        if (await _warehouseRepository.GetCountAsync() == 0)
        {
            // İlk uygun manager worker'ı bul
            // İlk saha teknikeri bul
            // Merkez Deposu
            await _warehouseRepository.InsertAsync(new Warehouse(_guidGenerator.Create())
            {
                Code = "WH-01",
                Name = "Merkez Lojistik Deposu",
                ManagerWorkerId = null,
                Address = "Ataşehir / İstanbul",
                IsActive = true
            }, autoSave: true);

            // İkinci Depo
            await _warehouseRepository.InsertAsync(new Warehouse(_guidGenerator.Create())
            {
                Code = "WH-02",
                Name = "Anadolu Yakası Depo",
                ManagerWorkerId = null,
                Address = "Tuzla / İstanbul",
                IsActive = true
            }, autoSave: true);
        }

        // 4. Vehicle (Araç) verileri
        if (await _vehicleRepository.GetCountAsync() == 0)
        {
            // Van araçları
            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 ABC 123",
                VehicleType = VehicleTypeEnum.Van,
                IsActive = true
            }, autoSave: true);

            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 ABC 456",
                VehicleType = VehicleTypeEnum.Van,
                IsActive = true
            }, autoSave: true);

            // Kamyonlar
            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 DEF 789",
                VehicleType = VehicleTypeEnum.Truck,
                IsActive = true
            }, autoSave: true);

            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 GHI 101",
                VehicleType = VehicleTypeEnum.Truck,
                IsActive = true
            }, autoSave: true);

            // Otomobil
            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 JKL 202",
                VehicleType = VehicleTypeEnum.Car,
                IsActive = true
            }, autoSave: true);
        }

        // 5. Product (Malzeme) verileri
        if (await _productRepository.GetCountAsync() == 0)
        {
            // Hammadde ve yapı malzemeleri
            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "PRD-01",
                Name = "C30 Hazır Beton",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = hammaddeCatId != default ? hammaddeCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "PRD-02",
                Name = "Nervürlü İnşaat Demiri Ø16",
                BaseUnit = UnitTypeEnum.Kilogram,
                CategoryId = hammaddeCatId != default ? hammaddeCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "PRD-03",
                Name = "Saf Kum 0-4mm",
                BaseUnit = UnitTypeEnum.Kilogram,
                CategoryId = hammaddeCatId != default ? hammaddeCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "PRD-04",
                Name = "Çakıl 5-15mm",
                BaseUnit = UnitTypeEnum.Kilogram,
                CategoryId = hammaddeCatId != default ? hammaddeCatId : null,
                IsActive = true
            }, autoSave: true);

            // Demirbaş ve ekipmanlar
            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "EQP-01",
                Name = "Hilti Kırıcı Delici TE 3000",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = ekipmanCatId != default ? ekipmanCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "EQP-02",
                Name = "Bosch Darbeli Matkap GSB 16",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = ekipmanCatId != default ? ekipmanCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "EQP-03",
                Name = "İnşaat İskelesi (Standart Çerçeve)",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = ekipmanCatId != default ? ekipmanCatId : null,
                IsActive = true
            }, autoSave: true);

            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "EQP-04",
                Name = "Güvenlik Kemeri",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = ekipmanCatId != default ? ekipmanCatId : null,
                IsActive = true
            }, autoSave: true);
        }
        else
        {
            // Eğer Product'lar önceden eklendiyse ama CategoryId boşsa güncelle
            var products = await _productRepository.GetListAsync();
            foreach (var p in products)
            {
                if (p.CategoryId == null)
                {
                    p.CategoryId = p.Code.StartsWith("PRD") ? hammaddeCatId : ekipmanCatId;
                    await _productRepository.UpdateAsync(p, autoSave: true);
                }
            }
        }

        // 6. Users (IdentityUser) + Workers
        await SeedUsersAndWorkersAsync();

        // 7. Warehouse manager atamalarını deterministik RegNo lookup ile yap
        await AssignWarehouseManagersAsync();

        // 8. StockLocation (Stok) verileri
        if (await _stockLocationRepository.GetCountAsync() == 0)
        {
            var allProducts = await _productRepository.GetListAsync();
            var allWarehouses = await _warehouseRepository.GetListAsync();
            var warehouseWarehouses = allWarehouses.ToList();

            // Depolardaki stok verileri
            if (warehouseWarehouses.Count > 0)
            {
                var warehouse1 = warehouseWarehouses[0];

                // Beton - Depo 1
                var beton = allProducts.FirstOrDefault(x => x.Code == "PRD-01");
                if (beton != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = beton.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 1000,
                        ReservedQuantity = 150
                    }, autoSave: true);
                }

                // Demir - Depo 1
                var demir = allProducts.FirstOrDefault(x => x.Code == "PRD-02");
                if (demir != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = demir.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 50,
                        ReservedQuantity = 10
                    }, autoSave: true);
                }

                // Kum - Depo 1
                var kum = allProducts.FirstOrDefault(x => x.Code == "PRD-03");
                if (kum != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = kum.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 500,
                        ReservedQuantity = 100
                    }, autoSave: true);
                }

                // Çakıl - Depo 1
                var cakil = allProducts.FirstOrDefault(x => x.Code == "PRD-04");
                if (cakil != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = cakil.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 800,
                        ReservedQuantity = 200
                    }, autoSave: true);
                }

                // Hilti Kırıcı - Depo 1
                var hilti = allProducts.FirstOrDefault(x => x.Code == "EQP-01");
                if (hilti != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = hilti.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 10,
                        ReservedQuantity = 2
                    }, autoSave: true);
                }

                // Bosch Matkap - Depo 1
                var bosch = allProducts.FirstOrDefault(x => x.Code == "EQP-02");
                if (bosch != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = bosch.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 8,
                        ReservedQuantity = 1
                    }, autoSave: true);
                }

                // İskele - Depo 1
                var iskele = allProducts.FirstOrDefault(x => x.Code == "EQP-03");
                if (iskele != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = iskele.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 50,
                        ReservedQuantity = 15
                    }, autoSave: true);
                }

                // Güvenlik Kemeri - Depo 1
                var kemeri = allProducts.FirstOrDefault(x => x.Code == "EQP-04");
                if (kemeri != null)
                {
                    await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                    {
                        ProductId = kemeri.Id,
                        LocationType = StockLocationTypeEnum.Warehouse,
                        LocationId = warehouse1.Id,
                        Quantity = 100,
                        ReservedQuantity = 20
                    }, autoSave: true);
                }

                // İkinci depo stokları
                if (warehouseWarehouses.Count > 1)
                {
                    var warehouse2 = warehouseWarehouses[1];
                    if (beton != null)
                    {
                        await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
                        {
                            ProductId = beton.Id,
                            LocationType = StockLocationTypeEnum.Warehouse,
                            LocationId = warehouse2.Id,
                            Quantity = 600,
                            ReservedQuantity = 50
                        }, autoSave: true);
                    }
                }
            }

        }

        // 8. Workflow (İş Akışı) verileri
        if (await _workflowDefinitionRepository.GetCountAsync() == 0)
        {
            var workflowDef = new InventoryTrackingAutomation.Entities.Workflows.WorkflowDefinition(
                id: _guidGenerator.Create(),
                name: "MovementRequest",
                description: "Malzeme hareketi onay akışı",
                isActive: true
            );

            // Adım 1: Talebi başlatanın yöneticisi onaylamalı
            workflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: workflowDef.Id,
                stepOrder: 1,
                requiredRoleName: null,
                resolverKey: "InitiatorManager"
            ));

            // Adım 2: Hedef deposunun yöneticisi onaylamalı
            workflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: workflowDef.Id,
                stepOrder: 2,
                requiredRoleName: null,
                resolverKey: "TargetWarehouseManager"
            ));

            // Adım 3: Kaynak deposunun yöneticisi onaylamalı
            workflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: workflowDef.Id,
                stepOrder: 3,
                requiredRoleName: null,
                resolverKey: "SourceWarehouseManager"
            ));

            await _workflowDefinitionRepository.InsertAsync(workflowDef, autoSave: true);
        }

        // 8.1 TaskMovementRequest (Sahaya Çıkış) İş Akışı
        if (await _workflowDefinitionRepository.FindAsync(w => w.Name == "TaskMovementRequest") == null)
        {
            var taskWorkflowDef = new InventoryTrackingAutomation.Entities.Workflows.WorkflowDefinition(
                id: _guidGenerator.Create(),
                name: "TaskMovementRequest",
                description: "Sahaya malzeme çıkış onay akışı (Daha Hızlı)",
                isActive: true
            );

            // Adım 1: Talebi başlatanın yöneticisi onaylamalı
            taskWorkflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: taskWorkflowDef.Id,
                stepOrder: 1,
                requiredRoleName: null,
                resolverKey: "InitiatorManager"
            ));

            // Adım 2: Lojistik Birimi Onayı (Basit Adım)
            taskWorkflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: taskWorkflowDef.Id,
                stepOrder: 2,
                requiredRoleName: null,
                resolverKey: "LogisticsManager"
            ));

            await _workflowDefinitionRepository.InsertAsync(taskWorkflowDef, autoSave: true);
        }

        // 9. InventoryTask + VehicleTask + Araç Stokları + InventoryTransaction seed verileri
        // PDF senaryosu: "Telsiz 20 adet → 10 depoda, 5 araç-1'de, 5 araç-2'de"
        await SeedTasksAndTransactionsAsync();
    }

    private async Task SeedTasksAndTransactionsAsync()
    {
        if (await _inventoryTaskRepository.GetCountAsync() > 0) return;

        // Gerekli verileri çek
        var allVehicles = await _vehicleRepository.GetListAsync();
        var allProducts = await _productRepository.GetListAsync();
        var allWarehouses = await _warehouseRepository.GetListAsync();
        var allWorkers = await _workerRepository.GetListAsync();

        var vehicle1 = allVehicles.FirstOrDefault(v => v.PlateNumber == "34 ABC 123");
        var vehicle2 = allVehicles.FirstOrDefault(v => v.PlateNumber == "34 ABC 456");
        var warehouse1 = allWarehouses.FirstOrDefault(w => w.Code == "WH-01");
        var telsiz = allProducts.FirstOrDefault(p => p.Code == "EQP-01"); // Hilti Kırıcı → Telsiz olarak kullan
        var matkap = allProducts.FirstOrDefault(p => p.Code == "EQP-02");
        var driverAli = allWorkers.FirstOrDefault(w => w.RegistrationNumber == "DRV-001");
        var driverVeli = allWorkers.FirstOrDefault(w => w.RegistrationNumber == "DRV-002");

        if (vehicle1 == null || vehicle2 == null || warehouse1 == null || telsiz == null || matkap == null) return;
        if (driverAli == null || driverVeli == null) return;

        // --- InventoryTask 1: İzmir Saha Destek Görevi (InProgress) ---
        var task1 = new InventoryTrackingAutomation.Entities.Tasks.InventoryTask(_guidGenerator.Create())
        {
            Code = "TSK-001",
            Name = "İzmir Saha Destek Görevi",
            Region = "İzmir / Bornova",
            StartDate = DateTime.UtcNow.AddDays(-3),
            EndDate = null,
            Status = TaskStatusEnum.InProgress,
            Description = "İzmir Bornova bölgesinde saha destek operasyonu.",
            ReturnWarehouseId = warehouse1.Id,
            IsActive = true
        };
        await _inventoryTaskRepository.InsertAsync(task1, autoSave: true);

        // --- InventoryTask 2: Ankara Bakım Görevi (Draft) ---
        var task2 = new InventoryTrackingAutomation.Entities.Tasks.InventoryTask(_guidGenerator.Create())
        {
            Code = "TSK-002",
            Name = "Ankara Bakım Görevi",
            Region = "Ankara / Çankaya",
            StartDate = DateTime.UtcNow.AddDays(2),
            EndDate = DateTime.UtcNow.AddDays(5),
            Status = TaskStatusEnum.Draft,
            Description = "Ankara Çankaya bölgesinde planlı bakım operasyonu.",
            ReturnWarehouseId = warehouse1.Id,
            IsActive = true
        };
        await _inventoryTaskRepository.InsertAsync(task2, autoSave: true);

        // --- InventoryTask 3: Tamamlanmış Görev ---
        var task3 = new InventoryTrackingAutomation.Entities.Tasks.InventoryTask(_guidGenerator.Create())
        {
            Code = "TSK-003",
            Name = "Bursa Acil Müdahale",
            Region = "Bursa / Nilüfer",
            StartDate = DateTime.UtcNow.AddDays(-10),
            EndDate = DateTime.UtcNow.AddDays(-7),
            Status = TaskStatusEnum.Completed,
            Description = "Bursa Nilüfer bölgesinde tamamlanmış acil müdahale.",
            ReturnWarehouseId = warehouse1.Id,
            IsActive = false
        };
        await _inventoryTaskRepository.InsertAsync(task3, autoSave: true);

        // --- VehicleTask: 34 ABC 123 → İzmir Görevi, Şoför Ali ---
        var vt1 = new InventoryTrackingAutomation.Entities.Tasks.VehicleTask(_guidGenerator.Create())
        {
            VehicleId = vehicle1.Id,
            InventoryTaskId = task1.Id,
            DriverWorkerId = driverAli.Id,
            AssignedAt = DateTime.UtcNow.AddDays(-3),
            IsActive = true
        };
        await _vehicleTaskRepository.InsertAsync(vt1, autoSave: true);

        // --- VehicleTask: 34 ABC 456 → İzmir Görevi, Şoför Veli ---
        var vt2 = new InventoryTrackingAutomation.Entities.Tasks.VehicleTask(_guidGenerator.Create())
        {
            VehicleId = vehicle2.Id,
            InventoryTaskId = task1.Id,
            DriverWorkerId = driverVeli.Id,
            AssignedAt = DateTime.UtcNow.AddDays(-3),
            IsActive = true
        };
        await _vehicleTaskRepository.InsertAsync(vt2, autoSave: true);

        // --- Araç Üstü Stoklar (StockLocation) ---
        // Araç 1'de 3 adet Hilti Kırıcı
        await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
        {
            ProductId = telsiz.Id,
            LocationType = StockLocationTypeEnum.Vehicle,
            LocationId = vehicle1.Id,
            Quantity = 3,
            ReservedQuantity = 0
        }, autoSave: true);

        // Araç 2'de 2 adet Hilti Kırıcı
        await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
        {
            ProductId = telsiz.Id,
            LocationType = StockLocationTypeEnum.Vehicle,
            LocationId = vehicle2.Id,
            Quantity = 2,
            ReservedQuantity = 0
        }, autoSave: true);

        // Araç 1'de 1 adet Bosch Matkap
        await _stockLocationRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.StockLocation(_guidGenerator.Create())
        {
            ProductId = matkap.Id,
            LocationType = StockLocationTypeEnum.Vehicle,
            LocationId = vehicle1.Id,
            Quantity = 1,
            ReservedQuantity = 0
        }, autoSave: true);

        // --- InventoryTransaction Ledger Kayıtları ---
        // Hilti Kırıcı: Depodan Araç 1'e 3 adet
        await _inventoryTransactionRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction(_guidGenerator.Create())
        {
            ProductId = telsiz.Id,
            TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
            Quantity = 3,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = warehouse1.Id,
            TargetLocationType = StockLocationTypeEnum.Vehicle,
            TargetLocationId = vehicle1.Id,
            RelatedTaskId = task1.Id,
            OccurredAt = DateTime.UtcNow.AddDays(-3),
            Note = "İzmir Saha Destek Görevi için Hilti Kırıcı yükleme"
        }, autoSave: true);

        // Hilti Kırıcı: Depodan Araç 2'ye 2 adet
        await _inventoryTransactionRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction(_guidGenerator.Create())
        {
            ProductId = telsiz.Id,
            TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
            Quantity = 2,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = warehouse1.Id,
            TargetLocationType = StockLocationTypeEnum.Vehicle,
            TargetLocationId = vehicle2.Id,
            RelatedTaskId = task1.Id,
            OccurredAt = DateTime.UtcNow.AddDays(-3),
            Note = "İzmir Saha Destek Görevi için Hilti Kırıcı yükleme"
        }, autoSave: true);

        // Bosch Matkap: Depodan Araç 1'e 1 adet
        await _inventoryTransactionRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction(_guidGenerator.Create())
        {
            ProductId = matkap.Id,
            TransactionType = InventoryTransactionTypeEnum.WarehouseToVehicle,
            Quantity = 1,
            SourceLocationType = StockLocationTypeEnum.Warehouse,
            SourceLocationId = warehouse1.Id,
            TargetLocationType = StockLocationTypeEnum.Vehicle,
            TargetLocationId = vehicle1.Id,
            RelatedTaskId = task1.Id,
            OccurredAt = DateTime.UtcNow.AddDays(-3),
            Note = "İzmir Saha Destek Görevi için Bosch Matkap yükleme"
        }, autoSave: true);

        // Tamamlanmış görev: iade transaction'ı (Araçtan depoya geri dönüş)
        await _inventoryTransactionRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Inventory.InventoryTransaction(_guidGenerator.Create())
        {
            ProductId = telsiz.Id,
            TransactionType = InventoryTransactionTypeEnum.VehicleToWarehouse,
            Quantity = 2,
            SourceLocationType = StockLocationTypeEnum.Vehicle,
            SourceLocationId = vehicle1.Id,
            TargetLocationType = StockLocationTypeEnum.Warehouse,
            TargetLocationId = warehouse1.Id,
            RelatedTaskId = task3.Id,
            OccurredAt = DateTime.UtcNow.AddDays(-7),
            Note = "Bursa Acil Müdahale tamamlandı - ekipman iadesi"
        }, autoSave: true);
    }

    private async Task CleanupOrphanWorkersAsync()
    {
        // UserId'si AbpUsers'da bulunmayan Worker kayıtlarını temizle.
        // Bu kayıtlar önceki seed sürümlerinden kalabilir; Warehouse manager FK'sini
        // bunlara bağlamak workflow'u kırar (login imkansız).
        var allWorkers = await _workerRepository.GetListAsync();
        foreach (var worker in allWorkers)
        {
            if (worker.UserId == default) continue;

            var user = await _identityUserManager.FindByIdAsync(worker.UserId.ToString());
            if (user == null)
            {
                // Önce bu worker'ı manager olarak gösteren Warehouseleri null'a çek
                var orphanedWarehouses = await _warehouseRepository.GetListAsync(s => s.ManagerWorkerId == worker.Id);
                foreach (var Warehouse in orphanedWarehouses)
                {
                    Warehouse.ManagerWorkerId = null;
                    await _warehouseRepository.UpdateAsync(Warehouse, autoSave: true);
                }

                // Sonra worker'ı manager gösteren diğer worker'ları null'a çek (hiyerarşi)
                var subordinates = await _workerRepository.GetListAsync(w => w.ManagerId == worker.Id);
                foreach (var sub in subordinates)
                {
                    sub.ManagerId = null;
                    await _workerRepository.UpdateAsync(sub, autoSave: true);
                }

                await _workerRepository.DeleteAsync(worker, autoSave: true);
            }
        }
    }

    private async Task AssignWarehouseManagersAsync()
    {
        // Deterministik atama - RegNo bazlı lookup, [0] indeksleme yerine
        var manager01 = await _workerRepository.FirstOrDefaultAsync(w => w.RegistrationNumber == "MGR-001");
        var warehouse01 = await _workerRepository.FirstOrDefaultAsync(w => w.RegistrationNumber == "WRK-WH-001");

        if (manager01 == null || warehouse01 == null)
        {
            // SeedUsersAndWorkersAsync düzgün çalıştıysa burası asla isabet etmemeli
            return;
        }

        var WarehouseManagerMap = new (string WarehouseCode, Guid ManagerWorkerId)[]
        {
            ("WH-01", manager01.Id),
            ("WH-02", manager01.Id),
        };

        foreach (var (WarehouseCode, managerId) in WarehouseManagerMap)
        {
            var Warehouse = await _warehouseRepository.FirstOrDefaultAsync(s => s.Code == WarehouseCode);
            if (Warehouse == null) continue;

            // Idempotent - sadece null ya da farklıysa güncelle (re-seed güvenli)
            if (Warehouse.ManagerWorkerId != managerId)
            {
                Warehouse.ManagerWorkerId = managerId;
                await _warehouseRepository.UpdateAsync(Warehouse, autoSave: true);
            }
        }
    }

    private async Task SeedRolesAndPermissionsAsync()
    {
        // Rol → izin haritası: tek doğruluk kaynağı, yeni rol/izin eklemek için sadece bu sözlük güncellenir.
        var managedPermissions = new[]
        {
            InventoryTrackingAutomationPermissions.MovementRequests.View,
            InventoryTrackingAutomationPermissions.MovementRequests.Create,
            InventoryTrackingAutomationPermissions.MovementRequests.Edit,
            InventoryTrackingAutomationPermissions.MovementRequests.Delete,
            InventoryTrackingAutomationPermissions.MovementRequests.Dispatch,
            InventoryTrackingAutomationPermissions.MovementRequests.Receive,
            InventoryTrackingAutomationPermissions.Workflows.View,
            InventoryTrackingAutomationPermissions.Workflows.Approve,
            InventoryTrackingAutomationPermissions.Workflows.Reject,
            InventoryTrackingAutomationPermissions.Inventory.View,
            InventoryTrackingAutomationPermissions.Inventory.Manage,
            InventoryTrackingAutomationPermissions.Tasks.View,
            InventoryTrackingAutomationPermissions.Tasks.Manage,
            InventoryTrackingAutomationPermissions.Tasks.Complete,
            InventoryTrackingAutomationPermissions.VehicleTasks.View,
            InventoryTrackingAutomationPermissions.VehicleTasks.Manage,
            InventoryTrackingAutomationPermissions.Masters.View,
            InventoryTrackingAutomationPermissions.Masters.Manage
        };

        var rolePermissionMap = new Dictionary<string, string[]>
        {
            [InventoryTrackingAutomationRoleConstants.Admin] = InventoryTrackingAutomationPermissions.GetAll(),

            [InventoryTrackingAutomationRoleConstants.Manager] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.Workflows.Approve,
                InventoryTrackingAutomationPermissions.Workflows.Reject,
                InventoryTrackingAutomationPermissions.MovementRequests.Dispatch,
                InventoryTrackingAutomationPermissions.MovementRequests.Receive,
                InventoryTrackingAutomationPermissions.Inventory.Manage,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.Tasks.Manage,
                InventoryTrackingAutomationPermissions.Tasks.Complete,
                InventoryTrackingAutomationPermissions.VehicleTasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.Manage
            },

            [InventoryTrackingAutomationRoleConstants.WorkflowApprover] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.Workflows.Approve,
                InventoryTrackingAutomationPermissions.Workflows.Reject,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.View
            },

            [InventoryTrackingAutomationRoleConstants.WarehouseWorker] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.Workflows.Approve,
                InventoryTrackingAutomationPermissions.MovementRequests.Dispatch,
                InventoryTrackingAutomationPermissions.MovementRequests.Receive,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.View
            },

            [InventoryTrackingAutomationRoleConstants.FieldWorker] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.MovementRequests.Create,
                InventoryTrackingAutomationPermissions.MovementRequests.Receive,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.View
            },

            [InventoryTrackingAutomationRoleConstants.LogisticsSupervisor] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.Workflows.Approve,
                InventoryTrackingAutomationPermissions.Workflows.Reject,
                InventoryTrackingAutomationPermissions.MovementRequests.Dispatch,
                InventoryTrackingAutomationPermissions.MovementRequests.Receive,
                InventoryTrackingAutomationPermissions.Inventory.Manage,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.Tasks.Manage,
                InventoryTrackingAutomationPermissions.Tasks.Complete,
                InventoryTrackingAutomationPermissions.VehicleTasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.Manage
            },

            [InventoryTrackingAutomationRoleConstants.VehicleManager] = new[]
            {
                InventoryTrackingAutomationPermissions.MovementRequests.View,
                InventoryTrackingAutomationPermissions.Workflows.View,
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.Masters.View,
                InventoryTrackingAutomationPermissions.Masters.Manage,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.Manage
            },

            [InventoryTrackingAutomationRoleConstants.Driver] = new[]
            {
                InventoryTrackingAutomationPermissions.Inventory.View,
                InventoryTrackingAutomationPermissions.MovementRequests.Receive,
                InventoryTrackingAutomationPermissions.Tasks.View,
                InventoryTrackingAutomationPermissions.VehicleTasks.View,
                InventoryTrackingAutomationPermissions.Masters.View
            }
        };

        // Tüm rolleri oluştur (yoksa).
        foreach (var roleName in rolePermissionMap.Keys)
        {
            var role = await _identityRoleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                role = new IdentityRole(_guidGenerator.Create(), roleName);
                await _identityRoleManager.CreateAsync(role);
            }
        }

        // İzinleri ABP'nin RolePermissionValueProvider sabiti ile rollere ata (magic string "R" yerine).
        foreach (var (roleName, permissions) in rolePermissionMap)
        {
            var permissionsToSet = roleName == InventoryTrackingAutomationRoleConstants.Admin
                ? InventoryTrackingAutomationPermissions.GetAll()
                : managedPermissions;

            foreach (var permission in permissionsToSet)
            {
                await _permissionManager.SetAsync(
                    permission,
                    RolePermissionProviderName,
                    roleName,
                    permissions.Contains(permission));
            }
        }
    }

    private async Task<Dictionary<string, Guid>> SeedUsersAndWorkersAsync()
    {
        var userWorkerMap = new Dictionary<string, Guid>();
        var workerIdMap = new Dictionary<string, Guid>(); // Username → Worker.Id mapping

        // Test kullanıcı ve worker verileri tanımla
        var usersToCreate = new[]
        {
            new { Username = "admin", Email = "admin@inventorysystem.local", Password = "123456aA@", FullName = "Sistem Yöneticisi", Roles = new[] { InventoryTrackingAutomationRoleConstants.Admin }, RegNo = "ADM-001", WorkerType = WorkerTypeEnum.WhiteCollar, ManagerUsername = (string)null },
            new { Username = "manager.istanbul", Email = "manager.istanbul@inventorysystem.local", Password = "123456aA@", FullName = "İstanbul Şube Müdürü", Roles = new[] { InventoryTrackingAutomationRoleConstants.Manager }, RegNo = "MGR-001", WorkerType = WorkerTypeEnum.WhiteCollar, ManagerUsername = "admin" },
            new { Username = "supervisor.logistics", Email = "supervisor.logistics@inventorysystem.local", Password = "123456aA@", FullName = "Lojistik Operasyon Müdürü", Roles = new[] { InventoryTrackingAutomationRoleConstants.LogisticsSupervisor }, RegNo = "SUP-LOG-001", WorkerType = WorkerTypeEnum.WhiteCollar, ManagerUsername = "manager.istanbul" },
            new { Username = "approver.warehouse", Email = "approver.warehouse@inventorysystem.local", Password = "123456aA@", FullName = "Depo Onay Sorumlusu", Roles = new[] { InventoryTrackingAutomationRoleConstants.WorkflowApprover }, RegNo = "APP-001", WorkerType = WorkerTypeEnum.WhiteCollar, ManagerUsername = "supervisor.logistics" },
            new { Username = "worker.warehouse01", Email = "worker.warehouse01@inventorysystem.local", Password = "123456aA@", FullName = "Depo Operatörü - Merkezhan", Roles = new[] { InventoryTrackingAutomationRoleConstants.WarehouseWorker }, RegNo = "WRK-WH-001", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "approver.warehouse" },
            new { Username = "worker.warehouse02", Email = "worker.warehouse02@inventorysystem.local", Password = "123456aA@", FullName = "Depo Operatörü - Depo 2", Roles = new[] { InventoryTrackingAutomationRoleConstants.WarehouseWorker }, RegNo = "WRK-WH-002", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "approver.warehouse" },
            new { Username = "worker.field01", Email = "worker.field01@inventorysystem.local", Password = "123456aA@", FullName = "Saha Teknikeri - Kadıköy", Roles = new[] { InventoryTrackingAutomationRoleConstants.FieldWorker }, RegNo = "WRK-FLD-001", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "supervisor.logistics" },
            new { Username = "worker.field02", Email = "worker.field02@inventorysystem.local", Password = "123456aA@", FullName = "Saha Teknikeri - Taksim", Roles = new[] { InventoryTrackingAutomationRoleConstants.FieldWorker }, RegNo = "WRK-FLD-002", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "supervisor.logistics" },
            new { Username = "manager.vehicle", Email = "manager.vehicle@inventorysystem.local", Password = "123456aA@", FullName = "Araç Servis Müdürü", Roles = new[] { InventoryTrackingAutomationRoleConstants.VehicleManager }, RegNo = "MGR-VHC-001", WorkerType = WorkerTypeEnum.WhiteCollar, ManagerUsername = "manager.istanbul" },
            new { Username = "driver.ali", Email = "driver.ali@inventorysystem.local", Password = "123456aA@", FullName = "Ali Yılmaz (Şoför)", Roles = new[] { InventoryTrackingAutomationRoleConstants.Driver }, RegNo = "DRV-001", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "manager.vehicle" },
            new { Username = "driver.veli", Email = "driver.veli@inventorysystem.local", Password = "123456aA@", FullName = "Veli Demir (Şoför)", Roles = new[] { InventoryTrackingAutomationRoleConstants.Driver }, RegNo = "DRV-002", WorkerType = WorkerTypeEnum.BlueCollar, ManagerUsername = "manager.vehicle" }
        };

        // Departmanlar ve Warehouseler hazırla
        var departments = await _departmentRepository.GetListAsync();
        var lojistikDep = departments.Find(x => x.Code == "DEP-LOJ") ??
                         (await _departmentRepository.FirstOrDefaultAsync(x => x.Name.Contains("Lojistik")));
        var sahaDepId = departments.Find(x => x.Code == "DEP-SAH")?.Id ??
                       (await _departmentRepository.FirstOrDefaultAsync(x => x.Name.Contains("Saha")))?.Id;

        var Warehouses = await _warehouseRepository.GetListAsync();
        var warehouseWarehouse = Warehouses.Find(x => x.Code == "WH-01");

        // Adım 1: Kullanıcı ve worker'ları oluştur (ManagerId henüz set etme)
        foreach (var userInfo in usersToCreate)
        {
            // Kullanıcı zaten varsa atla ama ID'sini kaydet
            var existingUser = await _identityUserManager.FindByNameAsync(userInfo.Username);
            if (existingUser != null)
            {
                userWorkerMap[userInfo.Username] = existingUser.Id;
                var existingWorker = await _workerRepository.FirstOrDefaultAsync(w => w.UserId == existingUser.Id);
                if (existingWorker != null)
                {
                    workerIdMap[userInfo.Username] = existingWorker.Id;
                }
                continue;
            }

            // Yeni kullanıcı oluştur
            var userId = _guidGenerator.Create();
            var user = new IdentityUser(userId, userInfo.Username, userInfo.Email)
            {
                Name = userInfo.FullName
            };

            var createResult = await _identityUserManager.CreateAsync(user, userInfo.Password);
            if (!createResult.Succeeded)
                throw new Exception($"Kullanıcı oluşturulamadı: {userInfo.Username}");

            // Rolleri ata
            foreach (var roleName in userInfo.Roles)
            {
                await _identityUserManager.AddToRoleAsync(user, roleName);
            }

            userWorkerMap[userInfo.Username] = userId;

            // Worker kaydı oluştur (ManagerId boş bırak, sonra set edelim)
            var workerId = _guidGenerator.Create();
            var worker = new Worker(workerId)
            {
                UserId = userId,
                RegistrationNumber = userInfo.RegNo,
                WorkerType = userInfo.WorkerType,
                DepartmentId = userInfo.WorkerType == WorkerTypeEnum.WhiteCollar ? lojistikDep?.Id : sahaDepId,
                DefaultWarehouseId = userInfo.WorkerType == WorkerTypeEnum.WhiteCollar ? warehouseWarehouse?.Id : null,
                IsActive = true,
                ManagerId = null // Adım 2'de set edelim
            };

            await _workerRepository.InsertAsync(worker, autoSave: true);
            workerIdMap[userInfo.Username] = workerId;
        }

        // Adım 2: Worker'ların ManagerId'lerini set et (hiyerarşi kurma)
        foreach (var userInfo in usersToCreate)
        {
            if (!string.IsNullOrEmpty(userInfo.ManagerUsername) && workerIdMap.ContainsKey(userInfo.Username))
            {
                var worker = await _workerRepository.GetAsync(workerIdMap[userInfo.Username]);
                if (workerIdMap.ContainsKey(userInfo.ManagerUsername))
                {
                    worker.ManagerId = workerIdMap[userInfo.ManagerUsername];
                    await _workerRepository.UpdateAsync(worker, autoSave: true);
                }
            }
        }

        return userWorkerMap;
    }
}
