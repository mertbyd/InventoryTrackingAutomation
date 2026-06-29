using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class MakeTaskLinesAudited : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: TaskLine entity'sini FullAuditedEntity seviyesinden AuditedEntity seviyesine ceker.
            // sistemdeki gorevi: Tahsis edilmemis taslak satirlar fiziksel silinecegi icin soft-delete kolonlarini ve filtreli unique indexi kaldirir.
            migrationBuilder.DropIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines",
                columns: new[] { "TaskId", "ProductId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Geri alma senaryosunda TaskLine soft-delete kolonlarini ve filtreli unique indexi tekrar olusturur.
            // sistemdeki gorevi: Migration rollback yapilirsa eski FullAuditedEntity tablo sozlesmesine donus saglar.
            migrationBuilder.DropIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines");

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "operation",
                table: "task_lines",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "operation",
                table: "task_lines",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "operation",
                table: "task_lines",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_task_lines_TaskId_ProductId",
                schema: "operation",
                table: "task_lines",
                columns: new[] { "TaskId", "ProductId" },
                unique: true,
                filter: "\"IsDeleted\" = FALSE");
        }
    }
}
