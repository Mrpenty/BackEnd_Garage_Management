using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class addEmployee_WorkingStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Employees",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                columns: new[] { "CreatedAt", "Status" },
                values: new object[] { new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5192), 1 });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                columns: new[] { "CreatedAt", "Status" },
                values: new object[] { new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5209), 1 });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                columns: new[] { "CreatedAt", "Status" },
                values: new object[] { new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5212), 1 });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                columns: new[] { "CreatedAt", "Status" },
                values: new object[] { new DateTime(2026, 3, 6, 12, 31, 34, 784, DateTimeKind.Utc).AddTicks(5215), 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Employees");

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
        }
    }
}
