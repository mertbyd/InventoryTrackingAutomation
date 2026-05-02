using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeTaskVehicleMovementFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_inventory_transactions_tasks_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_vehicles_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_tasks_InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_tasks_VehicleId_IsActive",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_inventory_transactions_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.RenameColumn(
                name: "InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "TaskId");

            migrationBuilder.RenameColumn(
                name: "DriverWorkerId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "ResponsibleWorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_TaskId");

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                schema: "operation",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "operation",
                table: "tasks",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleTaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests
                        WHERE "RequestedVehicleId" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'NormalizeTaskVehicleMovementFlow: movement_requests.RequestedVehicleId null olan kayit var; VehicleTask kurulamaz.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests
                        WHERE "Type" IN (2, 3)
                          AND "AssignedTaskId" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'NormalizeTaskVehicleMovementFlow: saha/iade hareketinde AssignedTaskId bos; TaskId turetilemez.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests m
                        WHERE m."AssignedTaskId" IS NOT NULL
                          AND NOT EXISTS (
                              SELECT 1
                              FROM operation.tasks t
                              WHERE t."Id" = m."AssignedTaskId"
                          )
                    ) THEN
                        RAISE EXCEPTION 'NormalizeTaskVehicleMovementFlow: AssignedTaskId operation.tasks tablosunda bulunamadi.';
                    END IF;
                END $$;
                """);

            migrationBuilder.Sql(
                """
                UPDATE operation.tasks
                SET "Type" = 2
                WHERE "Type" IS NULL;

                UPDATE operation.vehicle_tasks
                SET "ReleasedAt" = COALESCE("ReleasedAt", "LastModificationTime", "CreationTime")
                WHERE "IsActive" = false;
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO operation.tasks (
                    "Id", "Code", "Name", "Region", "StartDate", "EndDate", "Status", "Description",
                    "ReturnWarehouseId", "Type", "CreationTime", "CreatorId", "LastModificationTime",
                    "LastModifierId", "IsDeleted", "DeleterId", "DeletionTime")
                SELECT
                    m."Id",
                    CONCAT('WT-', LEFT(m."Id"::text, 47)),
                    LEFT(CONCAT('Warehouse Transfer ', m."RequestNumber"), 200),
                    NULL,
                    m."PlannedDate",
                    CASE WHEN m."Status" IN (5, 6, 7) THEN COALESCE(m."LastModificationTime", m."PlannedDate") ELSE NULL END,
                    CASE
                        WHEN m."Status" = 4 THEN 2
                        WHEN m."Status" = 5 THEN 3
                        WHEN m."Status" IN (6, 7) THEN 4
                        ELSE 1
                    END,
                    'Migrated from warehouse transfer movement request.',
                    NULL,
                    1,
                    m."CreationTime",
                    m."CreatorId",
                    m."LastModificationTime",
                    m."LastModifierId",
                    m."IsDeleted",
                    m."DeleterId",
                    m."DeletionTime"
                FROM movement.movement_requests m
                WHERE m."AssignedTaskId" IS NULL
                  AND NOT EXISTS (
                      SELECT 1
                      FROM operation.tasks t
                      WHERE t."Id" = m."Id"
                  );
                """);

            migrationBuilder.Sql(
                """
                INSERT INTO operation.vehicle_tasks (
                    "Id", "VehicleId", "TaskId", "ResponsibleWorkerId", "AssignedAt", "ReleasedAt",
                    "CreationTime", "CreatorId", "LastModificationTime", "LastModifierId",
                    "IsDeleted", "DeleterId", "DeletionTime")
                SELECT
                    source."MovementId",
                    source."VehicleId",
                    source."TaskId",
                    source."ResponsibleWorkerId",
                    source."AssignedAt",
                    source."ReleasedAt",
                    source."CreationTime",
                    source."CreatorId",
                    source."LastModificationTime",
                    source."LastModifierId",
                    source."IsDeleted",
                    source."DeleterId",
                    source."DeletionTime"
                FROM (
                    SELECT DISTINCT ON (COALESCE(m."AssignedTaskId", m."Id"), m."RequestedVehicleId")
                        m."Id" AS "MovementId",
                        m."RequestedVehicleId" AS "VehicleId",
                        COALESCE(m."AssignedTaskId", m."Id") AS "TaskId",
                        m."RequestedByWorkerId" AS "ResponsibleWorkerId",
                        m."CreationTime" AS "AssignedAt",
                        CASE WHEN m."Status" IN (5, 6, 7) THEN COALESCE(m."LastModificationTime", m."PlannedDate") ELSE NULL END AS "ReleasedAt",
                        m."CreationTime",
                        m."CreatorId",
                        m."LastModificationTime",
                        m."LastModifierId",
                        false AS "IsDeleted",
                        NULL::uuid AS "DeleterId",
                        NULL::timestamp without time zone AS "DeletionTime"
                    FROM movement.movement_requests m
                    WHERE m."RequestedVehicleId" IS NOT NULL
                    ORDER BY COALESCE(m."AssignedTaskId", m."Id"), m."RequestedVehicleId", m."CreationTime"
                ) source
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM operation.vehicle_tasks vt
                    WHERE vt."TaskId" = source."TaskId"
                      AND vt."VehicleId" = source."VehicleId"
                      AND vt."IsDeleted" = false
                );
                """);

            migrationBuilder.Sql(
                """
                UPDATE movement.movement_requests m
                SET
                    "TaskId" = COALESCE(m."AssignedTaskId", m."Id"),
                    "VehicleTaskId" = (
                        SELECT vt."Id"
                        FROM operation.vehicle_tasks vt
                        WHERE vt."TaskId" = COALESCE(m."AssignedTaskId", m."Id")
                          AND vt."VehicleId" = m."RequestedVehicleId"
                          AND vt."IsDeleted" = false
                        ORDER BY (vt."ReleasedAt" IS NULL) DESC, vt."AssignedAt" DESC, vt."CreationTime" DESC
                        LIMIT 1
                    );

                UPDATE movement.movement_requests r
                SET "ParentMovementRequestId" = (
                    SELECT p."Id"
                    FROM movement.movement_requests p
                    WHERE p."AssignedTaskId" = r."AssignedTaskId"
                      AND p."RequestedVehicleId" = r."RequestedVehicleId"
                      AND p."Type" = 2
                      AND p."Id" <> r."Id"
                    ORDER BY p."CreationTime" DESC
                    LIMIT 1
                )
                WHERE r."Type" = 3;
                """);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests
                        WHERE "TaskId" IS NULL OR "VehicleTaskId" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'NormalizeTaskVehicleMovementFlow: TaskId veya VehicleTaskId doldurulamadi.';
                    END IF;

                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests
                        WHERE "Type" = 3
                          AND "ParentMovementRequestId" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'NormalizeTaskVehicleMovementFlow: iade hareketi icin ana hareket bulunamadi.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                schema: "operation",
                table: "tasks",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "VehicleTaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "IsActive",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "RequestedVehicleId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "AssignedTaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tasks_ResponsibleWorkerId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "ResponsibleWorkerId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tasks_VehicleId_ReleasedAt",
                schema: "operation",
                table: "vehicle_tasks",
                columns: new[] { "VehicleId", "ReleasedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests",
                column: "ParentMovementRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_TaskId",
                schema: "movement",
                table: "movement_requests",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_VehicleTaskId",
                schema: "movement",
                table: "movement_requests",
                column: "VehicleTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_movement_requests_ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests",
                column: "ParentMovementRequestId",
                principalSchema: "movement",
                principalTable: "movement_requests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_tasks_TaskId",
                schema: "movement",
                table: "movement_requests",
                column: "TaskId",
                principalSchema: "operation",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_movement_requests_vehicle_tasks_VehicleTaskId",
                schema: "movement",
                table: "movement_requests",
                column: "VehicleTaskId",
                principalSchema: "operation",
                principalTable: "vehicle_tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_tasks_TaskId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "TaskId",
                principalSchema: "operation",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_tasks_workers_ResponsibleWorkerId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "ResponsibleWorkerId",
                principalSchema: "master",
                principalTable: "workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_movement_requests_ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_tasks_TaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_vehicle_tasks_VehicleTaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_tasks_TaskId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_tasks_workers_ResponsibleWorkerId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_tasks_ResponsibleWorkerId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_tasks_VehicleId_ReleasedAt",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_TaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_VehicleTaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "operation",
                table: "vehicle_tasks",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Region",
                schema: "operation",
                table: "tasks",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                schema: "operation",
                table: "tasks",
                type: "boolean",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<Guid>(
                name: "RequestedVehicleId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedTaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "movement",
                table: "movement_requests",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<Guid>(
                name: "RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE movement.movement_requests m
                SET
                    "RequestedVehicleId" = vt."VehicleId",
                    "AssignedTaskId" = CASE WHEN t."Type" = 2 THEN m."TaskId" ELSE NULL END,
                    "Type" = CASE
                        WHEN m."ParentMovementRequestId" IS NOT NULL THEN 3
                        WHEN t."Type" = 2 THEN 2
                        ELSE 1
                    END
                FROM operation.tasks t, operation.vehicle_tasks vt
                WHERE t."Id" = m."TaskId"
                  AND vt."Id" = m."VehicleTaskId";

                UPDATE operation.vehicle_tasks
                SET "IsActive" = "ReleasedAt" IS NULL;

                UPDATE operation.tasks
                SET "IsActive" = "Status" IN (1, 2);
                """);

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "ParentMovementRequestId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "VehicleTaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.RenameColumn(
                name: "TaskId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "InventoryTaskId");

            migrationBuilder.RenameColumn(
                name: "ResponsibleWorkerId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "DriverWorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_vehicle_tasks_TaskId",
                schema: "operation",
                table: "vehicle_tasks",
                newName: "IX_vehicle_tasks_InventoryTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_tasks_VehicleId_IsActive",
                schema: "operation",
                table: "vehicle_tasks",
                columns: new[] { "VehicleId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_RequestedVehicleId",
                schema: "movement",
                table: "movement_requests",
                column: "RequestedVehicleId");

            migrationBuilder.CreateIndex(
                name: "IX_inventory_transactions_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions",
                column: "RelatedTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_inventory_transactions_tasks_RelatedTaskId",
                schema: "inventory",
                table: "inventory_transactions",
                column: "RelatedTaskId",
                principalSchema: "operation",
                principalTable: "tasks",
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
                name: "FK_vehicle_tasks_tasks_InventoryTaskId",
                schema: "operation",
                table: "vehicle_tasks",
                column: "InventoryTaskId",
                principalSchema: "operation",
                principalTable: "tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
