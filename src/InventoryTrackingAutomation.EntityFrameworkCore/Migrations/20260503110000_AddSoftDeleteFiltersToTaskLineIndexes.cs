using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteFiltersToTaskLineIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines",
                columns: new[] { "TaskId", "ProductId" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "TaskLineId" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.DropIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines");

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines",
                columns: new[] { "TaskId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "TaskLineId" },
                unique: true);
        }
    }
}
