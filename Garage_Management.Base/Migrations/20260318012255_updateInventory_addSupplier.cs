using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class updateInventory_addSupplier : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "887993aa32894599b4fd6b76b439666d", new DateTime(2026, 3, 6, 12, 31, 34, 394, DateTimeKind.Utc).AddTicks(8913), "AQAAAAIAAYagAAAAEGhX7KvSFWurT9tpZpJiCyTKaD4qFQqSV1E3F3WsTTQB7S07sGt4jCQxrUlGIrhZ0Q==", "3a465575-4348-46fa-b956-a8bc0edb4278" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "856bbeda21d140eca86aa7427127e173", new DateTime(2026, 3, 6, 12, 31, 34, 452, DateTimeKind.Utc).AddTicks(8701), "AQAAAAIAAYagAAAAEG3R9LtTkHOx/NoBUuCabFJkN+bm81uU94HtKduw/SrllsFY6VBiGaL2Ufqz2bohSQ==", "9b179718-a492-4784-86c1-0b106773e3c7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed083484d69f4066902ba778b7a20738", new DateTime(2026, 3, 6, 12, 31, 34, 510, DateTimeKind.Utc).AddTicks(4596), "AQAAAAIAAYagAAAAEKGF/ZSGazZxkx3B1zcrSdKOSOYqdy3fLmb5bNT+FGXoj68gxPE9stbrrxks6ON1Aw==", "59602cb3-168a-40fa-9e96-8a6be4519ebd" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "64ab95a2ceb648108418f227f5bcf123", new DateTime(2026, 3, 6, 12, 31, 34, 580, DateTimeKind.Utc).AddTicks(4388), "AQAAAAIAAYagAAAAECBBkT+4A0G8HSTaRpik6144DtDhMBbOVg2jG/VZ507M3jRwYwGw7ggyMOV7lwaQxg==", "3f4cb836-f792-4556-be0a-325257a66332" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2822cfa152b94d7bbf392b66bed6e024", new DateTime(2026, 3, 6, 12, 31, 34, 661, DateTimeKind.Utc).AddTicks(1740), "AQAAAAIAAYagAAAAEHDxVuHn4MmubaZpW8pY+XnoXgGSgCN+rMY/iNtAZzI1sJtryt9GX6e/EU2NJUCF7A==", "34f936f4-d90f-4649-9809-d8ec91a82cee" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bdb35c70a0774c9ca76b55f5ffd12a08", new DateTime(2026, 3, 6, 12, 31, 34, 726, DateTimeKind.Utc).AddTicks(3295), "AQAAAAIAAYagAAAAEPH2PLmSidgPlzXLYiXpUnDb0iFUm/oHfbdTX9UicAOWz4vilUUah5BaJT8RA9QQew==", "0d1779e2-137b-4f8d-bcf6-98263cbc9ee3" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "be566b9e711241af84854f529d567873", new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(3954), "AQAAAAIAAYagAAAAEGgTQvUqWTUm2akiN9qSe/ditccuQfuU9Hm4wix67L9umSNS3BBo9Pq+Qo4qZIQWaQ==", "533cc87c-05a7-44b8-abb7-ba812797ffb9" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5369));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5375));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5192));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5209));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5212));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5215));
        }
    }
}
