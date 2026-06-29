using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeWorkersAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: Worker entity'sini FullAuditedEntity seviyesinden AuditedEntity seviyesine ceker.
            // sistemdeki gorevi: Master calisan verisinde silme audit kolonlarini kaldirip pasiflestirme modelini destekler.
            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "master",
                table: "workers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Geri alma senaryosunda Worker soft-delete kolonlarini tekrar olusturur.
            // sistemdeki gorevi: Migration rollback yapilirsa eski FullAuditedEntity tablo sozlesmesine donus saglar.
            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "master",
                table: "workers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "master",
                table: "workers",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "master",
                table: "workers",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
