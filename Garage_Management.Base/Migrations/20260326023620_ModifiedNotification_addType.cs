using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedNotification_addType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 414, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEIlv2tGQGnRSXMPEvMxd8d9IDrJGLPKXsJxsGp9d/n7UQUCzAikKf0sp++b1noFRTQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 467, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEMv3LiO7QhvQmGijcCnnxc6uwNGOP9+AsX6JyrwD6B091nzVgTAQD7utVkuMlPPfIg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 519, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEEWKxqsqvjs8WBRMbflS+ld/jPc7GKp7ndhv3mEm8fjBaQTRNYmLeVklXK+StPZ2qw==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 572, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEOgLc5nnmhkXlEBP4ofBrYbwPlzCpBtdHLbD7JpsVUvvEn8yzfkA/6GokoFpwhUXAA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 625, DateTimeKind.Utc), "AQAAAAIAAYagAAAAECq6RvSJCYxhle4dNgG8gIJyQDbO7dkWaoAV3Tn51n60wS9gOkY1w3lnaHEEHk7CBg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 679, DateTimeKind.Utc), "AQAAAAIAAYagAAAAECzxIQs1FH4OifGHz+IXNoAhwTuVxA0Y0IrV8O8Dzu86KLyPEYaLmFW90p7pDbZ3kg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc), "AQAAAAIAAYagAAAAEE8JQ1xlHDACihjFOTQqaJQwkpovzqGhLYPbmauiJ84DK95zrm7Ny95GLBlSpxxBmQ==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Channel",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Notifications");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 414, DateTimeKind.Utc).AddTicks(4689), "AQAAAAIAAYagAAAAEGFxhka/WP4rztLGaJDJUFT0etzu3mImoVa6Un9eif7892kQSHQ4HJfTVBCCVNcS5Q==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 467, DateTimeKind.Utc).AddTicks(1059), "AQAAAAIAAYagAAAAEHbj+VuGH46rtJNgIahubH9c+F3MmR6VV11EjSCFnh9dytaWCs++oU5c6EMnNLGZYA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 519, DateTimeKind.Utc).AddTicks(4962), "AQAAAAIAAYagAAAAECvDial0xApQljYNbl9EzUJntOOfnEXaehy+O7ElVvH0lcusMSdWC6WCV6sHcFiEHQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 572, DateTimeKind.Utc).AddTicks(1838), "AQAAAAIAAYagAAAAEGNOq66PfTFMPyds5bovLLTNbPbFY2HtBwvCnDsRmnLTyJmt8PtDZQDycHWbBg4nOQ==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 625, DateTimeKind.Utc).AddTicks(4623), "AQAAAAIAAYagAAAAEGnCXVjqAlAKyz51KUwdA+cz6VktM2vCF3c4AlEYAyLQZKV8fIGYAcO5b9ItoBIOFg==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 679, DateTimeKind.Utc).AddTicks(438), "AQAAAAIAAYagAAAAEOwTn6kgjx+07kPCCUFxMWoLgpBqxxHspues/SF0ruY8NwPapLbBd0GQq3e35Kt4DA==" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "CreatedAt", "PasswordHash" },
                values: new object[] { new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(6836), "AQAAAAIAAYagAAAAEPih9qVBDsMZfOJYvrt0AJD2zHVrJ4y/c6X4NIXZuqy/sm6dshbHJ3ZJR9J8BiUXsg==" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7216));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7219));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7172));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7176));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7178));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 25, 7, 28, 5, 732, DateTimeKind.Utc).AddTicks(7181));
        }
    }
}
