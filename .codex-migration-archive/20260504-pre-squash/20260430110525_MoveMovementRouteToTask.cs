using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MoveMovementRouteToTask : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_movement_requests_tasks_TaskId",
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

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_SourceWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropIndex(
                name: "IX_movement_requests_TaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceWarehouseId",
                schema: "operation",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TargetWarehouseId",
                schema: "operation",
                table: "tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM operation.tasks)
                       AND NOT EXISTS (SELECT 1 FROM master.warehouses) THEN
                        RAISE EXCEPTION 'MoveMovementRouteToTask: SourceWarehouseId icin master.warehouses tablosunda kayit bulunamadi.';
                    END IF;
                END $$;

                UPDATE operation.tasks t
                SET
                    "SourceWarehouseId" = source."SourceWarehouseId",
                    "TargetWarehouseId" = source."TargetWarehouseId"
                FROM (
                    SELECT DISTINCT ON (m."TaskId")
                        m."TaskId",
                        m."SourceWarehouseId",
                        m."TargetWarehouseId"
                    FROM movement.movement_requests m
                    WHERE m."TaskId" IS NOT NULL
                    ORDER BY m."TaskId", m."CreationTime" DESC
                ) source
                WHERE source."TaskId" = t."Id";

                UPDATE operation.tasks t
                SET "SourceWarehouseId" = COALESCE(
                    t."SourceWarehouseId",
                    t."ReturnWarehouseId",
                    (SELECT w."Id" FROM master.warehouses w ORDER BY w."CreationTime", w."Id" LIMIT 1)
                )
                WHERE t."SourceWarehouseId" IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWarehouseId",
                schema: "operation",
                table: "tasks",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "SourceWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TargetWarehouseId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "TaskId",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_SourceWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_tasks_TargetWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "TargetWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_warehouses_SourceWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "SourceWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_warehouses_TargetWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "TargetWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_warehouses_SourceWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_tasks_warehouses_TargetWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_SourceWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_TargetWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "SourceWarehouseId",
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
                name: "TaskId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE movement.movement_requests m
                SET
                    "TaskId" = vt."TaskId",
                    "SourceWarehouseId" = t."SourceWarehouseId",
                    "TargetWarehouseId" = t."TargetWarehouseId"
                FROM operation.vehicle_tasks vt
                JOIN operation.tasks t ON t."Id" = vt."TaskId"
                WHERE vt."Id" = m."VehicleTaskId";

                DO $$
                BEGIN
                    IF EXISTS (
                        SELECT 1
                        FROM movement.movement_requests
                        WHERE "TaskId" IS NULL OR "SourceWarehouseId" IS NULL
                    ) THEN
                        RAISE EXCEPTION 'MoveMovementRouteToTask down: movement_requests icin TaskId veya SourceWarehouseId geri doldurulamadi.';
                    END IF;
                END $$;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "SourceWarehouseId",
                schema: "movement",
                table: "movement_requests",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
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

            migrationBuilder.DropColumn(
                name: "SourceWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "TargetWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_SourceWarehouseId",
                schema: "movement",
                table: "movement_requests",
                column: "SourceWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_TargetWarehouseId",
                schema: "movement",
                table: "movement_requests",
                column: "TargetWarehouseId");

            migrationBuilder.CreateIndex(
                name: "IX_movement_requests_TaskId",
                schema: "movement",
                table: "movement_requests",
                column: "TaskId");

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
        }
    }
}
