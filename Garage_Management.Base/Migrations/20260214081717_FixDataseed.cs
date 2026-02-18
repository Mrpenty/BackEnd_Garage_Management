using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class FixDataseed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 4, 4 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 3, 2 },
                    { 4, 3 },
                    { 2, 4 }
                });

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
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "8852e1d58ad14077863a621a5537429b", new DateTime(2026, 2, 14, 8, 17, 14, 573, DateTimeKind.Utc).AddTicks(3829), "supervisor01@garage.vn", "SUPERVISOR01@GARAGE.VN", "SUPERVISOR01", "AQAAAAIAAYagAAAAEP8FYhbR44jOdGkL+cPSfVE9ZldeUxwg/2nf08iwsnPmguryhGh11HrLeryQiUFGWg==", "1ef7b7e0-4bda-4a26-a7e4-2e18ea0d878d", "Supervisor01" });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 4, 3 });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 2, 4 });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { 2, 2 },
                    { 3, 3 },
                    { 4, 4 }
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "175b366f59f54084925bbb14d1d93232", new DateTime(2026, 2, 10, 15, 33, 28, 683, DateTimeKind.Utc).AddTicks(6494), "AQAAAAIAAYagAAAAEJtVgXIFcef1l1DGQTcUcdiYb2P8Jmm1mLskvpxV+3tNP03nJVAlI9RxQN/ubxnOMQ==", "691a4948-4f58-4743-98a6-826aad318023" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "SecurityStamp", "UserName" },
                values: new object[] { "799c3e3a1f044c209153f0393f368dea", new DateTime(2026, 2, 10, 15, 33, 28, 740, DateTimeKind.Utc).AddTicks(8269), "manager01@garage.vn", "MANAGER01@GARAGE.VN", "MANAGER01", "AQAAAAIAAYagAAAAEIuGAgtJl+yUzA0TrYXY+sSWvJZUd7Ppm4zT5XknK9xTGMDcv5cfXU+K0KHcPtJP9A==", "018da9d0-ec6b-4218-a9a8-4f060c117ad4", "manager01" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ef5a89c2bf8a460ba13c89676b8175fa", new DateTime(2026, 2, 10, 15, 33, 28, 799, DateTimeKind.Utc).AddTicks(1361), "AQAAAAIAAYagAAAAEBxSO4+lVk+pSkdWUUZ7OAXq57yJNA7IlJ2pUwf+wHt5fd3yJJALSlDNeE11Qnl9RQ==", "3f9fb323-b2c2-467c-bd9c-bb0e67257c1c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "70a4bd5df7fb4e03bde1ca778d866ffa", new DateTime(2026, 2, 10, 15, 33, 28, 860, DateTimeKind.Utc).AddTicks(7222), "AQAAAAIAAYagAAAAELMvS+b2OAih+1k9SvY3rYcRDrvEIEyYGlBIF+HInkTaR0AQUWroDrPFuF1LFF3G/g==", "b45ed00c-b4f5-4825-b53b-1087c57bfaaa" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "59994a845e854d33b7c6131f5027d079", new DateTime(2026, 2, 10, 15, 33, 28, 926, DateTimeKind.Utc).AddTicks(3505), "AQAAAAIAAYagAAAAEARqj06+zffO+Q2Een90JpaMTuXRiN6GLUEShSLTchkKT3mgZ3+IJGN2WD6wx8fQlg==", "32af64b2-c0d1-4509-84d2-688162e1d6bf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "0a119755407a4fd7880c9fbaa532c293", new DateTime(2026, 2, 10, 15, 33, 28, 983, DateTimeKind.Utc).AddTicks(8900), "AQAAAAIAAYagAAAAEJuBdgv2kN2a0k0gguiPHhF+nQYX0ZTO0zRHeZUC2EgRe4fvfuzy6in5nGCPxfJ8Cg==", "7ccaa8e5-a736-4618-8e6b-4e29b1e9cb88" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "face88333e8d49bc947fa0fff4aa420f", new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(6261), "AQAAAAIAAYagAAAAEPXTgCH8SfsftYrnboUnyPGemgeO8qUQyv5NzLOQ9rgESS1CnxQdyZOxSe7I2bClLQ==", "8b656b39-df33-462d-b4d8-df6a20da1a20" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7692));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7696));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7593));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7599));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7602));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 10, 15, 33, 29, 45, DateTimeKind.Utc).AddTicks(7605));
        }
    }
}
