using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class AddPitonInventoryModelV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_MovementRequests_movement_request_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_Products_product_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_Sites_source_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_Sites_target_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_Vehicles_source_vehicle_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_Vehicles_target_vehicle_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_vehicle_tasks_vehicle_task_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementApprovals_MovementRequests_MovementRequestId",
                schema: "movement",
                table: "MovementApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementApprovals_Workers_ApproverWorkerId",
                schema: "movement",
                table: "MovementApprovals");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequestLines_MovementRequests_MovementRequestId",
                schema: "movement",
                table: "MovementRequestLines");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequestLines_Products_ProductId",
                schema: "movement",
                table: "MovementRequestLines");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequests_Sites_SourceSiteId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequests_Sites_TargetSiteId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequests_Vehicles_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequests_Workers_RequestedByWorkerId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentId",
                schema: "lookup",
                table: "ProductCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                schema: "master",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_locations_Products_product_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_locations_Sites_warehouse_site_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_locations_Vehicles_vehicle_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_Vehicles_vehicle_id",
                schema: "piton",
                table: "vehicle_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_inventory_tasks_inventory_task_id",
                schema: "piton",
                table: "vehicle_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Departments_DepartmentId",
                schema: "master",
                table: "Workers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Sites_DefaultSiteId",
                schema: "master",
                table: "Workers");

            migrationBuilder.DropForeignKey(
                name: "FK_Workers_Workers_ManagerId",
                schema: "master",
                table: "Workers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "WorkflowInstances");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstanceSteps_WorkflowInstances_WorkflowInstanceId",
                schema: "workflow",
                table: "WorkflowInstanceSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowInstanceSteps_WorkflowStepDefinitions_WorkflowStepD~",
                schema: "workflow",
                table: "WorkflowInstanceSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkflowStepDefinitions_WorkflowDefinitions_WorkflowDefinit~",
                schema: "workflow",
                table: "WorkflowStepDefinitions");

            migrationBuilder.DropTable(
                name: "ProductStocks",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "StockMovements",
                schema: "stock");

            migrationBuilder.DropTable(
                name: "Sites",
                schema: "master");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Workers",
                schema: "master",
                table: "Workers");

            migrationBuilder.DropIndex(
                name: "IX_Workers_DefaultSiteId",
                schema: "master",
                table: "Workers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vehicles",
                schema: "master",
                table: "Vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                schema: "master",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Departments",
                schema: "lookup",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_stock_locations_product_id_location_type_warehouse_site_id_~",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropIndex(
                name: "IX_stock_locations_vehicle_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropIndex(
                name: "IX_stock_locations_warehouse_site_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_movement_request_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_target_vehicle_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_target_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_vehicle_task_id",
                schema: "piton",
                table: "inventory_transactions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowStepDefinitions",
                schema: "workflow",
                table: "WorkflowStepDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowInstanceSteps",
                schema: "workflow",
                table: "WorkflowInstanceSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowInstances",
                schema: "workflow",
                table: "WorkflowInstances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_WorkflowDefinitions",
                schema: "workflow",
                table: "WorkflowDefinitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductCategories",
                schema: "lookup",
                table: "ProductCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovementRequests",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropIndex(
                name: "IX_MovementRequests_SourceSiteId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovementRequestLines",
                schema: "movement",
                table: "MovementRequestLines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MovementApprovals",
                schema: "movement",
                table: "MovementApprovals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventory_tasks",
                schema: "piton",
                table: "inventory_tasks");

            migrationBuilder.DropColumn(
                name: "vehicle_id",
                schema: "piton",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "SourceSiteId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.EnsureSchema(
                name: "operation");

            migrationBuilder.RenameTable(
                name: "Workers",
                schema: "master",
                newName: "workers",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "Vehicles",
                schema: "master",
                newName: "vehicles",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "Products",
                schema: "master",
                newName: "products",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "Departments",
                schema: "lookup",
                newName: "departments",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "vehicle_tasks",
                schema: "piton",
                newName: "vehicle_tasks",
                newSchema: "operation");

            migrationBuilder.RenameTable(
                name: "stock_locations",
                schema: "piton",
                newName: "stock_locations",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "inventory_transactions",
                schema: "piton",
                newName: "inventory_transactions",
                newSchema: "inventory");

            migrationBuilder.RenameTable(
                name: "WorkflowStepDefinitions",
                schema: "workflow",
                newName: "workflow_step_definitions",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "WorkflowInstanceSteps",
                schema: "workflow",
                newName: "workflow_instance_steps",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "WorkflowInstances",
                schema: "workflow",
                newName: "workflow_instances",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "WorkflowDefinitions",
                schema: "workflow",
                newName: "workflow_definitions",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "ProductCategories",
                schema: "lookup",
                newName: "product_categories",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "MovementRequests",
                schema: "movement",
                newName: "movement_requests",
                newSchema: "movement");

            migrationBuilder.RenameTable(
                name: "MovementRequestLines",
                schema: "movement",
                newName: "movement_request_lines",
                newSchema: "movement");

            migrationBuilder.RenameTable(
                name: "MovementApprovals",
                schema: "movement",
                newName: "movement_approvals",
                newSchema: "movement");

            migrationBuilder.RenameTable(
                name: "inventory_tasks",
                schema: "piton",
                newName: "tasks",
                newSchema: "operation");

            migrationBuilder.RenameColumn(
                name: "DefaultSiteId",
                schema: "master",
                table: "workers",
                newName: "TenantId");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_UserId",
                schema: "master",
                table: "workers",
                newName: "IX_workers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_RegistrationNumber",
                schema: "master",
                table: "workers",
                newName: "IX_workers_RegistrationNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_ManagerId",
                schema: "master",
                table: "workers",
                newName: "IX_workers_ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_Workers_DepartmentId",
                schema: "master",
                table: "workers",
                newName: "IX_workers_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_Vehicles_PlateNumber",
                schema: "master",
                table: "vehicles",
                newName: "IX_vehicles_PlateNumber");

            migrationBuilder.RenameIndex(
                name: "IX_Products_Code",
                schema: "master",
                table: "products",
                newName: "IX_products_Code");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CategoryId",
                schema: "master",
                table: "products",
                newName: "IX_products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Departments_Code",
                schema: "lookup",
                table: "departments",
                newName: "IX_departments_Code");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "vehicle_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "VehicleId");

            migrationBuilder.RenameColumn(
                name: "released_at",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "ReleasedAt");

            migrationBuilder.RenameColumn(
                name: "last_modifier_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "last_modification_time",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "inventory_task_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "InventoryTaskId");

            migrationBuilder.RenameColumn(
                name: "deletion_time",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "DeletionTime");

            migrationBuilder.RenameColumn(
                name: "deleter_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "DeleterId");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "assigned_at",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "AssignedAt");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_vehicle_id_is_active",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_VehicleId_IsActive");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_inventory_task_id",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_InventoryTaskId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                schema: "inventory",
                table: "stock_locations",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "inventory",
                table: "stock_locations",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "reserved_quantity",
                schema: "inventory",
                table: "stock_locations",
                newName: "ReservedQuantity");

            migrationBuilder.RenameColumn(
                name: "product_id",
                schema: "inventory",
                table: "stock_locations",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "location_type",
                schema: "inventory",
                table: "stock_locations",
                newName: "LocationType");

            migrationBuilder.RenameColumn(
                name: "last_modifier_id",
                schema: "inventory",
                table: "stock_locations",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "last_modification_time",
                schema: "inventory",
                table: "stock_locations",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                schema: "inventory",
                table: "stock_locations",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "deletion_time",
                schema: "inventory",
                table: "stock_locations",
                newName: "DeletionTime");

            migrationBuilder.RenameColumn(
                name: "deleter_id",
                schema: "inventory",
                table: "stock_locations",
                newName: "DeleterId");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "inventory",
                table: "stock_locations",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "inventory",
                table: "stock_locations",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "warehouse_site_id",
                schema: "inventory",
                table: "stock_locations",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "quantity",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "Quantity");

            migrationBuilder.RenameColumn(
                name: "note",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "Note");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "transaction_type",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "TransactionType");

            migrationBuilder.RenameColumn(
                name: "target_location_type",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "TargetLocationType");

            migrationBuilder.RenameColumn(
                name: "source_location_type",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "SourceLocationType");

            migrationBuilder.RenameColumn(
                name: "product_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "occurred_at",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "OccurredAt");

            migrationBuilder.RenameColumn(
                name: "last_modifier_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "last_modification_time",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "deletion_time",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "DeletionTime");

            migrationBuilder.RenameColumn(
                name: "deleter_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "DeleterId");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "vehicle_task_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "TenantId");

            migrationBuilder.RenameColumn(
                name: "target_warehouse_site_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "TargetLocationId");

            migrationBuilder.RenameColumn(
                name: "target_vehicle_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "SourceLocationId");

            migrationBuilder.RenameColumn(
                name: "source_warehouse_site_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "RelatedTaskId");

            migrationBuilder.RenameColumn(
                name: "source_vehicle_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "RelatedMovementRequestId");

            migrationBuilder.RenameColumn(
                name: "movement_request_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "PerformedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_source_warehouse_site_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_RelatedTaskId");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_source_vehicle_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_RelatedMovementRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_product_id",
                schema: "inventory",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowStepDefinitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "workflow_step_definitions",
                newName: "IX_workflow_step_definitions_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowInstanceSteps_WorkflowStepDefinitionId",
                schema: "workflow",
                table: "workflow_instance_steps",
                newName: "IX_workflow_instance_steps_WorkflowStepDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowInstanceSteps_WorkflowInstanceId",
                schema: "workflow",
                table: "workflow_instance_steps",
                newName: "IX_workflow_instance_steps_WorkflowInstanceId");

            migrationBuilder.RenameIndex(
                name: "IX_WorkflowInstances_WorkflowDefinitionId",
                schema: "workflow",
                table: "workflow_instances",
                newName: "IX_workflow_instances_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_ParentId",
                schema: "lookup",
                table: "product_categories",
                newName: "IX_product_categories_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductCategories_Code",
                schema: "lookup",
                table: "product_categories",
                newName: "IX_product_categories_Code");

            migrationBuilder.RenameColumn(
                name: "TargetSiteId",
                schema: "movement",
                table: "movement_requests",
                newName: "SourceWarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequests_TargetSiteId",
                schema: "movement",
                table: "movement_requests",
                newName: "IX_movement_requests_SourceWarehouseId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequests_RequestNumber",
                schema: "movement",
                table: "movement_requests",
                newName: "IX_movement_requests_RequestNumber");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequests_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests",
                newName: "IX_movement_requests_RequestedVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequests_RequestedByWorkerId",
                schema: "movement",
                table: "movement_requests",
                newName: "IX_movement_requests_RequestedByWorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequestLines_ProductId",
                schema: "movement",
                table: "movement_request_lines",
                newName: "IX_movement_request_lines_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementRequestLines_MovementRequestId",
                schema: "movement",
                table: "movement_request_lines",
                newName: "IX_movement_request_lines_MovementRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementApprovals_MovementRequestId",
                schema: "movement",
                table: "movement_approvals",
                newName: "IX_movement_approvals_MovementRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_MovementApprovals_ApproverWorkerId",
                schema: "movement",
                table: "movement_approvals",
                newName: "IX_movement_approvals_ApproverWorkerId");

            migrationBuilder.RenameColumn(
                name: "status",
                schema: "operation",
                table: "tasks",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "region",
                schema: "operation",
                table: "tasks",
                newName: "Region");

            migrationBuilder.RenameColumn(
                name: "name",
                schema: "operation",
                table: "tasks",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                schema: "operation",
                table: "tasks",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                schema: "operation",
                table: "tasks",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "operation",
                table: "tasks",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "start_date",
                schema: "operation",
                table: "tasks",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "last_modifier_id",
                schema: "operation",
                table: "tasks",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "last_modification_time",
                schema: "operation",
                table: "tasks",
                newName: "LastModificationTime");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                schema: "operation",
                table: "tasks",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "is_active",
                schema: "operation",
                table: "tasks",
                newName: "IsActive");

            migrationBuilder.RenameColumn(
                name: "extra_properties",
                schema: "operation",
                table: "tasks",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "end_date",
                schema: "operation",
                table: "tasks",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "deletion_time",
                schema: "operation",
                table: "tasks",
                newName: "DeletionTime");

            migrationBuilder.RenameColumn(
                name: "deleter_id",
                schema: "operation",
                table: "tasks",
                newName: "DeleterId");

            migrationBuilder.RenameColumn(
                name: "creator_id",
                schema: "operation",
                table: "tasks",
                newName: "CreatorId");

            migrationBuilder.RenameColumn(
                name: "creation_time",
                schema: "operation",
                table: "tasks",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "concurrency_stamp",
                schema: "operation",
                table: "tasks",
                newName: "ConcurrencyStamp");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_tasks_code",
                schema: "operation",
                table: "tasks",
                newName: "IX_tasks_Code");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "workers",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultWarehouseId",
                schema: "master",
                table: "workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "master",
                table: "workers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "vehicles",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "master",
                table: "vehicles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "master",
                table: "vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "products",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "master",
                table: "products",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "master",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "lookup",
                table: "departments",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "lookup",
                table: "departments",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "lookup",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "operation",
                table: "vehicle_tasks",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "DriverWorkerId",
                schema: "operation",
                table: "vehicle_tasks",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "operation",
                table: "vehicle_tasks",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "operation",
                table: "vehicle_tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "inventory",
                table: "stock_locations",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "inventory",
                table: "stock_locations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "LocationId",
                schema: "inventory",
                table: "stock_locations",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "inventory",
                table: "inventory_transactions",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "inventory",
                table: "inventory_transactions",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                schema: "lookup",
                table: "product_categories",
                type: "character varying(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                schema: "lookup",
                table: "product_categories",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "lookup",
                table: "product_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetWarehouseId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "movement",
                table: "movement_approvals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReturnWarehouseId",
                schema: "operation",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TenantId",
                schema: "operation",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_workers",
                schema: "master",
                table: "workers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vehicles",
                schema: "master",
                table: "vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                schema: "master",
                table: "products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_departments",
                schema: "lookup",
                table: "departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workflow_step_definitions",
                schema: "workflow",
                table: "workflow_step_definitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workflow_instance_steps",
                schema: "workflow",
                table: "workflow_instance_steps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workflow_instances",
                schema: "workflow",
                table: "workflow_instances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_workflow_definitions",
                schema: "workflow",
                table: "workflow_definitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_product_categories",
                schema: "lookup",
                table: "product_categories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_movement_requests",
                schema: "movement",
                table: "movement_requests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_movement_request_lines",
                schema: "movement",
                table: "movement_request_lines",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_movement_approvals",
                schema: "movement",
                table: "movement_approvals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tasks",
                schema: "operation",
                table: "tasks",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "warehouses",
                schema: "master",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ManagerWorkerId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    TenantId = table.Column<Guid>(type: "uuid", nullable: true),
                    ExtraProperties = table.Column<string>(type: "text", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_warehouses", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_workers_DefaultWarehouseId",
                schema: "master",
                table: "workers",
                column: "DefaultWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_ProductId",
                schema: "inventory",
                table: "stock_locations",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_TenantId_LocationType_LocationId_ProductId",
                schema: "inventory",
                table: "stock_locations",
                columns: new[] { "TenantId", "LocationType", "LocationId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests",
                column: "TargetWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_warehouses_Code",
                schema: "master",
                table: "warehouses",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_movement_requests_RelatedMovementReq~",
                schema: "inventory",
                table: "inventory_transactions",
                column: "RelatedMovementRequestId",
                principalSchema: "movement",
                principalTable: "movement_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_products_ProductId",
                schema: "inventory",
                table: "inventory_transactions",
                column: "ProductId",
                principalSchema: "master",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_vehicle_tasks_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions",
                column: "RelatedTaskId",
                principalSchema: "operation",
                principalTable: "vehicle_tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_approvals_movement_requests_MovementRequestId",
                schema: "movement",
                table: "movement_approvals",
                column: "MovementRequestId",
                principalSchema: "movement",
                principalTable: "movement_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_approvals_workers_ApproverWorkerId",
                schema: "movement",
                table: "movement_approvals",
                column: "ApproverWorkerId",
                principalSchema: "master",
                principalTable: "workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_request_lines_movement_requests_MovementRequestId",
                schema: "movement",
                table: "movement_request_lines",
                column: "MovementRequestId",
                principalSchema: "movement",
                principalTable: "movement_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_request_lines_products_ProductId",
                schema: "movement",
                table: "movement_request_lines",
                column: "ProductId",
                principalSchema: "master",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_vehicles_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests",
                column: "RequestedVehicleId",
                principalSchema: "master",
                principalTable: "vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_warehouses_SourceWarehouseId",
                schema: "movement",
                table: "movement_requests",
                column: "SourceWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_warehouses_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests",
                column: "TargetWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_workers_RequestedByWorkerId",
                schema: "movement",
                table: "movement_requests",
                column: "RequestedByWorkerId",
                principalSchema: "master",
                principalTable: "workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_product_categories_product_categories_ParentId",
                schema: "lookup",
                table: "product_categories",
                column: "ParentId",
                principalSchema: "lookup",
                principalTable: "product_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_products_product_categories_CategoryId",
                schema: "master",
                table: "products",
                column: "CategoryId",
                principalSchema: "lookup",
                principalTable: "product_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_locations_products_ProductId",
                schema: "inventory",
                table: "stock_locations",
                column: "ProductId",
                principalSchema: "master",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_tasks_InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "InventoryTaskId",
                principalSchema: "operation",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_vehicles_VehicleId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "VehicleId",
                principalSchema: "master",
                principalTable: "vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workers_departments_DepartmentId",
                schema: "master",
                table: "workers",
                column: "DepartmentId",
                principalSchema: "lookup",
                principalTable: "departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workers_warehouses_DefaultWarehouseId",
                schema: "master",
                table: "workers",
                column: "DefaultWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workers_workers_ManagerId",
                schema: "master",
                table: "workers",
                column: "ManagerId",
                principalSchema: "master",
                principalTable: "workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workflow_instance_steps_workflow_instances_WorkflowInstance~",
                schema: "workflow",
                table: "workflow_instance_steps",
                column: "WorkflowInstanceId",
                principalSchema: "workflow",
                principalTable: "workflow_instances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workflow_instance_steps_workflow_step_definitions_WorkflowS~",
                schema: "workflow",
                table: "workflow_instance_steps",
                column: "WorkflowStepDefinitionId",
                principalSchema: "workflow",
                principalTable: "workflow_step_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workflow_instances_workflow_definitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "workflow_instances",
                column: "WorkflowDefinitionId",
                principalSchema: "workflow",
                principalTable: "workflow_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_workflow_step_definitions_workflow_definitions_WorkflowDefi~",
                schema: "workflow",
                table: "workflow_step_definitions",
                column: "WorkflowDefinitionId",
                principalSchema: "workflow",
                principalTable: "workflow_definitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_movement_requests_RelatedMovementReq~",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_products_ProductId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_vehicle_tasks_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_approvals_movement_requests_MovementRequestId",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_approvals_workers_ApproverWorkerId",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_request_lines_movement_requests_MovementRequestId",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_request_lines_products_ProductId",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_vehicles_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_warehouses_SourceWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_warehouses_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_workers_RequestedByWorkerId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_product_categories_product_categories_ParentId",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropForeignKey(
                name: "FK_products_product_categories_CategoryId",
                schema: "master",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_stock_locations_products_ProductId",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_tasks_InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_vehicles_VehicleId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_workers_departments_DepartmentId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropForeignKey(
                name: "FK_workers_warehouses_DefaultWarehouseId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropForeignKey(
                name: "FK_workers_workers_ManagerId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropForeignKey(
                name: "FK_workflow_instance_steps_workflow_instances_WorkflowInstance~",
                schema: "workflow",
                table: "workflow_instance_steps");

            migrationBuilder.DropForeignKey(
                name: "FK_workflow_instance_steps_workflow_step_definitions_WorkflowS~",
                schema: "workflow",
                table: "workflow_instance_steps");

            migrationBuilder.DropForeignKey(
                name: "FK_workflow_instances_workflow_definitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "workflow_instances");

            migrationBuilder.DropForeignKey(
                name: "FK_workflow_step_definitions_workflow_definitions_WorkflowDefi~",
                schema: "workflow",
                table: "workflow_step_definitions");

            migrationBuilder.DropTable(
                name: "warehouses",
                schema: "master");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workers",
                schema: "master",
                table: "workers");

            migrationBuilder.DropIndex(
                name: "IX_workers_DefaultWarehouseId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vehicles",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                schema: "master",
                table: "products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_departments",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropIndex(
                name: "IX_stock_locations_ProductId",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropIndex(
                name: "IX_stock_locations_TenantId_LocationType_LocationId_ProductId",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workflow_step_definitions",
                schema: "workflow",
                table: "workflow_step_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workflow_instances",
                schema: "workflow",
                table: "workflow_instances");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workflow_instance_steps",
                schema: "workflow",
                table: "workflow_instance_steps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_workflow_definitions",
                schema: "workflow",
                table: "workflow_definitions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tasks",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_product_categories",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_movement_requests",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_movement_request_lines",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_movement_approvals",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "DefaultWarehouseId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "DriverWorkerId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "LocationId",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "ReturnWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "AssignedTaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TargetWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TenantId",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.EnsureSchema(
                name: "piton");

            migrationBuilder.EnsureSchema(
                name: "stock");

            migrationBuilder.RenameTable(
                name: "workers",
                schema: "master",
                newName: "Workers",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "vehicles",
                schema: "master",
                newName: "Vehicles",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "products",
                schema: "master",
                newName: "Products",
                newSchema: "master");

            migrationBuilder.RenameTable(
                name: "departments",
                schema: "lookup",
                newName: "Departments",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "vehicle_tasks",
                schema: "operation",
                newName: "vehicle_tasks",
                newSchema: "piton");

            migrationBuilder.RenameTable(
                name: "stock_locations",
                schema: "inventory",
                newName: "stock_locations",
                newSchema: "piton");

            migrationBuilder.RenameTable(
                name: "inventory_transactions",
                schema: "inventory",
                newName: "inventory_transactions",
                newSchema: "piton");

            migrationBuilder.RenameTable(
                name: "workflow_step_definitions",
                schema: "workflow",
                newName: "WorkflowStepDefinitions",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "workflow_instances",
                schema: "workflow",
                newName: "WorkflowInstances",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "workflow_instance_steps",
                schema: "workflow",
                newName: "WorkflowInstanceSteps",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "workflow_definitions",
                schema: "workflow",
                newName: "WorkflowDefinitions",
                newSchema: "workflow");

            migrationBuilder.RenameTable(
                name: "tasks",
                schema: "operation",
                newName: "inventory_tasks",
                newSchema: "piton");

            migrationBuilder.RenameTable(
                name: "product_categories",
                schema: "lookup",
                newName: "ProductCategories",
                newSchema: "lookup");

            migrationBuilder.RenameTable(
                name: "movement_requests",
                schema: "movement",
                newName: "MovementRequests",
                newSchema: "movement");

            migrationBuilder.RenameTable(
                name: "movement_request_lines",
                schema: "movement",
                newName: "MovementRequestLines",
                newSchema: "movement");

            migrationBuilder.RenameTable(
                name: "movement_approvals",
                schema: "movement",
                newName: "MovementApprovals",
                newSchema: "movement");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "master",
                table: "Workers",
                newName: "DefaultSiteId");

            migrationBuilder.RenameIndex(
                name: "IX_workers_UserId",
                schema: "master",
                table: "Workers",
                newName: "IX_Workers_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_workers_RegistrationNumber",
                schema: "master",
                table: "Workers",
                newName: "IX_Workers_RegistrationNumber");

            migrationBuilder.RenameIndex(
                name: "IX_workers_ManagerId",
                schema: "master",
                table: "Workers",
                newName: "IX_Workers_ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_workers_DepartmentId",
                schema: "master",
                table: "Workers",
                newName: "IX_Workers_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicles_PlateNumber",
                schema: "master",
                table: "Vehicles",
                newName: "IX_Vehicles_PlateNumber");

            migrationBuilder.RenameIndex(
                name: "IX_products_Code",
                schema: "master",
                table: "Products",
                newName: "IX_Products_Code");

            migrationBuilder.RenameIndex(
                name: "IX_products_CategoryId",
                schema: "master",
                table: "Products",
                newName: "IX_Products_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_departments_Code",
                schema: "lookup",
                table: "Departments",
                newName: "IX_Departments_Code");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "VehicleId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "vehicle_id");

            migrationBuilder.RenameColumn(
                name: "ReleasedAt",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "released_at");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "last_modifier_id");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "last_modification_time");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "InventoryTaskId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "inventory_task_id");

            migrationBuilder.RenameColumn(
                name: "DeletionTime",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "deletion_time");

            migrationBuilder.RenameColumn(
                name: "DeleterId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "deleter_id");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "creation_time");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "assigned_at");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_VehicleId_IsActive",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_vehicle_id_is_active");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_InventoryTaskId",
                schema: "piton",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_inventory_task_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "piton",
                table: "stock_locations",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "piton",
                table: "stock_locations",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ReservedQuantity",
                schema: "piton",
                table: "stock_locations",
                newName: "reserved_quantity");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                schema: "piton",
                table: "stock_locations",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "LocationType",
                schema: "piton",
                table: "stock_locations",
                newName: "location_type");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                schema: "piton",
                table: "stock_locations",
                newName: "last_modifier_id");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                schema: "piton",
                table: "stock_locations",
                newName: "last_modification_time");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "piton",
                table: "stock_locations",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "DeletionTime",
                schema: "piton",
                table: "stock_locations",
                newName: "deletion_time");

            migrationBuilder.RenameColumn(
                name: "DeleterId",
                schema: "piton",
                table: "stock_locations",
                newName: "deleter_id");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "piton",
                table: "stock_locations",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "piton",
                table: "stock_locations",
                newName: "creation_time");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "piton",
                table: "stock_locations",
                newName: "warehouse_site_id");

            migrationBuilder.RenameColumn(
                name: "Quantity",
                schema: "piton",
                table: "inventory_transactions",
                newName: "quantity");

            migrationBuilder.RenameColumn(
                name: "Note",
                schema: "piton",
                table: "inventory_transactions",
                newName: "note");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "piton",
                table: "inventory_transactions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "TransactionType",
                schema: "piton",
                table: "inventory_transactions",
                newName: "transaction_type");

            migrationBuilder.RenameColumn(
                name: "TargetLocationType",
                schema: "piton",
                table: "inventory_transactions",
                newName: "target_location_type");

            migrationBuilder.RenameColumn(
                name: "SourceLocationType",
                schema: "piton",
                table: "inventory_transactions",
                newName: "source_location_type");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "product_id");

            migrationBuilder.RenameColumn(
                name: "OccurredAt",
                schema: "piton",
                table: "inventory_transactions",
                newName: "occurred_at");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "last_modifier_id");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                schema: "piton",
                table: "inventory_transactions",
                newName: "last_modification_time");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "piton",
                table: "inventory_transactions",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "DeletionTime",
                schema: "piton",
                table: "inventory_transactions",
                newName: "deletion_time");

            migrationBuilder.RenameColumn(
                name: "DeleterId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "deleter_id");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "piton",
                table: "inventory_transactions",
                newName: "creation_time");

            migrationBuilder.RenameColumn(
                name: "TenantId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "vehicle_task_id");

            migrationBuilder.RenameColumn(
                name: "TargetLocationId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "target_warehouse_site_id");

            migrationBuilder.RenameColumn(
                name: "SourceLocationId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "target_vehicle_id");

            migrationBuilder.RenameColumn(
                name: "RelatedTaskId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "source_warehouse_site_id");

            migrationBuilder.RenameColumn(
                name: "RelatedMovementRequestId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "source_vehicle_id");

            migrationBuilder.RenameColumn(
                name: "PerformedByUserId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "movement_request_id");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_RelatedTaskId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_source_warehouse_site_id");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_RelatedMovementRequestId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_source_vehicle_id");

            migrationBuilder.RenameIndex(
                name: "IX_inventory_transactions_ProductId",
                schema: "piton",
                table: "inventory_transactions",
                newName: "IX_inventory_transactions_product_id");

            migrationBuilder.RenameIndex(
                name: "IX_workflow_step_definitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "WorkflowStepDefinitions",
                newName: "IX_WorkflowStepDefinitions_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_workflow_instances_WorkflowDefinitionId",
                schema: "workflow",
                table: "WorkflowInstances",
                newName: "IX_WorkflowInstances_WorkflowDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_workflow_instance_steps_WorkflowStepDefinitionId",
                schema: "workflow",
                table: "WorkflowInstanceSteps",
                newName: "IX_WorkflowInstanceSteps_WorkflowStepDefinitionId");

            migrationBuilder.RenameIndex(
                name: "IX_workflow_instance_steps_WorkflowInstanceId",
                schema: "workflow",
                table: "WorkflowInstanceSteps",
                newName: "IX_WorkflowInstanceSteps_WorkflowInstanceId");

            migrationBuilder.RenameColumn(
                name: "Status",
                schema: "piton",
                table: "inventory_tasks",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Region",
                schema: "piton",
                table: "inventory_tasks",
                newName: "region");

            migrationBuilder.RenameColumn(
                name: "Name",
                schema: "piton",
                table: "inventory_tasks",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                schema: "piton",
                table: "inventory_tasks",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                schema: "piton",
                table: "inventory_tasks",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "piton",
                table: "inventory_tasks",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                schema: "piton",
                table: "inventory_tasks",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                schema: "piton",
                table: "inventory_tasks",
                newName: "last_modifier_id");

            migrationBuilder.RenameColumn(
                name: "LastModificationTime",
                schema: "piton",
                table: "inventory_tasks",
                newName: "last_modification_time");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "piton",
                table: "inventory_tasks",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IsActive",
                schema: "piton",
                table: "inventory_tasks",
                newName: "is_active");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                schema: "piton",
                table: "inventory_tasks",
                newName: "extra_properties");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                schema: "piton",
                table: "inventory_tasks",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "DeletionTime",
                schema: "piton",
                table: "inventory_tasks",
                newName: "deletion_time");

            migrationBuilder.RenameColumn(
                name: "DeleterId",
                schema: "piton",
                table: "inventory_tasks",
                newName: "deleter_id");

            migrationBuilder.RenameColumn(
                name: "CreatorId",
                schema: "piton",
                table: "inventory_tasks",
                newName: "creator_id");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                schema: "piton",
                table: "inventory_tasks",
                newName: "creation_time");

            migrationBuilder.RenameColumn(
                name: "ConcurrencyStamp",
                schema: "piton",
                table: "inventory_tasks",
                newName: "concurrency_stamp");

            migrationBuilder.RenameIndex(
                name: "IX_tasks_Code",
                schema: "piton",
                table: "inventory_tasks",
                newName: "IX_inventory_tasks_code");

            migrationBuilder.RenameIndex(
                name: "IX_product_categories_ParentId",
                schema: "lookup",
                table: "ProductCategories",
                newName: "IX_ProductCategories_ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_product_categories_Code",
                schema: "lookup",
                table: "ProductCategories",
                newName: "IX_ProductCategories_Code");

            migrationBuilder.RenameColumn(
                name: "SourceWarehouseId",
                schema: "movement",
                table: "MovementRequests",
                newName: "TargetSiteId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_requests_SourceWarehouseId",
                schema: "movement",
                table: "MovementRequests",
                newName: "IX_MovementRequests_TargetSiteId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_requests_RequestNumber",
                schema: "movement",
                table: "MovementRequests",
                newName: "IX_MovementRequests_RequestNumber");

            migrationBuilder.RenameIndex(
                name: "IX_movement_requests_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests",
                newName: "IX_MovementRequests_RequestedVehicleId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_requests_RequestedByWorkerId",
                schema: "movement",
                table: "MovementRequests",
                newName: "IX_MovementRequests_RequestedByWorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_request_lines_ProductId",
                schema: "movement",
                table: "MovementRequestLines",
                newName: "IX_MovementRequestLines_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_request_lines_MovementRequestId",
                schema: "movement",
                table: "MovementRequestLines",
                newName: "IX_MovementRequestLines_MovementRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_approvals_MovementRequestId",
                schema: "movement",
                table: "MovementApprovals",
                newName: "IX_MovementApprovals_MovementRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_movement_approvals_ApproverWorkerId",
                schema: "movement",
                table: "MovementApprovals",
                newName: "IX_MovementApprovals_ApproverWorkerId");

            migrationBuilder.AddColumn<Guid>(
                name: "vehicle_id",
                schema: "piton",
                table: "stock_locations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SourceSiteId",
                schema: "movement",
                table: "MovementRequests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Workers",
                schema: "master",
                table: "Workers",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vehicles",
                schema: "master",
                table: "Vehicles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                schema: "master",
                table: "Products",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Departments",
                schema: "lookup",
                table: "Departments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowStepDefinitions",
                schema: "workflow",
                table: "WorkflowStepDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowInstances",
                schema: "workflow",
                table: "WorkflowInstances",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowInstanceSteps",
                schema: "workflow",
                table: "WorkflowInstanceSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_WorkflowDefinitions",
                schema: "workflow",
                table: "WorkflowDefinitions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventory_tasks",
                schema: "piton",
                table: "inventory_tasks",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductCategories",
                schema: "lookup",
                table: "ProductCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovementRequests",
                schema: "movement",
                table: "MovementRequests",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovementRequestLines",
                schema: "movement",
                table: "MovementRequestLines",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MovementApprovals",
                schema: "movement",
                table: "MovementApprovals",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "Sites",
                schema: "master",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ManagerWorkerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SiteType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductStocks",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    ReservedQuantity = table.Column<int>(type: "integer", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false),
                    TotalQuantity = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductStocks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductStocks_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "master",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductStocks_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "master",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StockMovements",
                schema: "stock",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BalanceAfter = table.Column<int>(type: "integer", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeleterId = table.Column<Guid>(type: "uuid", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    LastModificationTime = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uuid", nullable: true),
                    MovementType = table.Column<int>(type: "integer", nullable: false),
                    Note = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    OccurredAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    ReferenceId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReferenceType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ReservedAfter = table.Column<int>(type: "integer", nullable: false),
                    SiteId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "master",
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Sites_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "master",
                        principalTable: "Sites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Workers_DefaultSiteId",
                schema: "master",
                table: "Workers",
                column: "DefaultSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_product_id_location_type_warehouse_site_id_~",
                schema: "piton",
                table: "stock_locations",
                columns: new[] { "product_id", "location_type", "warehouse_site_id", "vehicle_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_vehicle_id",
                schema: "piton",
                table: "stock_locations",
                column: "vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_locations_warehouse_site_id",
                schema: "piton",
                table: "stock_locations",
                column: "warehouse_site_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_movement_request_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "movement_request_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_target_vehicle_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "target_vehicle_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_target_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "target_warehouse_site_id");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_vehicle_task_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "vehicle_task_id");

            migrationBuilder.CreateIndex(
                name: "IX_MovementRequests_SourceSiteId",
                schema: "movement",
                table: "MovementRequests",
                column: "SourceSiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ProductId_SiteId",
                schema: "stock",
                table: "ProductStocks",
                columns: new[] { "ProductId", "SiteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_SiteId",
                schema: "stock",
                table: "ProductStocks",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Sites_Code",
                schema: "master",
                table: "Sites",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId",
                schema: "stock",
                table: "StockMovements",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SiteId",
                schema: "stock",
                table: "StockMovements",
                column: "SiteId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_MovementRequests_movement_request_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "movement_request_id",
                principalSchema: "movement",
                principalTable: "MovementRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_Products_product_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "product_id",
                principalSchema: "master",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_Sites_source_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "source_warehouse_site_id",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_Sites_target_warehouse_site_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "target_warehouse_site_id",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_Vehicles_source_vehicle_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "source_vehicle_id",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_Vehicles_target_vehicle_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "target_vehicle_id",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_vehicle_tasks_vehicle_task_id",
                schema: "piton",
                table: "inventory_transactions",
                column: "vehicle_task_id",
                principalSchema: "piton",
                principalTable: "vehicle_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementApprovals_MovementRequests_MovementRequestId",
                schema: "movement",
                table: "MovementApprovals",
                column: "MovementRequestId",
                principalSchema: "movement",
                principalTable: "MovementRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementApprovals_Workers_ApproverWorkerId",
                schema: "movement",
                table: "MovementApprovals",
                column: "ApproverWorkerId",
                principalSchema: "master",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequestLines_MovementRequests_MovementRequestId",
                schema: "movement",
                table: "MovementRequestLines",
                column: "MovementRequestId",
                principalSchema: "movement",
                principalTable: "MovementRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequestLines_Products_ProductId",
                schema: "movement",
                table: "MovementRequestLines",
                column: "ProductId",
                principalSchema: "master",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequests_Sites_SourceSiteId",
                schema: "movement",
                table: "MovementRequests",
                column: "SourceSiteId",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequests_Sites_TargetSiteId",
                schema: "movement",
                table: "MovementRequests",
                column: "TargetSiteId",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequests_Vehicles_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests",
                column: "RequestedVehicleId",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequests_Workers_RequestedByWorkerId",
                schema: "movement",
                table: "MovementRequests",
                column: "RequestedByWorkerId",
                principalSchema: "master",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCategories_ProductCategories_ParentId",
                schema: "lookup",
                table: "ProductCategories",
                column: "ParentId",
                principalSchema: "lookup",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_ProductCategories_CategoryId",
                schema: "master",
                table: "Products",
                column: "CategoryId",
                principalSchema: "lookup",
                principalTable: "ProductCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_locations_Products_product_id",
                schema: "piton",
                table: "stock_locations",
                column: "product_id",
                principalSchema: "master",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_locations_Sites_warehouse_site_id",
                schema: "piton",
                table: "stock_locations",
                column: "warehouse_site_id",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_stock_locations_Vehicles_vehicle_id",
                schema: "piton",
                table: "stock_locations",
                column: "vehicle_id",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_Vehicles_vehicle_id",
                schema: "piton",
                table: "vehicle_tasks",
                column: "vehicle_id",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_inventory_tasks_inventory_task_id",
                schema: "piton",
                table: "vehicle_tasks",
                column: "inventory_task_id",
                principalSchema: "piton",
                principalTable: "inventory_tasks",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Departments_DepartmentId",
                schema: "master",
                table: "Workers",
                column: "DepartmentId",
                principalSchema: "lookup",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Sites_DefaultSiteId",
                schema: "master",
                table: "Workers",
                column: "DefaultSiteId",
                principalSchema: "master",
                principalTable: "Sites",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Workers_Workers_ManagerId",
                schema: "master",
                table: "Workers",
                column: "ManagerId",
                principalSchema: "master",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstances_WorkflowDefinitions_WorkflowDefinitionId",
                schema: "workflow",
                table: "WorkflowInstances",
                column: "WorkflowDefinitionId",
                principalSchema: "workflow",
                principalTable: "WorkflowDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstanceSteps_WorkflowInstances_WorkflowInstanceId",
                schema: "workflow",
                table: "WorkflowInstanceSteps",
                column: "WorkflowInstanceId",
                principalSchema: "workflow",
                principalTable: "WorkflowInstances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowInstanceSteps_WorkflowStepDefinitions_WorkflowStepD~",
                schema: "workflow",
                table: "WorkflowInstanceSteps",
                column: "WorkflowStepDefinitionId",
                principalSchema: "workflow",
                principalTable: "WorkflowStepDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkflowStepDefinitions_WorkflowDefinitions_WorkflowDefinit~",
                schema: "workflow",
                table: "WorkflowStepDefinitions",
                column: "WorkflowDefinitionId",
                principalSchema: "workflow",
                principalTable: "WorkflowDefinitions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
