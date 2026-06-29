using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeWarehousesAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: Warehouse entity'sini FullAuditedEntity seviyesinden AuditedEntity seviyesine ceker.
            // sistemdeki gorevi: Master depo verisinde silme audit kolonlarini kaldirip pasiflestirme modelini destekler.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "master",
                table: "warehouses");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "master",
                table: "warehouses");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "master",
                table: "warehouses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Geri alma senaryosunda Warehouse soft-delete kolonlarini tekrar olusturur.
            // sistemdeki gorevi: Migration rollback yapilirsa eski FullAuditedEntity tablo sozlesmesine donus saglar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "master",
                table: "warehouses",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "master",
                table: "warehouses",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "master",
                table: "warehouses",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
