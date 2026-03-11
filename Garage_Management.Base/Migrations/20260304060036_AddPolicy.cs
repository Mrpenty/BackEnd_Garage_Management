using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class AddPolicy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceWarrantyPolicy_ServiceWarrantyPolicyPolicyId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyServices_WarrantyPolicies_WarrantyPolicyId",
                table: "WarrantyServices");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantySpareParts_WarrantyPolicies_WarrantyPolicyId",
                table: "WarrantySpareParts");

            migrationBuilder.DropTable(
                name: "WarrantyPolicies");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceWarrantyPolicy",
                table: "ServiceWarrantyPolicy");

            migrationBuilder.DropColumn(
                name: "VehicleBrandId",
                table: "Appointments");

            migrationBuilder.RenameTable(
                name: "ServiceWarrantyPolicy",
                newName: "ServiceWarrantyPolicies");

            migrationBuilder.RenameColumn(
                name: "WarrantyPolicyId",
                table: "WarrantySpareParts",
                newName: "SparePartWarrantyPolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantySpareParts_WarrantyPolicyId",
                table: "WarrantySpareParts",
                newName: "IX_WarrantySpareParts_SparePartWarrantyPolicyId");

            migrationBuilder.RenameColumn(
                name: "WarrantyPolicyId",
                table: "WarrantyServices",
                newName: "ServiceWarrantyPolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantyServices_WarrantyPolicyId",
                table: "WarrantyServices",
                newName: "IX_WarrantyServices_ServiceWarrantyPolicyId");

            migrationBuilder.AddColumn<int>(
                name: "ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceWarrantyPolicies",
                table: "ServiceWarrantyPolicies",
                column: "PolicyId");

            migrationBuilder.CreateTable(
                name: "SparePartWarrantyPolicies",
                columns: table => new
                {
                    PolicyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PolicyName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DurationMonths = table.Column<int>(type: "int", nullable: true),
                    MileageLimit = table.Column<int>(type: "int", nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SparePartWarrantyPolicies", x => x.PolicyId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ed2fde03cb5846d4aa69e14c72039211", new DateTime(2026, 3, 4, 6, 0, 33, 770, DateTimeKind.Utc).AddTicks(1157), "AQAAAAIAAYagAAAAECjlHv3DiHwe2AvMCwH9e/hHqv4bb5Z514+LgYEyO3oNHDhlUQFKb+ft1v7LLga2zg==", "e9064c08-df3a-452e-a0f5-81ab4290a2af" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c0133430593f43c9833d4c4110ec29ec", new DateTime(2026, 3, 4, 6, 0, 33, 823, DateTimeKind.Utc).AddTicks(2525), "AQAAAAIAAYagAAAAEHSgs0rHXRVkZYh7QPlwnQrj3oXqXIQnIbSxKmgTrQPEAtTawq7g1H83Tq222gqSAQ==", "5ceebc9e-3a4e-4614-9e8e-e2ae8c6b10ac" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8828b6bb15684792988cc7f50db90f44", new DateTime(2026, 3, 4, 6, 0, 33, 877, DateTimeKind.Utc).AddTicks(5739), "AQAAAAIAAYagAAAAEKn1sbBfP/TmqiKtBOyOwHlDrle2H1L2NVYs7VQWczV3JK6CSKqm2v+1QhuBSQb+QQ==", "bc636c98-ab16-47ca-a8a9-6faeb912d571" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f659ee099560487582df77ec68b8fbb1", new DateTime(2026, 3, 4, 6, 0, 33, 931, DateTimeKind.Utc).AddTicks(6398), "AQAAAAIAAYagAAAAEKHXzADSk35gnfFAzyWejcnPeh9V6qPWhPo4VT+7bYVYGmYAQ+60KSM+CO8tWX02tQ==", "462f92c2-3f01-4369-95f5-07210a3b7518" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bf9429dd4a5642be8d252c1cf7e42765", new DateTime(2026, 3, 4, 6, 0, 33, 990, DateTimeKind.Utc).AddTicks(8878), "AQAAAAIAAYagAAAAEIAQ/nUZsvxDY8RhteQaJcQAggwkpXWqTnErdmd0oo4r7g/efI7xui0DDtpKlylHcg==", "a7d856ca-eb9d-4946-bb11-63d1e911239a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f1ae3002628f419f9507f792b8a22764", new DateTime(2026, 3, 4, 6, 0, 34, 45, DateTimeKind.Utc).AddTicks(4003), "AQAAAAIAAYagAAAAEHG7Sz83O4cshzzidiV6VHTIuV3onqLiul8WfzHq016RMf0vf96HiUctZkPn6PCyrw==", "c938f6ac-be53-4457-a76b-4eaeab7c1930" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f15f1c4bee1f44a0a9bf399f923eca7d", new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(4659), "AQAAAAIAAYagAAAAELTMcW8C7L8COSZKbYM1yHI1yiwZc+zvu8oLqMKk8pdENwJPSk6xy3yTHNrAcOiV3g==", "27608f83-7580-496c-a05b-0eaf4a580b27" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5283));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5285));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5173));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5177));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5180));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 4, 6, 0, 34, 108, DateTimeKind.Utc).AddTicks(5182));

            migrationBuilder.CreateIndex(
                name: "IX_WarrantyServices_ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices",
                column: "ServiceWarrantyPolicyPolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceWarrantyPolicies_ServiceWarrantyPolicyPolicyId",
                table: "Services",
                column: "ServiceWarrantyPolicyPolicyId",
                principalTable: "ServiceWarrantyPolicies",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyServices_ServiceWarrantyPolicies_ServiceWarrantyPolicyId",
                table: "WarrantyServices",
                column: "ServiceWarrantyPolicyId",
                principalTable: "ServiceWarrantyPolicies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyServices_ServiceWarrantyPolicies_ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices",
                column: "ServiceWarrantyPolicyPolicyId",
                principalTable: "ServiceWarrantyPolicies",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantySpareParts_SparePartWarrantyPolicies_SparePartWarrantyPolicyId",
                table: "WarrantySpareParts",
                column: "SparePartWarrantyPolicyId",
                principalTable: "SparePartWarrantyPolicies",
                principalColumn: "PolicyId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Services_ServiceWarrantyPolicies_ServiceWarrantyPolicyPolicyId",
                table: "Services");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyServices_ServiceWarrantyPolicies_ServiceWarrantyPolicyId",
                table: "WarrantyServices");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantyServices_ServiceWarrantyPolicies_ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices");

            migrationBuilder.DropForeignKey(
                name: "FK_WarrantySpareParts_SparePartWarrantyPolicies_SparePartWarrantyPolicyId",
                table: "WarrantySpareParts");

            migrationBuilder.DropTable(
                name: "SparePartWarrantyPolicies");

            migrationBuilder.DropIndex(
                name: "IX_WarrantyServices_ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServiceWarrantyPolicies",
                table: "ServiceWarrantyPolicies");

            migrationBuilder.DropColumn(
                name: "ServiceWarrantyPolicyPolicyId",
                table: "WarrantyServices");

            migrationBuilder.RenameTable(
                name: "ServiceWarrantyPolicies",
                newName: "ServiceWarrantyPolicy");

            migrationBuilder.RenameColumn(
                name: "SparePartWarrantyPolicyId",
                table: "WarrantySpareParts",
                newName: "WarrantyPolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantySpareParts_SparePartWarrantyPolicyId",
                table: "WarrantySpareParts",
                newName: "IX_WarrantySpareParts_WarrantyPolicyId");

            migrationBuilder.RenameColumn(
                name: "ServiceWarrantyPolicyId",
                table: "WarrantyServices",
                newName: "WarrantyPolicyId");

            migrationBuilder.RenameIndex(
                name: "IX_WarrantyServices_ServiceWarrantyPolicyId",
                table: "WarrantyServices",
                newName: "IX_WarrantyServices_WarrantyPolicyId");

            migrationBuilder.AddColumn<int>(
                name: "VehicleBrandId",
                table: "Appointments",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServiceWarrantyPolicy",
                table: "ServiceWarrantyPolicy",
                column: "PolicyId");

            migrationBuilder.CreateTable(
                name: "WarrantyPolicies",
                columns: table => new
                {
                    WarrantyPolicyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<int>(type: "int", nullable: true),
                    PolicyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    WarrantyMonths = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WarrantyPolicies", x => x.WarrantyPolicyId);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "96760c4a9d0f4b2aadad624fea2dd1fc", new DateTime(2026, 3, 1, 8, 50, 46, 410, DateTimeKind.Utc).AddTicks(3585), "AQAAAAIAAYagAAAAELSKKe+wI7s8YXGiHejNL83z30MGQc9abyQ5yVVYN9KKv1/ZDMl3CA5sN/KJ9YWXIw==", "81ee9518-5e1d-4cd9-8c6a-837c1bb34eef" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "429e2b5d8eb54aa492173111ed971cd5", new DateTime(2026, 3, 1, 8, 50, 46, 469, DateTimeKind.Utc).AddTicks(6188), "AQAAAAIAAYagAAAAEC+aW+lKtqwP0F5lsXe6+uTOMidBuWzcK+jzsZDBGq0RzFBYGT4hhOJRGCWMNT1GCg==", "31884dba-7346-4b61-aa5c-5c41c9b62adf" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "98919dc8879d41fc8bd4ba80b88ef008", new DateTime(2026, 3, 1, 8, 50, 46, 531, DateTimeKind.Utc).AddTicks(8543), "AQAAAAIAAYagAAAAEIYaLLBvwxyz7cbHzANujnIVWHHg8p21u7Ek0A52ikjfeVRoFqdqFtnPQMAjvO1Uag==", "467aeab6-f1a9-4dd7-b48a-bb4c6857f21a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bfe011a58c1743be9db13f49abf7fabd", new DateTime(2026, 3, 1, 8, 50, 46, 594, DateTimeKind.Utc).AddTicks(2384), "AQAAAAIAAYagAAAAEI+ByxfPpGsAqxrctn3cbC9v+FB4r9rgda9dFA6g9951IdzM3y4xMdwFfHXlGjIn5g==", "267a033e-0d13-4850-8e4e-8138f81aa99a" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fe2eed2076fe49879825eca0c451e6f5", new DateTime(2026, 3, 1, 8, 50, 46, 658, DateTimeKind.Utc).AddTicks(6623), "AQAAAAIAAYagAAAAEL1fvy5uB50Pn8w9gX46EW6l6utDcDjSCrCBViAHx9caW5dS0EK4qkISLtlyqRpS8Q==", "338118f9-1992-49aa-8975-3a8343fbc14c" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "57e2167e6cd041dd93ee1a83272e16d2", new DateTime(2026, 3, 1, 8, 50, 46, 725, DateTimeKind.Utc).AddTicks(5623), "AQAAAAIAAYagAAAAEFdCKi/sK4lgwL3oIKz/QWk1H/ruk/VRjUqrdDGiL8uCfKk92XKY1zXKcmBSmGwJDA==", "ce7bacaa-482a-4fc9-a161-9af10ab166c0" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b0d66d0cfa1949c9a13dae638ca8b21f", new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(1514), "AQAAAAIAAYagAAAAEN1Lpw2j+vzE9KWP5rkHYB317/rWhOr29pkX/z08GUV2Md4WjOnhmazDd7zOGhq7vQ==", "3e2285c9-8010-441d-9712-df3d75d10578" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3809));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3812));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3595));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3601));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3604));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 3, 1, 8, 50, 46, 822, DateTimeKind.Utc).AddTicks(3607));

            migrationBuilder.AddForeignKey(
                name: "FK_Services_ServiceWarrantyPolicy_ServiceWarrantyPolicyPolicyId",
                table: "Services",
                column: "ServiceWarrantyPolicyPolicyId",
                principalTable: "ServiceWarrantyPolicy",
                principalColumn: "PolicyId");

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantyServices_WarrantyPolicies_WarrantyPolicyId",
                table: "WarrantyServices",
                column: "WarrantyPolicyId",
                principalTable: "WarrantyPolicies",
                principalColumn: "WarrantyPolicyId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_WarrantySpareParts_WarrantyPolicies_WarrantyPolicyId",
                table: "WarrantySpareParts",
                column: "WarrantyPolicyId",
                principalTable: "WarrantyPolicies",
                principalColumn: "WarrantyPolicyId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
