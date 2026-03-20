using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedJobCardForTracking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompletedSteps",
                table: "JobCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProgressNotes",
                table: "JobCards",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProgressPercentage",
                table: "JobCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedSteps",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "ProgressNotes",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "ProgressPercentage",
                table: "JobCards");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2672afe95d9849bc9e4c1cae34a73e55", new DateTime(2026, 3, 18, 1, 28, 58, 194, DateTimeKind.Utc).AddTicks(4271), "AQAAAAIAAYagAAAAEPc8SMy4YwRkNsit9Kl1bLTHk5VEumlL6NHnHMqw1a/qKirEz7s32XjgcR2UT+sghw==", "3e4cf77d-4f13-4bd7-8ddf-d3b2472ece14" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1e10955eedfd49f4ab4f6563129f9146", new DateTime(2026, 3, 18, 1, 28, 58, 248, DateTimeKind.Utc).AddTicks(7416), "AQAAAAIAAYagAAAAEPN82Zx8O3qGkCKNk2PktAntt7WoGL2P/P4hu0lgv7Qvy4BUm9Z/6aIFzbVjTe21KA==", "ad7205f5-d926-4c96-b992-dda31f46f528" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a225e94285e04e54a29a011743028437", new DateTime(2026, 3, 18, 1, 28, 58, 311, DateTimeKind.Utc).AddTicks(7306), "AQAAAAIAAYagAAAAEC/JrF6yBP0pQ+Ab+yOn5i6bwYllqeLruaKo5D85zT/EweNKOXRTptO4iVyboWSC+w==", "e08570dc-7900-4981-b477-3f2c8ab1a7ad" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f5bdc9037603476a8313b31734c451d5", new DateTime(2026, 3, 18, 1, 28, 58, 374, DateTimeKind.Utc).AddTicks(9911), "AQAAAAIAAYagAAAAEGwHjNblSBPHyN6hqcVUeFxQ7SfRZPR5iFDCRFN6DiyTm/FBgWOt1qvTTpfhKj0KtQ==", "c7c36a07-fc67-43c8-acec-19362d96a889" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8226009056694d48a6646d717b3bfe1e", new DateTime(2026, 3, 18, 1, 28, 58, 435, DateTimeKind.Utc).AddTicks(5881), "AQAAAAIAAYagAAAAEINSMKnW7e+IX/IEZww5e/33IfocTAXUstxZRPYN4WWznYBWNQsCCsuQWt5rGIySdg==", "0c0de73e-e620-4beb-a988-d53f7bd39238" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e464aa0ece2244dfa3d0751e40df546b", new DateTime(2026, 3, 18, 1, 28, 58, 490, DateTimeKind.Utc).AddTicks(3977), "AQAAAAIAAYagAAAAEFAofGG1lMjR+ACw6QJAtFoECXMIst8fAStAc8oAEqrKtnfBH83EmRIxujCZ/+pZAQ==", "65bd95b9-0ff3-4e57-9d4b-e17b60933d0a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "31b0973b6ccd4879b56140e62467ac70", new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6129), "AQAAAAIAAYagAAAAEEtCUXIoEkf9NaqjtPsUqRbCOVhG0NnjAUO3D7l26TDs4GCZ/d3ga9vvhACq2sWS8A==", "e5388d4d-ca86-4e57-81fa-4487b2029056" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6733));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6735));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6675));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6679));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6682));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 18, 1, 28, 58, 545, DateTimeKind.Utc).AddTicks(6685));
        }
    }
}
