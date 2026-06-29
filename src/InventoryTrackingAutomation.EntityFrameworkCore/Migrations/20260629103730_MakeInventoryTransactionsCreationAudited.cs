using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeInventoryTransactionsCreationAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: InventoryTransaction artik CreationAuditedEntity oldugu icin mutasyon/soft-delete audit kolonlarini kaldirir.
            // sistemdeki gorevi: Append-only stok defteri tablosunda gereksiz audit kolon yukunu azaltir.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                schema: "inventory",
                table: "inventory_transactions");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                schema: "inventory",
                table: "inventory_transactions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Rollback durumunda FullAuditedEntity kolonlarini geri ekler.
            // sistemdeki gorevi: Migration geri alinirsa onceki audit semasina donusu mumkun kilar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "inventory",
                table: "inventory_transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "inventory",
                table: "inventory_transactions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "inventory",
                table: "inventory_transactions",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                schema: "inventory",
                table: "inventory_transactions",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                schema: "inventory",
                table: "inventory_transactions",
                type: "uuid",
                nullable: true);
        }
    }
}
