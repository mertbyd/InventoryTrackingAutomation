using System;
using System.Threading.Tasks;
using InventoryTrackingAutomation.Entities.Masters;
using InventoryTrackingAutomation.Enums;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Guids;

namespace InventoryTrackingAutomation.Data;

public class InventoryTrackingAutomationDataSeedContributor : IDataSeedContributor, ITransientDependency
{
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.Department, Guid> _departmentRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Lookups.ProductCategory, Guid> _productCategoryRepository;
    private readonly IRepository<Product, Guid> _productRepository;
    private readonly IRepository<Site, Guid> _siteRepository;
    private readonly IRepository<Worker, Guid> _workerRepository;
    private readonly IRepository<Vehicle, Guid> _vehicleRepository;
    private readonly IRepository<InventoryTrackingAutomation.Entities.Stock.ProductStock, Guid> _productStockRepository;
    private readonly InventoryTrackingAutomation.Interface.Workflows.IWorkflowDefinitionRepository _workflowDefinitionRepository;
    private readonly IGuidGenerator _guidGenerator;

    public InventoryTrackingAutomationDataSeedContributor(
        IRepository<InventoryTrackingAutomation.Entities.Lookups.Department, Guid> departmentRepository,
        IRepository<InventoryTrackingAutomation.Entities.Lookups.ProductCategory, Guid> productCategoryRepository,
        IRepository<Product, Guid> productRepository,
        IRepository<Site, Guid> siteRepository,
        IRepository<Worker, Guid> workerRepository,
        IRepository<Vehicle, Guid> vehicleRepository,
        IRepository<InventoryTrackingAutomation.Entities.Stock.ProductStock, Guid> productStockRepository,
        InventoryTrackingAutomation.Interface.Workflows.IWorkflowDefinitionRepository workflowDefinitionRepository,
        IGuidGenerator guidGenerator)
    {
        _departmentRepository = departmentRepository;
        _productCategoryRepository = productCategoryRepository;
        _productRepository = productRepository;
        _siteRepository = siteRepository;
        _workerRepository = workerRepository;
        _vehicleRepository = vehicleRepository;
        _productStockRepository = productStockRepository;
        _workflowDefinitionRepository = workflowDefinitionRepository;
        _guidGenerator = guidGenerator;
    }

    public async Task SeedAsync(DataSeedContext context)
    {
        // 0. Lookup verileri (Departman ve Kategori)
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

        // 1. Worker (Çalışan) verileri
        if (await _workerRepository.GetCountAsync() == 0)
        {
            var managerId = _guidGenerator.Create();
            var manager = new Worker(managerId)
            {
                UserId = Guid.NewGuid(), // Dummy User
                RegistrationNumber = "EMP-MGR-001",
                WorkerType = WorkerTypeEnum.WhiteCollar,
                DepartmentId = lojistikDepId != default ? lojistikDepId : null,
                IsActive = true
            };

            var worker = new Worker(_guidGenerator.Create())
            {
                UserId = Guid.NewGuid(), // Dummy User
                RegistrationNumber = "EMP-FLD-002",
                WorkerType = WorkerTypeEnum.BlueCollar,
                ManagerId = managerId,
                DepartmentId = sahaDepId != default ? sahaDepId : null,
                IsActive = true
            };

            await _workerRepository.InsertAsync(manager, autoSave: true);
            await _workerRepository.InsertAsync(worker, autoSave: true);

            // 2. Site (Lokasyon/Şantiye/Depo) verileri
            if (await _siteRepository.GetCountAsync() == 0)
            {
                await _siteRepository.InsertAsync(new Site(_guidGenerator.Create())
                {
                    Code = "WH-01",
                    Name = "Merkez Lojistik Deposu",
                    SiteType = SiteTypeEnum.Warehouse,
                    ManagerWorkerId = managerId,
                    Address = "Ataşehir / İstanbul",
                    IsActive = true
                }, autoSave: true);

                await _siteRepository.InsertAsync(new Site(_guidGenerator.Create())
                {
                    Code = "ST-01",
                    Name = "Kadıköy Kentsel Dönüşüm Şantiyesi",
                    SiteType = SiteTypeEnum.Field,
                    ManagerWorkerId = worker.Id,
                    Address = "Kadıköy / İstanbul",
                    IsActive = true
                }, autoSave: true);
            }
        }
        else
        {
            // Eğer Worker'lar önceden eklendiyse ama DepartmentId boşsa güncelle
            var workers = await _workerRepository.GetListAsync();
            foreach (var w in workers)
            {
                if (w.DepartmentId == null)
                {
                    w.DepartmentId = w.RegistrationNumber == "EMP-MGR-001" ? lojistikDepId : sahaDepId;
                    await _workerRepository.UpdateAsync(w, autoSave: true);
                }
            }
        }

        // 3. Vehicle (Araç) verileri
        if (await _vehicleRepository.GetCountAsync() == 0)
        {
            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 ABC 123",
                VehicleType = VehicleTypeEnum.Van,
                IsActive = true
            }, autoSave: true);

            await _vehicleRepository.InsertAsync(new Vehicle(_guidGenerator.Create())
            {
                PlateNumber = "34 DEF 456",
                VehicleType = VehicleTypeEnum.Truck,
                IsActive = true
            }, autoSave: true);
        }

        // 4. Product (Malzeme) verileri
        if (await _productRepository.GetCountAsync() == 0)
        {
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
                Name = "Nervürlü İnşaat Demiri",
                BaseUnit = UnitTypeEnum.Piece,
                CategoryId = hammaddeCatId != default ? hammaddeCatId : null,
                IsActive = true
            }, autoSave: true);
            
            await _productRepository.InsertAsync(new Product(_guidGenerator.Create())
            {
                Code = "EQP-01",
                Name = "Hilti Kırıcı Delici",
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

        // 5. ProductStock (Stok) verileri
        if (await _productStockRepository.GetCountAsync() == 0)
        {
            var warehouse = await _siteRepository.FirstOrDefaultAsync(x => x.Code == "WH-01");
            var beton = await _productRepository.FirstOrDefaultAsync(x => x.Code == "PRD-01");
            var demir = await _productRepository.FirstOrDefaultAsync(x => x.Code == "PRD-02");
            var hilti = await _productRepository.FirstOrDefaultAsync(x => x.Code == "EQP-01");

            if (warehouse != null && beton != null && demir != null && hilti != null)
            {
                await _productStockRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Stock.ProductStock(_guidGenerator.Create())
                {
                    ProductId = beton.Id,
                    SiteId = warehouse.Id,
                    TotalQuantity = 1000,
                    ReservedQuantity = 0
                }, autoSave: true);

                await _productStockRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Stock.ProductStock(_guidGenerator.Create())
                {
                    ProductId = demir.Id,
                    SiteId = warehouse.Id,
                    TotalQuantity = 5000,
                    ReservedQuantity = 0
                }, autoSave: true);

                await _productStockRepository.InsertAsync(new InventoryTrackingAutomation.Entities.Stock.ProductStock(_guidGenerator.Create())
                {
                    ProductId = hilti.Id,
                    SiteId = warehouse.Id,
                    TotalQuantity = 10,
                    ReservedQuantity = 0
                }, autoSave: true);
            }
        }

        // 6. Workflow (İş Akışı) verileri
        if (await _workflowDefinitionRepository.GetCountAsync() == 0)
        {
            var workflowDef = new InventoryTrackingAutomation.Entities.Workflows.WorkflowDefinition(
                id: _guidGenerator.Create(),
                name: "MovementRequest",
                description: "Hareket Talebi İş Akışı",
                isActive: true
            );

            // Adım 1: Talebi başlatanın yöneticisi onaylamalı
            workflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: workflowDef.Id,
                stepOrder: 1,
                requiredRoleName: null,
                isManagerApprovalRequired: true,
                resolverKey: null
            ));

            // Adım 2: Çıkış deposunun yöneticisi onaylamalı
            workflowDef.Steps.Add(new InventoryTrackingAutomation.Entities.Workflows.WorkflowStepDefinition(
                id: _guidGenerator.Create(),
                workflowDefinitionId: workflowDef.Id,
                stepOrder: 2,
                requiredRoleName: null,
                isManagerApprovalRequired: false,
                resolverKey: "SourceSiteManager"
            ));

            await _workflowDefinitionRepository.InsertAsync(workflowDef, autoSave: true);
        }
    }
}
