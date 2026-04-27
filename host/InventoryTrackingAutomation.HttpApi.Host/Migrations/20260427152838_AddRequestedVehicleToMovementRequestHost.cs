using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class AddRequestedVehicleToMovementRequestHost : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovementRequests_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests",
                column: "RequestedVehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_MovementRequests_Vehicles_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests",
                column: "RequestedVehicleId",
                principalSchema: "master",
                principalTable: "Vehicles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MovementRequests_Vehicles_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropIndex(
                name: "IX_MovementRequests_RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests");

            migrationBuilder.DropColumn(
                name: "RequestedVehicleId",
                schema: "movement",
                table: "MovementRequests");
        }
    }
}
