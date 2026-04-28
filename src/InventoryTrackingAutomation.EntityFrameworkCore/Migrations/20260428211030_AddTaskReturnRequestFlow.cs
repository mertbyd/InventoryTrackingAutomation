using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskReturnRequestFlow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Type",
                schema: "movement",
                table: "movement_requests",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql(
                "UPDATE movement.movement_requests SET \"Type\" = 2 WHERE \"AssignedTaskId\" IS NOT NULL;");

            migrationBuilder.AddColumn<int>(
                name: "ConsumedQuantity",
                schema: "movement",
                table: "movement_request_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DamagedQuantity",
                schema: "movement",
                table: "movement_request_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LostQuantity",
                schema: "movement",
                table: "movement_request_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ReceiveNote",
                schema: "movement",
                table: "movement_request_lines",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReceivedQuantity",
                schema: "movement",
                table: "movement_request_lines",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_tasks_ReturnWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "ReturnWarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_tasks_warehouses_ReturnWarehouseId",
                schema: "operation",
                table: "tasks",
                column: "ReturnWarehouseId",
                principalSchema: "master",
                principalTable: "warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_tasks_warehouses_ReturnWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropIndex(
                name: "IX_tasks_ReturnWarehouseId",
                schema: "operation",
                table: "tasks");

            migrationBuilder.DropColumn(
                name: "Type",
                schema: "movement",
                table: "movement_requests");

            migrationBuilder.DropColumn(
                name: "ConsumedQuantity",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropColumn(
                name: "DamagedQuantity",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropColumn(
                name: "LostQuantity",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropColumn(
                name: "ReceiveNote",
                schema: "movement",
                table: "movement_request_lines");

            migrationBuilder.DropColumn(
                name: "ReceivedQuantity",
                schema: "movement",
                table: "movement_request_lines");
        }
    }
}
