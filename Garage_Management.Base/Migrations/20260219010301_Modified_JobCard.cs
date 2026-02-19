using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class Modified_JobCard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Employees_CreatedByUserEmployeeId",
                table: "JobCards");

            migrationBuilder.DropIndex(
                name: "IX_WorkBay_JobcardId",
                table: "WorkBay");

            migrationBuilder.DropIndex(
                name: "IX_JobCards_CreatedByUserEmployeeId",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "CreatedByUserEmployeeId",
                table: "JobCards");

            migrationBuilder.RenameColumn(
                name: "CreatedByEmployeeId",
                table: "JobCards",
                newName: "EmployeeId");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b00a05e6f74c4e6ca405cc3eb637bc93", new DateTime(2026, 2, 19, 1, 2, 58, 444, DateTimeKind.Utc).AddTicks(6216), "AQAAAAIAAYagAAAAENwxlQatuF81XWEBWRAymlGrp9QzpjQvuJ0zTd92GYAYYK7/C5WatjbX63jvSuN0Lg==", "3aeff363-7886-497e-a513-5cd59d0b7298" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "22bbdb7c88654aeeb51ee423cf7d0f98", new DateTime(2026, 2, 19, 1, 2, 58, 503, DateTimeKind.Utc).AddTicks(1342), "manager01@garage.vn", "MANAGER01@GARAGE.VN", "AQAAAAIAAYagAAAAEPBtZDb/lHOHLNyqEGu5SjxfxInVZ1yCvhpifThxZ6JgrV0Jfd8B9t9+CFJ9/DIGSg==", "fad7b8da-2533-4dc9-98aa-0906eb7ff828" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ad0e8c7627a64c21925ac0d49bb8569e", new DateTime(2026, 2, 19, 1, 2, 58, 556, DateTimeKind.Utc).AddTicks(8583), "AQAAAAIAAYagAAAAEC3PdonQhyyjxx68GiPKl2hyJQO+9joL1gPGPQy/Zb019bNWPLOAZeisApCHll2H8w==", "d9b6a6be-003b-4ef9-be4a-792875fd9083" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8cf68ad392ac45419ddcaf39806735aa", new DateTime(2026, 2, 19, 1, 2, 58, 613, DateTimeKind.Utc).AddTicks(4210), "AQAAAAIAAYagAAAAEEzCBkbNJzU1WVjlVYaFUJZjboCWUE92uoFzcd/FVCIU6ZgL8vrSGHwHbLkqiNLeBg==", "72251bca-ba5e-48e0-9a50-cc481f3ea3e9" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "08e36691246a4f9cacbe771b19fb3528", new DateTime(2026, 2, 19, 1, 2, 58, 668, DateTimeKind.Utc).AddTicks(9822), "AQAAAAIAAYagAAAAEG4TAiRkCe4HOuJue/Hjs/r9k7Z5nPGYiH/5phCmUIe52pgvA/8NP99x5UwBKSkFcw==", "697d2ef7-e0ad-493e-9d32-134227265248" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2409bddaec0443d3b48f786513c45267", new DateTime(2026, 2, 19, 1, 2, 58, 725, DateTimeKind.Utc).AddTicks(1507), "AQAAAAIAAYagAAAAEDCoH6hJUnTFT7pp8TMyY8aZT3CzTFQQNbZPQzlxSwGkNFWo3vpC8PlBiW1C4wOBEw==", "a071d314-02ef-4f46-8275-1c31aca992e7" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c7dc95be5b7d4880a95956395ea0edd5", new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2130), "AQAAAAIAAYagAAAAEJszMwCLviIewzot5LXiTmI2pw8p9xwh3P73U2Gdc3pnQZVDRfxXRRF7cic1XLj4Cw==", "a1526484-c073-4623-81de-ee6b44f905ad" });

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2961));

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "CustomerId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2963));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2886));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2890));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2892));

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "EmployeeId",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTime(2026, 2, 19, 1, 2, 58, 780, DateTimeKind.Utc).AddTicks(2895));

            migrationBuilder.CreateIndex(
                name: "IX_WorkBay_JobcardId",
                table: "WorkBay",
                column: "JobcardId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCards_CreatedBy",
                table: "JobCards",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_JobCards_EmployeeId",
                table: "JobCards",
                column: "EmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Employees_CreatedBy",
                table: "JobCards",
                column: "CreatedBy",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Employees_EmployeeId",
                table: "JobCards",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Employees_CreatedBy",
                table: "JobCards");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Employees_EmployeeId",
                table: "JobCards");

            migrationBuilder.DropIndex(
                name: "IX_WorkBay_JobcardId",
                table: "WorkBay");

            migrationBuilder.DropIndex(
                name: "IX_JobCards_CreatedBy",
                table: "JobCards");

            migrationBuilder.DropIndex(
                name: "IX_JobCards_EmployeeId",
                table: "JobCards");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "JobCards",
                newName: "CreatedByEmployeeId");

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserEmployeeId",
                table: "JobCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

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
                columns: new[] { "ConcurrencyStamp", "CreatedAt", "Email", "NormalizedEmail", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c6cafe3c12ea415c964841af8ec53e50", new DateTime(2026, 2, 18, 9, 49, 44, 40, DateTimeKind.Utc).AddTicks(8443), "supervisor01@garage.vn", "SUPERVISOR01@GARAGE.VN", "AQAAAAIAAYagAAAAEAwEbiir3/VPM5TzCbOSnFwqQeGOAt7odNVro16i93vxU9KTJTqr4hiNB1qE5LYCzw==", "ba4790c0-b787-4e54-96c8-2181a6f46e0e" });

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

            migrationBuilder.CreateIndex(
                name: "IX_WorkBay_JobcardId",
                table: "WorkBay",
                column: "JobcardId",
                unique: true,
                filter: "[JobcardId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_JobCards_CreatedByUserEmployeeId",
                table: "JobCards",
                column: "CreatedByUserEmployeeId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Employees_CreatedByUserEmployeeId",
                table: "JobCards",
                column: "CreatedByUserEmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
