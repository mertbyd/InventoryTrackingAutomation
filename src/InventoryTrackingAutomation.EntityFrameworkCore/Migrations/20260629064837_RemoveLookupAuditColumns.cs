using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class RemoveLookupAuditColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                schema: "lookup",
                table: "product_categories");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                schema: "lookup",
                table: "departments");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                schema: "lookup",
                table: "departments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "lookup",
                table: "product_categories",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                schema: "lookup",
                table: "product_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "lookup",
                table: "product_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "lookup",
                table: "product_categories",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "lookup",
                table: "product_categories",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                schema: "lookup",
                table: "product_categories",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                schema: "lookup",
                table: "product_categories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                schema: "lookup",
                table: "departments",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                schema: "lookup",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                schema: "lookup",
                table: "departments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                schema: "lookup",
                table: "departments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                schema: "lookup",
                table: "departments",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                schema: "lookup",
                table: "departments",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                schema: "lookup",
                table: "departments",
                type: "uuid",
                nullable: true);
        }
    }
}
