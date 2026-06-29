using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeMovementApprovalsCreationAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: MovementApproval artik CreationAuditedEntity oldugu icin mutasyon/soft-delete audit kolonlarini kaldirir.
            // sistemdeki gorevi: Onay/red karar izini append-only gecmis kaydi olarak hafifletir.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                schema: "movement",
                table: "movement_approvals");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                schema: "movement",
                table: "movement_approvals");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Rollback durumunda FullAuditedEntity kolonlarini geri ekler.
            // sistemdeki gorevi: Migration geri alinirsa onceki audit semasina donusu mumkun kilar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "movement",
                table: "movement_approvals",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "movement",
                table: "movement_approvals",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "movement",
                table: "movement_approvals",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                schema: "movement",
                table: "movement_approvals",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                schema: "movement",
                table: "movement_approvals",
                type: "uuid",
                nullable: true);
        }
    }
}
