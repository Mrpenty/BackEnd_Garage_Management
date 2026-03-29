using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedStockTranssaction_deleteReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StockTransactions");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "38a8d0183ed74ac8a1c1249b08c413a3", new DateTime(2026, 3, 25, 7, 28, 5, 414, DateTimeKind.Utc).AddTicks(4689), "AQAAAAIAAYagAAAAEGFxhka/WP4rztLGaJDJUFT0etzu3mImoVa6Un9eif7892kQSHQ4HJfTVBCCVNcS5Q==", "5b6a4224-84dd-4305-b979-15b78c58c314" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5917370b62804f18af83f15b6380f997", new DateTime(2026, 3, 25, 7, 28, 5, 467, DateTimeKind.Utc).AddTicks(1059), "AQAAAAIAAYagAAAAEHbj+VuGH46rtJNgIahubH9c+F3MmR6VV11EjSCFnh9dytaWCs++oU5c6EMnNLGZYA==", "c61a1880-b13b-4590-9482-753068f03897" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "12f1d72885d84c62b509135fb4c6c215", new DateTime(2026, 3, 25, 7, 28, 5, 519, DateTimeKind.Utc).AddTicks(4962), "AQAAAAIAAYagAAAAECvDial0xApQljYNbl9EzUJntOOfnEXaehy+O7ElVvH0lcusMSdWC6WCV6sHcFiEHQ==", "cdc21a21-318b-4190-a35c-70e5df9ea56b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5a0347c0fcd04afcafcf7c0186cbb747", new DateTime(2026, 3, 25, 7, 28, 5, 572, DateTimeKind.Utc).AddTicks(1838), "AQAAAAIAAYagAAAAEGNOq66PfTFMPyds5bovLLTNbPbFY2HtBwvCnDsRmnLTyJmt8PtDZQDycHWbBg4nOQ==", "01f26687-aac8-470a-ade0-17e00fc98446" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "289cca9a352241d68433c5fae2da7f0f", new DateTime(2026, 3, 25, 7, 28, 5, 625, DateTimeKind.Utc).AddTicks(4623), "AQAAAAIAAYagAAAAEGnCXVjqAlAKyz51KUwdA+cz6VktM2vCF3c4AlEYAyLQZKV8fIGYAcO5b9ItoBIOFg==", "c346c74b-28bc-47a4-963a-1dfd9d20e698" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b36398e53dd3477f9a8d270742882e6b", new DateTime(2026, 3, 25, 7, 28, 5, 679, DateTimeKind.Utc).AddTicks(438), "AQAAAAIAAYagAAAAEOwTn6kgjx+07kPCCUFxMWoLgpBqxxHspues/SF0ruY8NwPapLbBd0GQq3e35Kt4DA==", "ed3d8b90-55be-46d9-ae4d-f10d46005c0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a5bdbc63a0754efc947c2db86dd8d7c3", new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(6836), "AQAAAAIAAYagAAAAEPih9qVBDsMZfOJYvrt0AJD2zHVrJ4y/c6X4NIXZuqy/sm6dshbHJ3ZJR9J8BiUXsg==", "1250155f-2669-47ad-bb37-162d1ddac80e" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7216));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7219));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7172));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7176));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7178));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7181));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StockTransactions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a0b0679853049c5aaedc3e0f359f899", new DateTime(2026, 3, 24, 7, 24, 18, 266, DateTimeKind.Utc).AddTicks(5484), "AQAAAAIAAYagAAAAEEGw4sPiYqAtC862JnfZ7jiS0iX+TPOu6XX3cKKuI6zT0z4d1dPf8pbsj2bslJ+nNw==", "ed395f57-6723-4d51-a6e3-3f0287c0cef5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ca7a984c1a2040318d0b85d736733fcd", new DateTime(2026, 3, 24, 7, 24, 18, 323, DateTimeKind.Utc).AddTicks(5051), "AQAAAAIAAYagAAAAEFqKpSF4RWKPUDQdSZvovkLpVqqD8+0WGxHtEyzXNwtAs0CS7iC2Unnmx16P+mSmSw==", "711ef9f0-3d4f-4cab-88cd-dbbbad0d997b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bc7187c8f7a84fab8e934da9416e5768", new DateTime(2026, 3, 24, 7, 24, 18, 378, DateTimeKind.Utc).AddTicks(6980), "AQAAAAIAAYagAAAAEOEkvGuulAwIFx26pV7RMJ9ekIqmiDqE2Xz74qCusixooUdHVCywXVTgP2k23MOwpQ==", "3a7ea82d-69e6-4587-abbc-cb9f00ad8b57" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf1228eea90a4d92bf547c518b3525e2", new DateTime(2026, 3, 24, 7, 24, 18, 433, DateTimeKind.Utc).AddTicks(5950), "AQAAAAIAAYagAAAAEO7z5T2/OPAgQIRQ0anBqSltDjSPvIlDMtuCxHgWb/WAyI3cTvaDB27Qm7bmEKi7Og==", "acae21f0-577a-42e4-b326-57bf100028db" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c40c8c678e64ed4afe4bf7692a9c3c1", new DateTime(2026, 3, 24, 7, 24, 18, 493, DateTimeKind.Utc).AddTicks(4341), "AQAAAAIAAYagAAAAEFNpaojbeEBdqeUor4BPqqUge/G3fo8V2GFAtRouPZHNr9q7SB7eLZgyIfvCGphCQg==", "0cbf78df-6a47-4177-94b7-eb9cdffade09" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3151966263d84ba1808036e3fa00e628", new DateTime(2026, 3, 24, 7, 24, 18, 550, DateTimeKind.Utc).AddTicks(318), "AQAAAAIAAYagAAAAELB+bUYZZTZdXgUkmst8/B8qfLB7vNK2KYWdPfM+2/RtoylfvNNpsJ62qRn8gQm+Bg==", "eb00b249-569e-4a3d-a4f5-d2d9652943a8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1dd18966c76b433195afcc4ade8a9d31", new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8387), "AQAAAAIAAYagAAAAEAeElfsFiz+XOGz9nwNeSckugxf4hdiLyFC/JJ4dS2AaHJ1/R0Ngfhff4Aa1Or0T6g==", "2a3a2090-03f3-48b3-b2b3-6da4ea8118f9" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8801));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8803));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8760));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8764));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8766));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8768));
        }
    }
}
