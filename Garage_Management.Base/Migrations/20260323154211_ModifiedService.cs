using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BasePrice",
                table: "Services",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "de58371893e6451a8a84d6fb4f0b6580", new DateTime(2026, 3, 23, 15, 42, 8, 870, DateTimeKind.Utc).AddTicks(7025), "AQAAAAIAAYagAAAAEFrF91JBbm3vwlZdPO56rmvPgkuvAs4gtR0JPdPpsmu99MnEt/TWImGP5/uc+7EMMA==", "76ce8d64-aa45-4d22-8a7a-c846137fb1a1" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a721ea29174e4ae699083bc6f4a5e58e", new DateTime(2026, 3, 23, 15, 42, 8, 926, DateTimeKind.Utc).AddTicks(2681), "AQAAAAIAAYagAAAAEFFW2psjfgNie/xcjSeOa/evqw+AQJzMB4FMMTMRQ9nOyD0R/dlvivs1bd7T9I62ew==", "898521f6-f158-4444-8c18-30c9a20fdfb0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ebccecab256d4db389277e6877432894", new DateTime(2026, 3, 23, 15, 42, 8, 981, DateTimeKind.Utc).AddTicks(2895), "AQAAAAIAAYagAAAAELQtQRjaplA9Ze9BJcLWM5hVJfClsJQGmIbBKVt1DseaoUFU3ANh/TDZ4t1HXkN4Xw==", "5a67d6c0-059a-49bd-8f26-79cddeed7b5d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13dd1b25d72b4aea82bfaa4645315319", new DateTime(2026, 3, 23, 15, 42, 9, 37, DateTimeKind.Utc).AddTicks(6705), "AQAAAAIAAYagAAAAEL7Tgf8lYbAmLm4cMGTEErke3p1Q9574TN90bCQslDyolZ8KIULH07gPRmP7lgbdNQ==", "ae64c70e-3f29-4821-8d1a-4913cce45304" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b227d0ca770e4abf8beea9907ea449ce", new DateTime(2026, 3, 23, 15, 42, 9, 91, DateTimeKind.Utc).AddTicks(8773), "AQAAAAIAAYagAAAAELaKU+1v3U9vWS6OkOIbAOqxNdGPLwBtStTnluy0WUan1mHL2vTVWNs+TU8xHSZPVQ==", "af873906-6503-45a5-a42f-6c46eb03da1a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a89a5f6c6c654260ac4e469ae8d3578b", new DateTime(2026, 3, 23, 15, 42, 9, 148, DateTimeKind.Utc).AddTicks(1475), "AQAAAAIAAYagAAAAENdqOGEBQxhWin6gV7ncjrrXHuuoQ9ymeQLAPB0sx46SwYzdzHZTUMxbSDGU9V6t8A==", "9fc5fce3-8630-4b4e-a900-f3b9df204a52" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "492042140c5242179278c49603e8b792", new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1479), "AQAAAAIAAYagAAAAEBZVqc3950RAUmNKE0uvSirNiR6Pjk4P1gloV10TnAol8OAF09MoKe8DOaRMrOddcw==", "75f12ab1-415e-419d-ac69-59f9d2d01916" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1917));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1919));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1857));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1861));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1864));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 23, 15, 42, 9, 202, DateTimeKind.Utc).AddTicks(1866));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "BasePrice",
                table: "Services",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f186683084e24f4888b8c7ba8f2dd4ed", new DateTime(2026, 3, 18, 2, 47, 33, 555, DateTimeKind.Utc).AddTicks(7140), "AQAAAAIAAYagAAAAEILBgDBicxfFjSkenXUECgdvu5GPFso/hHTQAwz7hFAZ9S3t210zWKXvM/+p5dfttQ==", "3deea86c-11b8-47a7-aff5-586516813920" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a0cfb3838e684d43a4bad28785ae19fd", new DateTime(2026, 3, 18, 2, 47, 33, 622, DateTimeKind.Utc).AddTicks(4763), "AQAAAAIAAYagAAAAEHF8cwYNUbCP6bWf0uXj8vQ8VYggx9i7JmYgDl4IDIhOkoJtLhePl/V5h4W+NWYZUw==", "1883f649-3e39-433a-8634-5ea6cbe9ecfe" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4264a06b79654f9cb71e5d938831a1a0", new DateTime(2026, 3, 18, 2, 47, 33, 681, DateTimeKind.Utc).AddTicks(1149), "AQAAAAIAAYagAAAAEJsO06iwwQtCKg/79MUKCAbSCxRuDQTvUbNVWZB6hxWolsksVB5fOke0kze1Ga3CEw==", "2b4d9044-b4ef-4af8-a928-17cce1a97745" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "74e586e200fb4e828c3dac552968de96", new DateTime(2026, 3, 18, 2, 47, 33, 743, DateTimeKind.Utc).AddTicks(4236), "AQAAAAIAAYagAAAAEDwNakStprgYcHmeh9EQ9GJ711FY/Mo4W4KRficgbWSa5K5f3yfYOZigwDJPIR1h+A==", "2c21da87-d3c7-46a6-8707-29f93e214a96" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b41d20de607f4f31a52f2a1a30231f1f", new DateTime(2026, 3, 18, 2, 47, 33, 807, DateTimeKind.Utc).AddTicks(4593), "AQAAAAIAAYagAAAAEIjsxO+N+xeejkTku/LGquqS6Gqo/U4hbVF6JIF94rvbYWniQkTpzj87Qi2RpBu9Qg==", "10a99442-46cf-4e49-ab73-a29be5600007" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a8293e81da594ba58bb2e34d4478b104", new DateTime(2026, 3, 18, 2, 47, 33, 871, DateTimeKind.Utc).AddTicks(5115), "AQAAAAIAAYagAAAAELAOPYUOAnK8yrfySswjRovVg2DVT8SXdLwQ66Q7lZ85hJyTe7DNWC9x3QCLkdzSvg==", "0edf89a0-8268-4d2f-90fc-034062aabb2b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "93fefc0f06af4aad85786d579fd4bbf0", new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(4396), "AQAAAAIAAYagAAAAEDygGmI3DGEdgkOavhOehIZzdyw0Joy5yy/pAVnVIEDwYmvqSb7pmxVfZ5posbGGvg==", "a07454b8-af2e-4ec8-9496-ea312f88eb42" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5068));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5071));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5010));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5014));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5017));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 2, 47, 33, 934, DateTimeKind.Utc).AddTicks(5019));
        }
    }
}
