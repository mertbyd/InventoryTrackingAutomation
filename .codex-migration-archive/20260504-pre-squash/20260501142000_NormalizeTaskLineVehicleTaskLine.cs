using System;
using InventoryTrackingAutomation.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(InventoryTrackingAutomationDbContext))]
    [Migration("20260501142000_NormalizeTaskLineVehicleTaskLine")]
    public partial class NormalizeTaskLineVehicleTaskLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_vehicle_task_lines_products_ProductId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_ProductId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_ProductId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropColumn(
                name: "ProductId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.DropColumn(
                name: "AllocatedQuantity",
                schema: "operation",
                table: "task_lines");

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
            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                type: "uuid",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.AddColumn<int>(
                name: "AllocatedQuantity",
                schema: "operation",
                table: "task_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                UPDATE operation.vehicle_task_lines AS vtl
                SET "ProductId" = tl."ProductId"
                FROM operation.task_lines AS tl
                WHERE vtl."TaskLineId" = tl."Id";
                """);

            migrationBuilder.Sql("""
                UPDATE operation.task_lines AS tl
                SET "AllocatedQuantity" = COALESCE(allocated."Quantity", 0)
                FROM (
                    SELECT "TaskLineId", SUM("AllocatedQuantity") AS "Quantity"
                    FROM operation.vehicle_task_lines
                    WHERE "IsDeleted" = FALSE
                    GROUP BY "TaskLineId"
                ) AS allocated
                WHERE tl."Id" = allocated."TaskLineId";
                """);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "ProductId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicle_task_lines_products_ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                column: "ProductId",
                principalSchema: "master",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
