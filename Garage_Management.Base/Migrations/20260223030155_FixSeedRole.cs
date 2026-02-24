using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class FixSeedRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Stocker", "STOCKER" });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Name", "NormalizedName" },
                values: new object[] { "Stoker", "STOKER" });

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
        }
    }
}
