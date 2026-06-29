using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeVehicleTasksAndVehicleTaskLinesAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: VehicleTask ve VehicleTaskLine entity'lerini FullAuditedEntity seviyesinden AuditedEntity seviyesine ceker.
            // sistemdeki gorevi: Operasyonel child kayitlarda soft-delete kolonlarini kaldirip sureci FK ve domain kurallariyla korur.
            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "operation",
                table: "vehicle_tasks");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "TaskLineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Geri alma senaryosunda VehicleTask ve VehicleTaskLine icin soft-delete kolonlarini tekrar olusturur.
            // sistemdeki gorevi: Eski FullAuditedEntity sozlesmesine donuldugunde filtreli unique index davranisini geri getirir.
            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "operation",
                table: "vehicle_tasks",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "operation",
                table: "vehicle_tasks",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "operation",
                table: "vehicle_tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "operation",
                table: "vehicle_task_lines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "operation",
                table: "vehicle_task_lines",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "operation",
                table: "vehicle_task_lines",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "TaskLineId" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }
    }
}
