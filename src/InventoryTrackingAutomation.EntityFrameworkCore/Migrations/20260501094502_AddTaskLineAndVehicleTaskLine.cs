using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskLineAndVehicleTaskLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "task_lines",
                schema: "operation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    AllocatedQuantity = table.Column<int>(type: "integer", nullable: false),
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
                    table.PrimaryKey("PK_task_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_task_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "master",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_task_lines_tasks_TaskId",
                        column: x => x.TaskId,
                        principalSchema: "operation",
                        principalTable: "tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_task_lines",
                schema: "operation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VehicleTaskId = table.Column<Guid>(type: "uuid", nullable: false),
                    TaskLineId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProductId = table.Column<Guid>(type: "uuid", nullable: false),
                    AllocatedQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReceivedQuantity = table.Column<int>(type: "integer", nullable: false),
                    DamagedQuantity = table.Column<int>(type: "integer", nullable: false),
                    LostQuantity = table.Column<int>(type: "integer", nullable: false),
                    ConsumedQuantity = table.Column<int>(type: "integer", nullable: false),
                    ReceiveNote = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
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
                    table.PrimaryKey("PK_vehicle_task_lines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_vehicle_task_lines_products_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "master",
                        principalTable: "products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_task_lines_task_lines_TaskLineId",
                        column: x => x.TaskLineId,
                        principalSchema: "operation",
                        principalTable: "task_lines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_vehicle_task_lines_vehicle_tasks_VehicleTaskId",
                        column: x => x.VehicleTaskId,
                        principalSchema: "operation",
                        principalTable: "vehicle_tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_ProductId",
                schema: "operation",
                table: "task_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines",
                columns: new[] { "TaskId", "ProductId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_TaskLineId",
                schema: "operation",
                table: "vehicle_task_lines",
                column: "TaskLineId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_task_lines_VehicleTaskId_ProductId",
                schema: "operation",
                table: "vehicle_task_lines",
                columns: new[] { "VehicleTaskId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "vehicle_task_lines",
                schema: "operation");

            migrationBuilder.DropTable(
                name: "task_lines",
                schema: "operation");
        }
    }
}
