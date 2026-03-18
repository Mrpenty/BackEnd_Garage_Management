using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class updateInventory_addSupplierV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ModelCompatible",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "Inventories");

            migrationBuilder.AddColumn<int>(
                name: "JobCardId",
                table: "StockTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LotNumber",
                table: "StockTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReceiptCode",
                table: "StockTransactions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "StockTransactions",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SupplierId",
                table: "StockTransactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "StockTransactions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SparePartBrands",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedBy",
                table: "SparePartBrands",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SparePartBrands",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedBy",
                table: "SparePartBrands",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "SparePartBrands",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SparePartBrands",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedBy",
                table: "SparePartBrands",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MinQuantity",
                table: "Inventories",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartCode",
                table: "Inventories",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InventoryVehicleModels",
                columns: table => new
                {
                    SparePartId = table.Column<int>(type: "int", nullable: false),
                    VehicleModelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryVehicleModels", x => new { x.SparePartId, x.VehicleModelId });
                    table.ForeignKey(
                        name: "FK_InventoryVehicleModels_Inventories_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "Inventories",
                        principalColumn: "SparePartId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InventoryVehicleModels_VehicleModels_VehicleModelId",
                        column: x => x.VehicleModelId,
                        principalTable: "VehicleModels",
                        principalColumn: "ModelId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SparePartCategories",
                columns: table => new
                {
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CategoryName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartCategories", x => x.CategoryId);
                });

            migrationBuilder.CreateTable(
                name: "Suppliers",
                columns: table => new
                {
                    SupplierId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SupplierName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SupplierType = table.Column<int>(type: "int", nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TaxCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suppliers", x => x.SupplierId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2672afe95d9849bc9e4c1cae34a73e55", new DateTime(2026, 3, 18, 1, 28, 58, 194, DateTimeKind.Utc).AddTicks(4271), "AQAAAAIAAYagAAAAEPc8SMy4YwRkNsit9Kl1bLTHk5VEumlL6NHnHMqw1a/qKirEz7s32XjgcR2UT+sghw==", "3e4cf77d-4f13-4bd7-8ddf-d3b2472ece14" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e10955eedfd49f4ab4f6563129f9146", new DateTime(2026, 3, 18, 1, 28, 58, 248, DateTimeKind.Utc).AddTicks(7416), "AQAAAAIAAYagAAAAEPN82Zx8O3qGkCKNk2PktAntt7WoGL2P/P4hu0lgv7Qvy4BUm9Z/6aIFzbVjTe21KA==", "ad7205f5-d926-4c96-b992-dda31f46f528" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a225e94285e04e54a29a011743028437", new DateTime(2026, 3, 18, 1, 28, 58, 311, DateTimeKind.Utc).AddTicks(7306), "AQAAAAIAAYagAAAAEC/JrF6yBP0pQ+Ab+yOn5i6bwYllqeLruaKo5D85zT/EweNKOXRTptO4iVyboWSC+w==", "e08570dc-7900-4981-b477-3f2c8ab1a7ad" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f5bdc9037603476a8313b31734c451d5", new DateTime(2026, 3, 18, 1, 28, 58, 374, DateTimeKind.Utc).AddTicks(9911), "AQAAAAIAAYagAAAAEGwHjNblSBPHyN6hqcVUeFxQ7SfRZPR5iFDCRFN6DiyTm/FBgWOt1qvTTpfhKj0KtQ==", "c7c36a07-fc67-43c8-acec-19362d96a889" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8226009056694d48a6646d717b3bfe1e", new DateTime(2026, 3, 18, 1, 28, 58, 435, DateTimeKind.Utc).AddTicks(5881), "AQAAAAIAAYagAAAAEINSMKnW7e+IX/IEZww5e/33IfocTAXUstxZRPYN4WWznYBWNQsCCsuQWt5rGIySdg==", "0c0de73e-e620-4beb-a988-d53f7bd39238" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e464aa0ece2244dfa3d0751e40df546b", new DateTime(2026, 3, 18, 1, 28, 58, 490, DateTimeKind.Utc).AddTicks(3977), "AQAAAAIAAYagAAAAEFAofGG1lMjR+ACw6QJAtFoECXMIst8fAStAc8oAEqrKtnfBH83EmRIxujCZ/+pZAQ==", "65bd95b9-0ff3-4e57-9d4b-e17b60933d0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "31b0973b6ccd4879b56140e62467ac70", new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6129), "AQAAAAIAAYagAAAAEEtCUXIoEkf9NaqjtPsUqRbCOVhG0NnjAUO3D7l26TDs4GCZ/d3ga9vvhACq2sWS8A==", "e5388d4d-ca86-4e57-81fa-4487b2029056" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6735));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6675));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6679));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6682));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6685));

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_JobCardId",
                table: "StockTransactions",
                column: "JobCardId");

            migrationBuilder.CreateIndex(
                name: "IX_StockTransactions_SupplierId",
                table: "StockTransactions",
                column: "SupplierId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_CategoryId",
                table: "Inventories",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_PartCode",
                table: "Inventories",
                column: "PartCode",
                unique: true,
                filter: "[PartCode] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryVehicleModels_VehicleModelId",
                table: "InventoryVehicleModels",
                column: "VehicleModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_SparePartCategories_CategoryId",
                table: "Inventories",
                column: "CategoryId",
                principalTable: "SparePartCategories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_JobCards_JobCardId",
                table: "StockTransactions",
                column: "JobCardId",
                principalTable: "JobCards",
                principalColumn: "JobCardId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_StockTransactions_Suppliers_SupplierId",
                table: "StockTransactions",
                column: "SupplierId",
                principalTable: "Suppliers",
                principalColumn: "SupplierId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_SparePartCategories_CategoryId",
                table: "Inventories");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_JobCards_JobCardId",
                table: "StockTransactions");

            migrationBuilder.DropForeignKey(
                name: "FK_StockTransactions_Suppliers_SupplierId",
                table: "StockTransactions");

            migrationBuilder.DropTable(
                name: "InventoryVehicleModels");

            migrationBuilder.DropTable(
                name: "SparePartCategories");

            migrationBuilder.DropTable(
                name: "Suppliers");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_JobCardId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_StockTransactions_SupplierId",
                table: "StockTransactions");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_CategoryId",
                table: "Inventories");

            migrationBuilder.DropIndex(
                name: "IX_Inventories_PartCode",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "JobCardId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "LotNumber",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "ReceiptCode",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "SupplierId",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "StockTransactions");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "SparePartBrands");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "MinQuantity",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "PartCode",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Inventories");

            migrationBuilder.AddColumn<string>(
                name: "ModelCompatible",
                table: "Inventories",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                table: "Inventories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe16f5dadc2a426b85834e74f0f919e8", new DateTime(2026, 3, 18, 1, 22, 53, 14, DateTimeKind.Utc).AddTicks(7777), "AQAAAAIAAYagAAAAEKkC45mdcamlzNWkwOayFNyPqJUgd3wbRbjc/SDQFPhKz/tEFaDvY57n1A9A9XPVBw==", "852a1472-f9c6-40a8-a399-6df8d4df6a1e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "07473610bf3642e7832193d5a9f2dfbf", new DateTime(2026, 3, 18, 1, 22, 53, 68, DateTimeKind.Utc).AddTicks(5732), "AQAAAAIAAYagAAAAEMEzMm7hsmAvzB9TO4YM8NPH5Sj9m+UFl63+xXDijImEDVG5rS+Vi6//nGzYIFCMug==", "ca2ffc44-dbcb-4697-9330-40a5ae141c8d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "32fff1714c994a56bc86b143d09ee77e", new DateTime(2026, 3, 18, 1, 22, 53, 123, DateTimeKind.Utc).AddTicks(4066), "AQAAAAIAAYagAAAAEOMBSnMRk2ArGdJlwx/Dh+HEqBevrhQ37Sgy+lNGVsXORWfnOhXnvD8y1+bMb5VCOg==", "5907fde8-464a-4e2d-a17c-ed410e18b795" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ce08361dbd5447a687c447e9e4b90e71", new DateTime(2026, 3, 18, 1, 22, 53, 176, DateTimeKind.Utc).AddTicks(4639), "AQAAAAIAAYagAAAAELR6nANYTKboakCOAzHSznSxE7NqnUBVG+FPkzpL80jka37MG6SZimOdw0JXA8f27w==", "00cd4cc6-8f9b-4283-8b65-18e930245aec" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "06c0998d16724fc49a604eb4d656dd11", new DateTime(2026, 3, 18, 1, 22, 53, 230, DateTimeKind.Utc).AddTicks(4076), "AQAAAAIAAYagAAAAEKS11ykJcw7fZ9r1F8L2LnGZdIWUPlRkIxcxWLJGJFwd0hD+1Z5bdWG/Y8zPtA2Muw==", "b27a1bdb-39d5-45c6-bc87-b064873fd183" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "321e8dce4ebe4579947a3078c3e81d08", new DateTime(2026, 3, 18, 1, 22, 53, 284, DateTimeKind.Utc).AddTicks(1766), "AQAAAAIAAYagAAAAEG8PZzdARkkf4xJDhXz/JDWE6ol0t8O22YCf0Eo+LaNwpYFKYY3PP5KzY6pgcC0RiQ==", "2ae0ed32-5198-4367-8fbe-39f953449115" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b09cfea9b461425ca5d1b9d8cbe579f9", new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(3585), "AQAAAAIAAYagAAAAEOP2wA3ZGCNVSTxMCay8K1UuEJz5OLl7eGQTskO+aHwHS/9IiOp4FYjMo5NeNUFXZQ==", "1a1c2b74-a741-495b-a523-a4b869f25a78" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4299));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4301));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4240));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4245));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4247));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 22, 53, 345, DateTimeKind.Utc).AddTicks(4250));
        }
    }
}
