using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAppointmentV4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "96760c4a9d0f4b2aadad624fea2dd1fc", new DateTime(2026, 3, 1, 8, 50, 46, 410, DateTimeKind.Utc).AddTicks(3585), "AQAAAAIAAYagAAAAELSKKe+wI7s8YXGiHejNL83z30MGQc9abyQ5yVVYN9KKv1/ZDMl3CA5sN/KJ9YWXIw==", "81ee9518-5e1d-4cd9-8c6a-837c1bb34eef" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "429e2b5d8eb54aa492173111ed971cd5", new DateTime(2026, 3, 1, 8, 50, 46, 469, DateTimeKind.Utc).AddTicks(6188), "AQAAAAIAAYagAAAAEC+aW+lKtqwP0F5lsXe6+uTOMidBuWzcK+jzsZDBGq0RzFBYGT4hhOJRGCWMNT1GCg==", "31884dba-7346-4b61-aa5c-5c41c9b62adf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "98919dc8879d41fc8bd4ba80b88ef008", new DateTime(2026, 3, 1, 8, 50, 46, 531, DateTimeKind.Utc).AddTicks(8543), "AQAAAAIAAYagAAAAEIYaLLBvwxyz7cbHzANujnIVWHHg8p21u7Ek0A52ikjfeVRoFqdqFtnPQMAjvO1Uag==", "467aeab6-f1a9-4dd7-b48a-bb4c6857f21a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bfe011a58c1743be9db13f49abf7fabd", new DateTime(2026, 3, 1, 8, 50, 46, 594, DateTimeKind.Utc).AddTicks(2384), "AQAAAAIAAYagAAAAEI+ByxfPpGsAqxrctn3cbC9v+FB4r9rgda9dFA6g9951IdzM3y4xMdwFfHXlGjIn5g==", "267a033e-0d13-4850-8e4e-8138f81aa99a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe2eed2076fe49879825eca0c451e6f5", new DateTime(2026, 3, 1, 8, 50, 46, 658, DateTimeKind.Utc).AddTicks(6623), "AQAAAAIAAYagAAAAEL1fvy5uB50Pn8w9gX46EW6l6utDcDjSCrCBViAHx9caW5dS0EK4qkISLtlyqRpS8Q==", "338118f9-1992-49aa-8975-3a8343fbc14c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "57e2167e6cd041dd93ee1a83272e16d2", new DateTime(2026, 3, 1, 8, 50, 46, 725, DateTimeKind.Utc).AddTicks(5623), "AQAAAAIAAYagAAAAEFdCKi/sK4lgwL3oIKz/QWk1H/ruk/VRjUqrdDGiL8uCfKk92XKY1zXKcmBSmGwJDA==", "ce7bacaa-482a-4fc9-a161-9af10ab166c0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b0d66d0cfa1949c9a13dae638ca8b21f", new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(1514), "AQAAAAIAAYagAAAAEN1Lpw2j+vzE9KWP5rkHYB317/rWhOr29pkX/z08GUV2Md4WjOnhmazDd7zOGhq7vQ==", "3e2285c9-8010-441d-9712-df3d75d10578" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3809));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3595));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3601));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3604));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3607));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
