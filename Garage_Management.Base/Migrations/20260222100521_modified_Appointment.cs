using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class modified_Appointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppointmentServices",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentServices", x => new { x.AppointmentId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_AppointmentServices_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AppointmentSpareParts",
                columns: table => new
                {
                    AppointmentId = table.Column<int>(type: "int", nullable: false),
                    SparePartId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppointmentSpareParts", x => new { x.AppointmentId, x.SparePartId });
                    table.ForeignKey(
                        name: "FK_AppointmentSpareParts_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "AppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppointmentSpareParts_Inventories_SparePartId",
                        column: x => x.SparePartId,
                        principalTable: "Inventories",
                        principalColumn: "SparePartId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a220e566de3c40c18e562e5e0343b36b", new DateTime(2026, 2, 22, 10, 5, 18, 692, DateTimeKind.Utc).AddTicks(4788), "AQAAAAIAAYagAAAAEDjNmqokH3UX2lndQXCnPCDdVs3nFc4A9MZj3WsxZLIbANEwlq9uZkbmAOz2Jh6c3Q==", "d390bef8-a487-498c-9c6d-397180295b06" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de03ff2620db4ea494f1de0f9a90473a", new DateTime(2026, 2, 22, 10, 5, 18, 756, DateTimeKind.Utc).AddTicks(3775), "AQAAAAIAAYagAAAAEPJkyGw7tJTmAr11fy97Lckb3dtR7IbI8fdDOAL17SjIoPgjbVAIV/JOH7fgwaBcpQ==", "79c82c8a-bafb-41c4-b037-6317ebf2f8e2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "761fd3a19a564df596f246476985d01e", new DateTime(2026, 2, 22, 10, 5, 18, 813, DateTimeKind.Utc).AddTicks(2013), "AQAAAAIAAYagAAAAEDUmYnDiTcErN92J3pB4JiK9wCreoJjd5ahcvVhFJJm/KKQMsiVw3ispWpuksoic/g==", "043c1978-6bdf-4716-8ef9-0c90d3a67a48" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d40f8ccb396b4ab386c6a6e05837602d", new DateTime(2026, 2, 22, 10, 5, 18, 870, DateTimeKind.Utc).AddTicks(2576), "AQAAAAIAAYagAAAAEJ4sHTu5oDEmA92Yq1w/7beT+CG0WAJu143y2YwAH9hJ4mHmkQt678qUZBcmvToM5w==", "649fd979-6d0b-423e-aac1-e341aca0545f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f83f13b7d5044353921b1bef76badbe2", new DateTime(2026, 2, 22, 10, 5, 18, 926, DateTimeKind.Utc).AddTicks(1955), "AQAAAAIAAYagAAAAEK+HQqSipVvVkIrVHKS1Y8HuADigzXtONlH4L3qki/1uqWRTGM4LRBnD9iPDIELQ3Q==", "48b8b332-0cbf-47a3-a95d-8d9ac4ba5127" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "00ac17ad68eb40a3aa43e8b87f3fb94d", new DateTime(2026, 2, 22, 10, 5, 18, 981, DateTimeKind.Utc).AddTicks(834), "AQAAAAIAAYagAAAAELgIwwLzgJmnclxZstdA18j2rKkVNnmc8pSyAep1S3LQh4fRQJq9zUImqpOSkFpqfQ==", "93aa2e60-bf48-4d7f-a29c-c19b4fda1b98" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8b8a0eeaeb874391a8c028ae448a3bb6", new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(4505), "AQAAAAIAAYagAAAAEGM/HCsx//8+6WNzLLsaLKnWHGeeVlVDY5y606IjcsOqrbvgZr+XkOp7X6zV5hM1EQ==", "5023da0d-5e78-4c6d-afad-330535cd7d2f" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5081));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5083));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5016));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5020));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5023));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 22, 10, 5, 19, 36, DateTimeKind.Utc).AddTicks(5025));

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentServices_ServiceId",
                table: "AppointmentServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppointmentSpareParts_SparePartId",
                table: "AppointmentSpareParts",
                column: "SparePartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppointmentServices");

            migrationBuilder.DropTable(
                name: "AppointmentSpareParts");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b00a05e6f74c4e6ca405cc3eb637bc93", new DateTime(2026, 2, 19, 1, 2, 58, 444, DateTimeKind.Utc).AddTicks(6216), "AQAAAAIAAYagAAAAENwxlQatuF81XWEBWRAymlGrp9QzpjQvuJ0zTd92GYAYYK7/C5WatjbX63jvSuN0Lg==", "3aeff363-7886-497e-a513-5cd59d0b7298" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22bbdb7c88654aeeb51ee423cf7d0f98", new DateTime(2026, 2, 19, 1, 2, 58, 503, DateTimeKind.Utc).AddTicks(1342), "AQAAAAIAAYagAAAAEPBtZDb/lHOHLNyqEGu5SjxfxInVZ1yCvhpifThxZ6JgrV0Jfd8B9t9+CFJ9/DIGSg==", "fad7b8da-2533-4dc9-98aa-0906eb7ff828" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad0e8c7627a64c21925ac0d49bb8569e", new DateTime(2026, 2, 19, 1, 2, 58, 556, DateTimeKind.Utc).AddTicks(8583), "AQAAAAIAAYagAAAAEC3PdonQhyyjxx68GiPKl2hyJQO+9joL1gPGPQy/Zb019bNWPLOAZeisApCHll2H8w==", "d9b6a6be-003b-4ef9-be4a-792875fd9083" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8cf68ad392ac45419ddcaf39806735aa", new DateTime(2026, 2, 19, 1, 2, 58, 613, DateTimeKind.Utc).AddTicks(4210), "AQAAAAIAAYagAAAAEEzCBkbNJzU1WVjlVYaFUJZjboCWUE92uoFzcd/FVCIU6ZgL8vrSGHwHbLkqiNLeBg==", "72251bca-ba5e-48e0-9a50-cc481f3ea3e9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "08e36691246a4f9cacbe771b19fb3528", new DateTime(2026, 2, 19, 1, 2, 58, 668, DateTimeKind.Utc).AddTicks(9822), "AQAAAAIAAYagAAAAEG4TAiRkCe4HOuJue/Hjs/r9k7Z5nPGYiH/5phCmUIe52pgvA/8NP99x5UwBKSkFcw==", "697d2ef7-e0ad-493e-9d32-134227265248" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2409bddaec0443d3b48f786513c45267", new DateTime(2026, 2, 19, 1, 2, 58, 725, DateTimeKind.Utc).AddTicks(1507), "AQAAAAIAAYagAAAAEDCoH6hJUnTFT7pp8TMyY8aZT3CzTFQQNbZPQzlxSwGkNFWo3vpC8PlBiW1C4wOBEw==", "a071d314-02ef-4f46-8275-1c31aca992e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c7dc95be5b7d4880a95956395ea0edd5", new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2130), "AQAAAAIAAYagAAAAEJszMwCLviIewzot5LXiTmI2pw8p9xwh3P73U2Gdc3pnQZVDRfxXRRF7cic1XLj4Cw==", "a1526484-c073-4623-81de-ee6b44f905ad" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2961));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2963));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2886));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2890));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2892));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2895));
        }
    }
}
