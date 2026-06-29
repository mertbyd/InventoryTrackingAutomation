using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeProductsAndVehiclesAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: Product ve Vehicle entity'lerini FullAuditedEntity seviyesinden AuditedEntity seviyesine ceker.
            // sistemdeki gorevi: Master verilerde silme audit kolonlarini kaldirip pasiflestirme modelini destekler.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "master",
                table: "products");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "master",
                table: "vehicles");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Geri alma senaryosunda Product ve Vehicle soft-delete kolonlarini tekrar olusturur.
            // sistemdeki gorevi: Migration rollback yapilirsa eski FullAuditedEntity tablo sozlesmesine donus saglar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "master",
                table: "products",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "master",
                table: "products",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "master",
                table: "products",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "master",
                table: "vehicles",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "master",
                table: "vehicles",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "master",
                table: "vehicles",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
