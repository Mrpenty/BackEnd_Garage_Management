using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedInventori : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "StockTransactions",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4a0b0679853049c5aaedc3e0f359f899", new DateTime(2026, 3, 24, 7, 24, 18, 266, DateTimeKind.Utc).AddTicks(5484), "AQAAAAIAAYagAAAAEEGw4sPiYqAtC862JnfZ7jiS0iX+TPOu6XX3cKKuI6zT0z4d1dPf8pbsj2bslJ+nNw==", "ed395f57-6723-4d51-a6e3-3f0287c0cef5" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ca7a984c1a2040318d0b85d736733fcd", new DateTime(2026, 3, 24, 7, 24, 18, 323, DateTimeKind.Utc).AddTicks(5051), "AQAAAAIAAYagAAAAEFqKpSF4RWKPUDQdSZvovkLpVqqD8+0WGxHtEyzXNwtAs0CS7iC2Unnmx16P+mSmSw==", "711ef9f0-3d4f-4cab-88cd-dbbbad0d997b" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bc7187c8f7a84fab8e934da9416e5768", new DateTime(2026, 3, 24, 7, 24, 18, 378, DateTimeKind.Utc).AddTicks(6980), "AQAAAAIAAYagAAAAEOEkvGuulAwIFx26pV7RMJ9ekIqmiDqE2Xz74qCusixooUdHVCywXVTgP2k23MOwpQ==", "3a7ea82d-69e6-4587-abbc-cb9f00ad8b57" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf1228eea90a4d92bf547c518b3525e2", new DateTime(2026, 3, 24, 7, 24, 18, 433, DateTimeKind.Utc).AddTicks(5950), "AQAAAAIAAYagAAAAEO7z5T2/OPAgQIRQ0anBqSltDjSPvIlDMtuCxHgWb/WAyI3cTvaDB27Qm7bmEKi7Og==", "acae21f0-577a-42e4-b326-57bf100028db" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4c40c8c678e64ed4afe4bf7692a9c3c1", new DateTime(2026, 3, 24, 7, 24, 18, 493, DateTimeKind.Utc).AddTicks(4341), "AQAAAAIAAYagAAAAEFNpaojbeEBdqeUor4BPqqUge/G3fo8V2GFAtRouPZHNr9q7SB7eLZgyIfvCGphCQg==", "0cbf78df-6a47-4177-94b7-eb9cdffade09" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3151966263d84ba1808036e3fa00e628", new DateTime(2026, 3, 24, 7, 24, 18, 550, DateTimeKind.Utc).AddTicks(318), "AQAAAAIAAYagAAAAELB+bUYZZTZdXgUkmst8/B8qfLB7vNK2KYWdPfM+2/RtoylfvNNpsJ62qRn8gQm+Bg==", "eb00b249-569e-4a3d-a4f5-d2d9652943a8" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1dd18966c76b433195afcc4ade8a9d31", new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8387), "AQAAAAIAAYagAAAAEAeElfsFiz+XOGz9nwNeSckugxf4hdiLyFC/JJ4dS2AaHJ1/R0Ngfhff4Aa1Or0T6g==", "2a3a2090-03f3-48b3-b2b3-6da4ea8118f9" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8801));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8803));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8760));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8764));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8766));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 24, 7, 24, 18, 612, DateTimeKind.Utc).AddTicks(8768));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reason",
                table: "StockTransactions");

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
    }
}
