using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryTrackingAutomation.Migrations
{
    /// <inheritdoc />
    public partial class Optimization_Lookups_SoftDelete : Migration
    {
        private static readonly Guid PieceUnitTypeId = new("10000000-0000-0000-0000-000000000001");
        private static readonly Guid BoxUnitTypeId = new("10000000-0000-0000-0000-000000000002");
        private static readonly Guid KilogramUnitTypeId = new("10000000-0000-0000-0000-000000000003");
        private static readonly Guid MeterUnitTypeId = new("10000000-0000-0000-0000-000000000004");
        private static readonly Guid LiterUnitTypeId = new("10000000-0000-0000-0000-000000000005");

        private static readonly Guid TruckVehicleTypeId = new("20000000-0000-0000-0000-000000000001");
        private static readonly Guid VanVehicleTypeId = new("20000000-0000-0000-0000-000000000002");
        private static readonly Guid CarVehicleTypeId = new("20000000-0000-0000-0000-000000000003");

        private static readonly Guid WhiteCollarWorkerTypeId = new("30000000-0000-0000-0000-000000000001");
        private static readonly Guid BlueCollarWorkerTypeId = new("30000000-0000-0000-0000-000000000002");
        private static readonly Guid SubcontractorWorkerTypeId = new("30000000-0000-0000-0000-000000000003");

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // islevi: UnitType, VehicleType ve WorkerType enum kolonlarini lookup FK modeline tasir.
            // sistemdeki gorevi: Enum degerlerini DB tarafinda referans tabloya baglayarak iliski ve yonetilebilirlik saglar.
            migrationBuilder.CreateTable(
                name: "unit_types",
                schema: "lookup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_unit_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "vehicle_types",
                schema: "lookup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_vehicle_types", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "worker_types",
                schema: "lookup",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Code = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_worker_types", x => x.Id);
                });

            // islevi: Yeni lookup tablolarina sabit baslangic degerlerini ekler.
            // sistemdeki gorevi: InsertData metadata bagimliligi yerine acik SQL ile PostgreSQL migration zincirini guvenilir calistirir.
            migrationBuilder.Sql($"""
                INSERT INTO lookup.unit_types ("Id", "Code", "Name", "Description", "IsActive") VALUES
                    ('{PieceUnitTypeId:D}', 'PIECE', 'Adet', NULL, TRUE),
                    ('{BoxUnitTypeId:D}', 'BOX', 'Kutu', NULL, TRUE),
                    ('{KilogramUnitTypeId:D}', 'KILOGRAM', 'Kilogram', NULL, TRUE),
                    ('{MeterUnitTypeId:D}', 'METER', 'Metre', NULL, TRUE),
                    ('{LiterUnitTypeId:D}', 'LITER', 'Litre', NULL, TRUE);
                """);

            migrationBuilder.Sql($"""
                INSERT INTO lookup.vehicle_types ("Id", "Code", "Name", "Description", "IsActive") VALUES
                    ('{TruckVehicleTypeId:D}', 'TRUCK', 'Kamyon', NULL, TRUE),
                    ('{VanVehicleTypeId:D}', 'VAN', 'Panelvan', NULL, TRUE),
                    ('{CarVehicleTypeId:D}', 'CAR', 'Otomobil', NULL, TRUE);
                """);

            migrationBuilder.Sql($"""
                INSERT INTO lookup.worker_types ("Id", "Code", "Name", "Description", "IsActive") VALUES
                    ('{WhiteCollarWorkerTypeId:D}', 'WHITE_COLLAR', 'Beyaz Yaka', NULL, TRUE),
                    ('{BlueCollarWorkerTypeId:D}', 'BLUE_COLLAR', 'Mavi Yaka', NULL, TRUE),
                    ('{SubcontractorWorkerTypeId:D}', 'SUBCONTRACTOR', 'Taseron', NULL, TRUE);
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "WorkerTypeId",
                schema: "master",
                table: "workers",
                type: "uuid",
                nullable: false,
                defaultValue: WhiteCollarWorkerTypeId);

            migrationBuilder.AddColumn<Guid>(
                name: "VehicleTypeId",
                schema: "master",
                table: "vehicles",
                type: "uuid",
                nullable: false,
                defaultValue: VanVehicleTypeId);

            migrationBuilder.AddColumn<Guid>(
                name: "UnitTypeId",
                schema: "master",
                table: "products",
                type: "uuid",
                nullable: false,
                defaultValue: PieceUnitTypeId);

            migrationBuilder.Sql("""
                UPDATE master.products
                SET "UnitTypeId" = CASE "BaseUnit"
                    WHEN 1 THEN '10000000-0000-0000-0000-000000000001'::uuid
                    WHEN 2 THEN '10000000-0000-0000-0000-000000000002'::uuid
                    WHEN 3 THEN '10000000-0000-0000-0000-000000000003'::uuid
                    WHEN 4 THEN '10000000-0000-0000-0000-000000000004'::uuid
                    WHEN 5 THEN '10000000-0000-0000-0000-000000000005'::uuid
                    ELSE '10000000-0000-0000-0000-000000000001'::uuid
                END;
                """);

            migrationBuilder.Sql("""
                UPDATE master.vehicles
                SET "VehicleTypeId" = CASE "VehicleType"
                    WHEN 1 THEN '20000000-0000-0000-0000-000000000001'::uuid
                    WHEN 2 THEN '20000000-0000-0000-0000-000000000002'::uuid
                    WHEN 3 THEN '20000000-0000-0000-0000-000000000003'::uuid
                    ELSE '20000000-0000-0000-0000-000000000002'::uuid
                END;
                """);

            migrationBuilder.Sql("""
                UPDATE master.workers
                SET "WorkerTypeId" = CASE "WorkerType"
                    WHEN 1 THEN '30000000-0000-0000-0000-000000000001'::uuid
                    WHEN 2 THEN '30000000-0000-0000-0000-000000000002'::uuid
                    WHEN 3 THEN '30000000-0000-0000-0000-000000000003'::uuid
                    ELSE '30000000-0000-0000-0000-000000000001'::uuid
                END;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_workers_WorkerTypeId",
                schema: "master",
                table: "workers",
                column: "WorkerTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_vehicles_VehicleTypeId",
                schema: "master",
                table: "vehicles",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_products_UnitTypeId",
                schema: "master",
                table: "products",
                column: "UnitTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_unit_types_Code",
                schema: "lookup",
                table: "unit_types",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_vehicle_types_Code",
                schema: "lookup",
                table: "vehicle_types",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_worker_types_Code",
                schema: "lookup",
                table: "worker_types",
                column: "Code",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_products_unit_types_UnitTypeId",
                schema: "master",
                table: "products",
                column: "UnitTypeId",
                principalSchema: "lookup",
                principalTable: "unit_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_vehicles_vehicle_types_VehicleTypeId",
                schema: "master",
                table: "vehicles",
                column: "VehicleTypeId",
                principalSchema: "lookup",
                principalTable: "vehicle_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_workers_worker_types_WorkerTypeId",
                schema: "master",
                table: "workers",
                column: "WorkerTypeId",
                principalSchema: "lookup",
                principalTable: "worker_types",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.DropColumn(
                name: "WorkerType",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "VehicleType",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "BaseUnit",
                schema: "master",
                table: "products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // islevi: Lookup FK modelini eski enum kolonlarina geri alir.
            // sistemdeki gorevi: Rollback halinde mevcut FK degerlerini enum integer karsiliklarina donusturur.
            migrationBuilder.DropForeignKey(
                name: "FK_products_unit_types_UnitTypeId",
                schema: "master",
                table: "products");

            migrationBuilder.DropForeignKey(
                name: "FK_vehicles_vehicle_types_VehicleTypeId",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropForeignKey(
                name: "FK_workers_worker_types_WorkerTypeId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropIndex(
                name: "IX_workers_WorkerTypeId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropIndex(
                name: "IX_vehicles_VehicleTypeId",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropIndex(
                name: "IX_products_UnitTypeId",
                schema: "master",
                table: "products");

            migrationBuilder.AddColumn<int>(
                name: "WorkerType",
                schema: "master",
                table: "workers",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<int>(
                name: "VehicleType",
                schema: "master",
                table: "vehicles",
                type: "integer",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.AddColumn<int>(
                name: "BaseUnit",
                schema: "master",
                table: "products",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.Sql("""
                UPDATE master.products
                SET "BaseUnit" = CASE "UnitTypeId"
                    WHEN '10000000-0000-0000-0000-000000000001'::uuid THEN 1
                    WHEN '10000000-0000-0000-0000-000000000002'::uuid THEN 2
                    WHEN '10000000-0000-0000-0000-000000000003'::uuid THEN 3
                    WHEN '10000000-0000-0000-0000-000000000004'::uuid THEN 4
                    WHEN '10000000-0000-0000-0000-000000000005'::uuid THEN 5
                    ELSE 1
                END;
                """);

            migrationBuilder.Sql("""
                UPDATE master.vehicles
                SET "VehicleType" = CASE "VehicleTypeId"
                    WHEN '20000000-0000-0000-0000-000000000001'::uuid THEN 1
                    WHEN '20000000-0000-0000-0000-000000000002'::uuid THEN 2
                    WHEN '20000000-0000-0000-0000-000000000003'::uuid THEN 3
                    ELSE 2
                END;
                """);

            migrationBuilder.Sql("""
                UPDATE master.workers
                SET "WorkerType" = CASE "WorkerTypeId"
                    WHEN '30000000-0000-0000-0000-000000000001'::uuid THEN 1
                    WHEN '30000000-0000-0000-0000-000000000002'::uuid THEN 2
                    WHEN '30000000-0000-0000-0000-000000000003'::uuid THEN 3
                    ELSE 1
                END;
                """);

            migrationBuilder.DropColumn(
                name: "WorkerTypeId",
                schema: "master",
                table: "workers");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                schema: "master",
                table: "vehicles");

            migrationBuilder.DropColumn(
                name: "UnitTypeId",
                schema: "master",
                table: "products");

            migrationBuilder.DropTable(
                name: "unit_types",
                schema: "lookup");

            migrationBuilder.DropTable(
                name: "vehicle_types",
                schema: "lookup");

            migrationBuilder.DropTable(
                name: "worker_types",
                schema: "lookup");
        }
    }
}
