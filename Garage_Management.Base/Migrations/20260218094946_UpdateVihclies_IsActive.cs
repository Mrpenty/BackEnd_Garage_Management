using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVihclies_IsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VehicleModels",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "VehicleBrands",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13c96177e761496ebd9b0692ef291eda", new DateTime(2026, 2, 18, 9, 49, 43, 986, DateTimeKind.Utc).AddTicks(5494), "AQAAAAIAAYagAAAAEDgiE9xmvc8/w3XZyu+QgREwzfGULHo8vaisnXwA3pBHbD70CS7ea6TADSjgwKqJPA==", "dca49ea9-f448-474a-9412-27c8da214e32" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6cafe3c12ea415c964841af8ec53e50", new DateTime(2026, 2, 18, 9, 49, 44, 40, DateTimeKind.Utc).AddTicks(8443), "AQAAAAIAAYagAAAAEAwEbiir3/VPM5TzCbOSnFwqQeGOAt7odNVro16i93vxU9KTJTqr4hiNB1qE5LYCzw==", "ba4790c0-b787-4e54-96c8-2181a6f46e0e" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "661a923cf6d44575997e12ae3e9d7dd8", new DateTime(2026, 2, 18, 9, 49, 44, 94, DateTimeKind.Utc).AddTicks(8774), "AQAAAAIAAYagAAAAEIQSnQGuQ8BCW9n99pR5t4W+6/4Epx4/6QvHZ7YuX3CcS5EIIYMGoC4AsX5+5D5xCg==", "0f064446-5764-4342-b6ce-82003727a192" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1ea8caf5106741dca806280c0fe94b3a", new DateTime(2026, 2, 18, 9, 49, 44, 154, DateTimeKind.Utc).AddTicks(2860), "AQAAAAIAAYagAAAAEMNZTwxGrbyboGSKvbqVE2NjTLVyxbZLBNVWOQBa+KS9Fl17eJuOqUble9ab1xi9JQ==", "563e422f-a409-4c01-9f51-e36b07a62942" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a2df7095a089442a875fb6e1ae4b0df1", new DateTime(2026, 2, 18, 9, 49, 44, 209, DateTimeKind.Utc).AddTicks(7510), "AQAAAAIAAYagAAAAEJvSdxcUBjdL3J9SiL3mJplPiKNKLLOpTtkYFOkomiGVx8k9NPG+BsAhhoLB4UmOfQ==", "bbfcbeaa-cb29-4a9b-8b15-297bad077603" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b49a3e1163c649d5868fb951bc9b9255", new DateTime(2026, 2, 18, 9, 49, 44, 266, DateTimeKind.Utc).AddTicks(2344), "AQAAAAIAAYagAAAAEB88MoqOsSQvRHT54QjGN7Zc0e3GFv1dI33LjFDA3qhCCNgoQ55sIRT+R0DJJXxWkA==", "01ebc71c-f038-4650-ba42-363652655620" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2099020cc357439e9854eda70e1b02bb", new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2265), "AQAAAAIAAYagAAAAEI2KWrAaeZDvsgzQqQQp32zLFXXYxkNUxd8mOvhjh/S7Hzb9wsjsbeRIU83g8lhqmw==", "60b65f05-b13b-4dc4-8748-3aff81f48976" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2912));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2914));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2857));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2861));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2863));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 18, 9, 49, 44, 322, DateTimeKind.Utc).AddTicks(2865));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VehicleModels");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "VehicleBrands");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cf9c3d833eb4ff88a92294428c4d7ef", new DateTime(2026, 2, 14, 8, 17, 14, 519, DateTimeKind.Utc).AddTicks(4351), "AQAAAAIAAYagAAAAENANEXHQc71YrXwZwvEFqSlI5PuuZ/lGELthmQZvxCTftEhmnH+rN/J5lvMZEezTIQ==", "cc176c68-fe99-427d-ad6d-3120f4ca6778" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8852e1d58ad14077863a621a5537429b", new DateTime(2026, 2, 14, 8, 17, 14, 573, DateTimeKind.Utc).AddTicks(3829), "AQAAAAIAAYagAAAAEP8FYhbR44jOdGkL+cPSfVE9ZldeUxwg/2nf08iwsnPmguryhGh11HrLeryQiUFGWg==", "1ef7b7e0-4bda-4a26-a7e4-2e18ea0d878d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fc696fa6264d45d5917dd4f8d8e9a7c0", new DateTime(2026, 2, 14, 8, 17, 14, 634, DateTimeKind.Utc).AddTicks(1907), "AQAAAAIAAYagAAAAENqEUhfde27nSMUt4pJYsJAWE4DkNh+oa+O7Nt44O5mGY8ujf9jQgAUs+fBZci8+0g==", "17ca26fa-321d-4877-a503-b89a4ea7a5e8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cef4c99945264286aa80fdfdf9b7fc7f", new DateTime(2026, 2, 14, 8, 17, 14, 692, DateTimeKind.Utc).AddTicks(1807), "AQAAAAIAAYagAAAAELA8KvdXVSRGfcFw08aFBQM4J+mXtyBjjYGi4WnPFT/21Fq+fcrOwdLQZGBHbjgTAQ==", "a714b8bf-4aab-45e9-81f4-64a68a09aae9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d856377cda8446e495d94f18a4803578", new DateTime(2026, 2, 14, 8, 17, 14, 749, DateTimeKind.Utc).AddTicks(7091), "AQAAAAIAAYagAAAAENceq0P/cw+FQ1p3x31g9Q7Nc5tTdjr+lDu0Uwtd10WA02v9Sz7s3EC8exa+H4hvUw==", "084df1bd-2beb-468b-afdf-63da2f12f4e2" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d2c02328f5374985a7b55f9ddf56ab95", new DateTime(2026, 2, 14, 8, 17, 14, 823, DateTimeKind.Utc).AddTicks(8432), "AQAAAAIAAYagAAAAEIVJ+/Hjj4qAHszs1sztgCgDJSHSmrRm55+pZ66iax5R7zc07F8npq8OEE4Ug4CjQQ==", "31a1afbb-ed57-4d8f-8955-836452b44155" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8fa14225e3cf480b83a74a752473090f", new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(6613), "AQAAAAIAAYagAAAAEEvrshuiqwTU4aYURPPyzGwlbUQWWFRi3xqLzuWxf9ySr6dFFbnKg3zLuw4sx+r4/g==", "2f98e6aa-2610-4c19-952c-aff0ede735b4" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7252));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7255));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7102));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7106));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7108));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 14, 8, 17, 14, 878, DateTimeKind.Utc).AddTicks(7109));
        }
    }
}
