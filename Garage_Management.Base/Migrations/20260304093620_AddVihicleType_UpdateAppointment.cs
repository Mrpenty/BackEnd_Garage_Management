using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class AddVihicleType_UpdateAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "VehicleTypeId",
                table: "VehicleModels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomVehicleBrand",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomVehicleModel",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LicensePlate",
                table: "Appointments",
                type: "nvarchar(11)",
                maxLength: 11,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VehicleTypes",
                columns: table => new
                {
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleTypes", x => x.VehicleTypeId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceVehicleTypes",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false),
                    VehicleTypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceVehicleTypes", x => new { x.ServiceId, x.VehicleTypeId });
                    table.ForeignKey(
                        name: "FK_ServiceVehicleTypes_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceVehicleTypes_VehicleTypes_VehicleTypeId",
                        column: x => x.VehicleTypeId,
                        principalTable: "VehicleTypes",
                        principalColumn: "VehicleTypeId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e3d519f1bb74460a8b35fae19590d5bd", new DateTime(2026, 3, 4, 9, 36, 18, 150, DateTimeKind.Utc).AddTicks(9908), "AQAAAAIAAYagAAAAEGQv5/unenPZLPl4LogiUUOUifnhwPquCNqUCwCJqAk9RpEzDuLv0gS6cY/+YnE/KQ==", "88a01d6a-212c-4d30-bc01-0af8a8e3d9a2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e1bbbef53d084b54a52e7ff200bc32f0", new DateTime(2026, 3, 4, 9, 36, 18, 205, DateTimeKind.Utc).AddTicks(3503), "AQAAAAIAAYagAAAAELC/mJuo1X94Xf42ZTSouAZnArqe+V+oKuwHRGfuBMeHgn4Ihs/5J1tGDuDSqt8YbA==", "6a31f032-103a-4a02-aa35-dc2a1443d786" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f58e7ac465e14bc6b1409f9a4d8ca87e", new DateTime(2026, 3, 4, 9, 36, 18, 260, DateTimeKind.Utc).AddTicks(2182), "AQAAAAIAAYagAAAAEA/i7Xe10vp2TGbGD7mM/wBcaywK04znIbuXXxzVEDSfFeYltXMkk673cDHXNWnKIw==", "c8fc78fa-4356-440e-ab5f-7ee5a4dd7fa9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dad5ab7e29ac488194839f4dca75096b", new DateTime(2026, 3, 4, 9, 36, 18, 321, DateTimeKind.Utc).AddTicks(7028), "AQAAAAIAAYagAAAAEO933yTVzhhzrfGz0coqxb3ps8gftUFsDAHAXid2qr9wtJwi726xoKnIW7DOu02mOQ==", "07834b10-5b0e-4059-b877-69504d4b86b2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0b83d73cb4b34ee686954d7fa027a869", new DateTime(2026, 3, 4, 9, 36, 18, 376, DateTimeKind.Utc).AddTicks(5983), "AQAAAAIAAYagAAAAEOT5FIDllXQRFhTsLWywy1bW0+rgMa6zAr3VxXPQRT8sEKdOPi7UvUq+LAMXIEfhAw==", "88b3ffd7-1cdc-4fbf-9c05-2952574f2e58" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3ceef522fcd4d27a35cb7873ffa4c36", new DateTime(2026, 3, 4, 9, 36, 18, 432, DateTimeKind.Utc).AddTicks(4199), "AQAAAAIAAYagAAAAEAkYHhufNs5O7DbOTTza1Iz5MxEVPS1/uSHhx1kgp/ZJ6NOJEBD+Hu8W12a0KmUbfA==", "2720cb86-0d71-420f-be95-548063ebda94" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "701c3b9fca8a4e07a7379bb1ddd54f78", new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(4508), "AQAAAAIAAYagAAAAEC5Y41h3BGSM9S4NbII0Sk4X89pu4UjbI0Lv9+GVIGvT0a+CI26rYAKCazF9W6B1lA==", "f6623aaf-6b38-480d-a1cd-884791319851" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5269));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5272));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5215));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5219));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5221));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 9, 36, 18, 489, DateTimeKind.Utc).AddTicks(5223));

            migrationBuilder.CreateIndex(
                name: "IX_VehicleModels_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_VehicleModelId",
                table: "Appointments",
                column: "VehicleModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceVehicleTypes_VehicleTypeId",
                table: "ServiceVehicleTypes",
                column: "VehicleTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_VehicleModels_VehicleModelId",
                table: "Appointments",
                column: "VehicleModelId",
                principalTable: "VehicleModels",
                principalColumn: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels",
                column: "VehicleTypeId",
                principalTable: "VehicleTypes",
                principalColumn: "VehicleTypeId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_VehicleModels_VehicleModelId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_VehicleModels_VehicleTypes_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropTable(
                name: "ServiceVehicleTypes");

            migrationBuilder.DropTable(
                name: "VehicleTypes");

            migrationBuilder.DropIndex(
                name: "IX_VehicleModels_VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_VehicleModelId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VehicleTypeId",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "CustomVehicleBrand",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "CustomVehicleModel",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LicensePlate",
                table: "Appointments");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed2fde03cb5846d4aa69e14c72039211", new DateTime(2026, 3, 4, 6, 0, 33, 770, DateTimeKind.Utc).AddTicks(1157), "AQAAAAIAAYagAAAAECjlHv3DiHwe2AvMCwH9e/hHqv4bb5Z514+LgYEyO3oNHDhlUQFKb+ft1v7LLga2zg==", "e9064c08-df3a-452e-a0f5-81ab4290a2af" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c0133430593f43c9833d4c4110ec29ec", new DateTime(2026, 3, 4, 6, 0, 33, 823, DateTimeKind.Utc).AddTicks(2525), "AQAAAAIAAYagAAAAEHSgs0rHXRVkZYh7QPlwnQrj3oXqXIQnIbSxKmgTrQPEAtTawq7g1H83Tq222gqSAQ==", "5ceebc9e-3a4e-4614-9e8e-e2ae8c6b10ac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8828b6bb15684792988cc7f50db90f44", new DateTime(2026, 3, 4, 6, 0, 33, 877, DateTimeKind.Utc).AddTicks(5739), "AQAAAAIAAYagAAAAEKn1sbBfP/TmqiKtBOyOwHlDrle2H1L2NVYs7VQWczV3JK6CSKqm2v+1QhuBSQb+QQ==", "bc636c98-ab16-47ca-a8a9-6faeb912d571" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f659ee099560487582df77ec68b8fbb1", new DateTime(2026, 3, 4, 6, 0, 33, 931, DateTimeKind.Utc).AddTicks(6398), "AQAAAAIAAYagAAAAEKHXzADSk35gnfFAzyWejcnPeh9V6qPWhPo4VT+7bYVYGmYAQ+60KSM+CO8tWX02tQ==", "462f92c2-3f01-4369-95f5-07210a3b7518" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf9429dd4a5642be8d252c1cf7e42765", new DateTime(2026, 3, 4, 6, 0, 33, 990, DateTimeKind.Utc).AddTicks(8878), "AQAAAAIAAYagAAAAEIAQ/nUZsvxDY8RhteQaJcQAggwkpXWqTnErdmd0oo4r7g/efI7xui0DDtpKlylHcg==", "a7d856ca-eb9d-4946-bb11-63d1e911239a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1ae3002628f419f9507f792b8a22764", new DateTime(2026, 3, 4, 6, 0, 34, 45, DateTimeKind.Utc).AddTicks(4003), "AQAAAAIAAYagAAAAEHG7Sz83O4cshzzidiV6VHTIuV3onqLiul8WfzHq016RMf0vf96HiUctZkPn6PCyrw==", "c938f6ac-be53-4457-a76b-4eaeab7c1930" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f15f1c4bee1f44a0a9bf399f923eca7d", new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(4659), "AQAAAAIAAYagAAAAELTMcW8C7L8COSZKbYM1yHI1yiwZc+zvu8oLqMKk8pdENwJPSk6xy3yTHNrAcOiV3g==", "27608f83-7580-496c-a05b-0eaf4a580b27" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5283));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5285));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5173));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5177));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5180));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5182));
        }
    }
}
