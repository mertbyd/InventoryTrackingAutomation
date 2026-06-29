using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeStockLocationsAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: StockLocation artik AuditedEntity oldugu icin soft-delete audit kolonlarini kaldirir.
            // sistemdeki gorevi: Guncellenebilir stok bakiyesi tablosunda silme yerine miktar guncelleme/stock movement akisini zorunlu kilar.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "inventory",
                table: "stock_locations");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "inventory",
                table: "stock_locations");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Rollback durumunda soft-delete kolonlarini geri ekler.
            // sistemdeki gorevi: Migration geri alinirsa onceki FullAuditedEntity semasina donusu mumkun kilar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "inventory",
                table: "stock_locations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "inventory",
                table: "stock_locations",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "inventory",
                table: "stock_locations",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
