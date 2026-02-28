using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedAppoiment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Appointments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Appointments",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Appointments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4efb538c4d204fefb0562e04d747d79b", new DateTime(2026, 2, 28, 7, 18, 29, 557, DateTimeKind.Utc).AddTicks(5440), "AQAAAAIAAYagAAAAEJaY0+eJKV0dAM8xzDvMFu2x8lQyy2iF3AuFBIdD3Jjycv6dhg0XSZ0BNJsexzUguA==", "be48e7f5-a02f-46a7-8a17-fdda99d35c8b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3cb94513aa034fb28293ddfd40e5617a", new DateTime(2026, 2, 28, 7, 18, 29, 610, DateTimeKind.Utc).AddTicks(6077), "AQAAAAIAAYagAAAAEPQufF79v0zFSGBPy0Vs/ZbPvOSoZIMF8IixDYRwt+niPb5Jg5BABGssz9O0wIi6dg==", "971fd748-0bb6-496e-9a14-387616225b4a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6544bf43dbd4e04a1873a2a7f77ade6", new DateTime(2026, 2, 28, 7, 18, 29, 664, DateTimeKind.Utc).AddTicks(3447), "AQAAAAIAAYagAAAAECkZVnBxnfrSQyKrJYgs/OPgighrK4e5Em0GFcHrF17Hk+5hPVQYWsS89nOsD5w8IQ==", "b45d9b34-1a7f-4ca5-9f0a-cec0f10f4704" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a18200b4cfd40828cbeb38776ebfcba", new DateTime(2026, 2, 28, 7, 18, 29, 722, DateTimeKind.Utc).AddTicks(1007), "AQAAAAIAAYagAAAAEFug60cyqwZTLXOS12PYhK2+Ry6qvWToBqQphjDu8+lLEldyreNDkXEvQzID0jNdcg==", "2e717f86-ce28-4f62-8455-da6f83cfc3ac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1662024aa9194e079a1e4eea28074993", new DateTime(2026, 2, 28, 7, 18, 29, 775, DateTimeKind.Utc).AddTicks(4215), "AQAAAAIAAYagAAAAEJcR/MPtKcq9PqmGMZWEeJq5AGSnl0M35fY/3cbcAfcYW46n1K22yKS98Ma4MugHYQ==", "ed0ac10c-ae8c-422d-ac35-d3c17f12c5e1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7bcb98ae49694066bd7352a05aeced0f", new DateTime(2026, 2, 28, 7, 18, 29, 828, DateTimeKind.Utc).AddTicks(8284), "AQAAAAIAAYagAAAAEIV8zIjQ2wZPGrPvuiyJ55YylYx61Tx5s0sSw0ZsaBK43joC8aEEA/joJCnPzHLcag==", "716bb84e-7bb7-43f7-8479-edd674b98470" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d3a804da2a14a51a557d3727f5607fd", new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(104), "AQAAAAIAAYagAAAAEEf4HuZpU6Js6bTqHFC7kf3dxcbpgsAXa2puahthqsR61Ee4GXg/F0UusSfKB69+fA==", "7651df7d-f276-49cc-965e-63669088493a" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(774));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(776));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(635));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(638));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(641));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 28, 7, 18, 29, 885, DateTimeKind.Utc).AddTicks(643));

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Appointments");

            migrationBuilder.AlterColumn<int>(
                name: "CustomerId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a9dc8cd5b3fe4b6b94e4f706aa4298e1", new DateTime(2026, 2, 23, 3, 1, 52, 782, DateTimeKind.Utc).AddTicks(1581), "AQAAAAIAAYagAAAAEJed9HXx145yvsGSBpy/WhhvBAfgV8w04urb8OZMyJdGtMvE6w5eDFl/PdibKcFADg==", "1830b41e-3bda-4a81-a41d-c5d95578c96e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d72207bfb52541bdba18b59a2e6f8926", new DateTime(2026, 2, 23, 3, 1, 52, 845, DateTimeKind.Utc).AddTicks(162), "AQAAAAIAAYagAAAAEGCkh7e6z1mNNCXzrCasW5j2k6VQuOjXrr5HAtI63FKDLcsh2IIDfaZGhDCVu7QDKg==", "d40c9066-12b1-46bb-8b0f-8567da96a6cc" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7d7c5414389342fa8d1eaa6404bf8188", new DateTime(2026, 2, 23, 3, 1, 52, 915, DateTimeKind.Utc).AddTicks(994), "AQAAAAIAAYagAAAAEOqzmiokN5dfo0+eRg0phnMBpxkdxr5rcfyeBxiRM2imvo3RJFmW24ZfQxAX9AIcuw==", "8fc84749-26a4-48ea-9266-b9ea82633c57" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ab70cc182d1a4c62ada7ea2331b19d92", new DateTime(2026, 2, 23, 3, 1, 52, 976, DateTimeKind.Utc).AddTicks(9524), "AQAAAAIAAYagAAAAEERy+rUjDv5x1N6N8j2AbN0eUIroFKjLE4eDX0cD0jG6PuMlbF6HMeR3SFvepyx4sw==", "83c3e873-a3fe-4685-94d2-af68d1511d65" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0516a083511c4e249a6a1c07976e7023", new DateTime(2026, 2, 23, 3, 1, 53, 40, DateTimeKind.Utc).AddTicks(4304), "AQAAAAIAAYagAAAAEJ1+kndM2puiO6mEPjnBBjNw6quRKFk2HZJ/1M/CjR12q0CiNO7GwnLJ3R1Q7+4D+w==", "25edc126-d572-44d5-b6d5-b4843ec69a10" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "54ad441c96514daaa03a427b04fbc537", new DateTime(2026, 2, 23, 3, 1, 53, 105, DateTimeKind.Utc).AddTicks(9781), "AQAAAAIAAYagAAAAEJpA0Oi/Q2ok3lQNQhS6hdnwECM6+zJ8Bf8wzlWzVo2U2v94NT6T6xBkfn7nNlrIAg==", "5c6add17-1ea6-47a9-82db-1a57051b27d8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "61c719c2a31b44118563f55bccef9148", new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(169), "AQAAAAIAAYagAAAAEFmApE/8+NtWVdn33ACWzOeqoUicibg9tS894o90HzqLkhLIyhljAOCZv4Bq6r+q/g==", "bfa0f8c1-91b1-4f21-9013-433232a1ef76" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1260));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1263));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1143));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1150));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1153));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 23, 3, 1, 53, 192, DateTimeKind.Utc).AddTicks(1157));

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Customers_CustomerId",
                table: "Appointments",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
