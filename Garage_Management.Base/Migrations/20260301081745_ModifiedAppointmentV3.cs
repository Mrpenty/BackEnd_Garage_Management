using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAppointmentV3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "Appointments");

            migrationBuilder.AddColumn<int>(
                name: "VehicleBrandId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VehicleModelId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "20ab1222a6574288835bb4fba59d20b7", new DateTime(2026, 3, 1, 8, 17, 44, 606, DateTimeKind.Utc).AddTicks(6141), "AQAAAAIAAYagAAAAEBWaBy7hWBIsjnlDOOUMTkEda+OUtWeodhl//Et/umXny41sbZ+hG/uThAupzglpMg==", "eb08d3f7-6784-4ef0-be75-5e15467ff87c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b4d4de0faddb441abd96ceb42965683b", new DateTime(2026, 3, 1, 8, 17, 44, 669, DateTimeKind.Utc).AddTicks(1359), "AQAAAAIAAYagAAAAEPHzdRiSH1ZqnkRBkojgDxKDS8s7jKzmhadQM3jTKe5+zsc3iJQcfVS1RQaIdjgqMA==", "86d05829-5fff-4deb-93cf-70934d7df676" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eab6c0e918d24af79d86401ae6a95823", new DateTime(2026, 3, 1, 8, 17, 44, 727, DateTimeKind.Utc).AddTicks(4818), "AQAAAAIAAYagAAAAEFw7Ygcn/2phu2deXcWGEOb5gUeGYRuYcfbpV3sguvNjr18DDtsg9uYDP2MUXnKF+w==", "175cd149-2d0f-40c7-b7a1-ad886013bf9b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e0265c5c4334defbca32e4b0e911803", new DateTime(2026, 3, 1, 8, 17, 44, 785, DateTimeKind.Utc).AddTicks(6212), "AQAAAAIAAYagAAAAEHx1gbUI443IiF5vbnLpGXV+09EuvEg2fVM6wvINFFwmFQZ0WV+7tPG68ueFSyA8gA==", "d5b32b76-f53f-4592-a0bd-b7b8e185e2d7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "450727cad3c1427eaa95204d50b1a814", new DateTime(2026, 3, 1, 8, 17, 44, 846, DateTimeKind.Utc).AddTicks(4844), "AQAAAAIAAYagAAAAEKIMb3nUsc6VBxP+UX8jBj3Xv7gmfvpdZJZymF+IPq38EPsPZUQhmWn4/KhL6ouQng==", "830bb177-0d21-486a-b071-b0de8c21368f" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3055f82210144e08982357f2f3b430f2", new DateTime(2026, 3, 1, 8, 17, 44, 907, DateTimeKind.Utc).AddTicks(2760), "AQAAAAIAAYagAAAAEDAz6yZ2c1kQWS/uYkWyhu1zYJX5mnKjwJ4fL5dydQ5FSiksjGnD8FbioXkVdYHanQ==", "5c9a4bdd-c6b2-48af-9079-0341a506f0c3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6bf53fcc36b249ba89a03ba97a7938b8", new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(1979), "AQAAAAIAAYagAAAAEI67o2PAog5BIIkyyc/Y9Z5xVlIbyB1WdIjApuZQnPJiCnWG38sf+tknJSPOpaJtCg==", "69bed701-3977-4ede-8364-03b2d1d80fb1" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(5012));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(5014));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(4600));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(4604));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(4937));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 17, 44, 974, DateTimeKind.Utc).AddTicks(4940));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleBrandId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VehicleModelId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "VehicleBrand",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VehicleModel",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0cc23eb6c7de40e7ac1e79b7abc0ba8b", new DateTime(2026, 3, 1, 7, 32, 49, 215, DateTimeKind.Utc).AddTicks(4464), "AQAAAAIAAYagAAAAEI/dGrBWg5n5QP88Q0/fehtAIjPi1zGXSMwis3mQnJZYsR0XN+gLkR7W+NbXZp4urQ==", "ae045dda-d8e5-4d8b-a47d-92592f33cf85" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8fee1d88295f4d0badf55c62b4825694", new DateTime(2026, 3, 1, 7, 32, 49, 287, DateTimeKind.Utc).AddTicks(204), "AQAAAAIAAYagAAAAEGlouAPFbopz2XWHjB3LGhEo6tvYzHJljhN2ZLTeHkC6jQQFrdFMduJw8z+SJ75ZUQ==", "fbdd863d-bfde-48d9-a76e-acc716efea54" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8c2a72b7ec5649129f2198c8edd528a9", new DateTime(2026, 3, 1, 7, 32, 49, 357, DateTimeKind.Utc).AddTicks(5120), "AQAAAAIAAYagAAAAEMwtZWHvocyJxzUcmb7hjWOWpfBZUjXajZ5+NffKVAGUZeCoLPxl7vuU0j+TSkN3Ng==", "a8b3acef-651b-4570-bf16-1f8c7280806c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c2d297d35c084b8d880e1cdf2de99034", new DateTime(2026, 3, 1, 7, 32, 49, 436, DateTimeKind.Utc).AddTicks(1647), "AQAAAAIAAYagAAAAEB2zDCPOXDb0i6+HOKt8OxAcbC8ia/pu51DO8GacwrL7aSvTtwCWCQd1uX4jtX+nzw==", "6faf0838-66f7-4979-8b2d-cfcb58f8feac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f10dcb7692f644faabc423172ce40919", new DateTime(2026, 3, 1, 7, 32, 49, 516, DateTimeKind.Utc).AddTicks(4856), "AQAAAAIAAYagAAAAEKc04rzy+tYFrH9eZ3xVO+x0EbTIez3Zoy6Pq62bp7aEsxDCBN6xDkkV3GREKe2XsA==", "71c3181c-77cf-4ed7-9810-83f71902a836" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f57382b490f54df79374241de8d93ee4", new DateTime(2026, 3, 1, 7, 32, 49, 596, DateTimeKind.Utc).AddTicks(5070), "AQAAAAIAAYagAAAAENuwlKs4TH7UszFUfbJ6OoCDDRnoPNnsxhav3VD0dkq1ySxLbdpjvV5KS/G42JPQhA==", "178d4beb-a5c9-4d31-be77-c239b02180e1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1e0279be725464d8c997d3364aef98b", new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(1506), "AQAAAAIAAYagAAAAEPTn8zoWKw7lI7r4h2SuGPsbpVlNr9nkGPRJW8ace40plluIyz0UtVsdlw/b0KmwZg==", "7e504a26-5380-4aac-8394-55e0b4dae608" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2244));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2247));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2005));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2009));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2013));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 7, 32, 49, 672, DateTimeKind.Utc).AddTicks(2186));
        }
    }
}
