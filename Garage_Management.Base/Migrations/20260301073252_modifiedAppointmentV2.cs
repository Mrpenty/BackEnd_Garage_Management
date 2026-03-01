using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class modifiedAppointmentV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VehicleBrand",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "VehicleModel",
                table: "Appointments");

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
        }
    }
}
