using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Garage_Management.Base.Migrations
{
    /// <inheritdoc />
    public partial class ModifyRepairEstimate_AddStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "RepairEstimateSpareParts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "RepairEstimateServices",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "RepairEstimates",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEPfQKLMFK69MP+sTURwW1hFrRgHgK2iAhf7YRZKtyBhW9EWsz3lSMO7nkpIq6yvzIA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEJ4ApK+PRo8vi05Tgmqp01TkTfwVk/WjJpaCUNQJ+plKTNh6CimW0SebeOw2QBVXKA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEI2sid1I2mbeFqYuuYcGwj3sX7DFfe3RyhsIYWcWp3TgVQ9UdeRFrlWbODv6JSA9uA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEDso2E2J71N2jkp4Cu3kVhiNpgqK8cUEB1d5crSmbGqjQ9lYp9Ljj4UDr3LkPuP/7w==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIb0kt9MxpFm0z1TkSK9FvPLYJYwYlv2vg992CItwCUQoC+tNCJqMKToRPDn+c9g2Q==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEKxJna+ttvKoaHLRF9f7Iu2WdMt2kAxRWlGSul1HG7HmeJZ9IN5LnT2LyKzkzcMMlA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE0henamH55ggpXJVMrdwkSdvyUkl7aCsTIj/+m85IeI0xrWJDflfMZDMVS/BR3oCg==");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "status",
                table: "RepairEstimateSpareParts");

            migrationBuilder.DropColumn(
                name: "status",
                table: "RepairEstimateServices");

            migrationBuilder.DropColumn(
                name: "status",
                table: "RepairEstimates");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEIlv2tGQGnRSXMPEvMxd8d9IDrJGLPKXsJxsGp9d/n7UQUCzAikKf0sp++b1noFRTQ==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEMv3LiO7QhvQmGijcCnnxc6uwNGOP9+AsX6JyrwD6B091nzVgTAQD7utVkuMlPPfIg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 3,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEEWKxqsqvjs8WBRMbflS+ld/jPc7GKp7ndhv3mEm8fjBaQTRNYmLeVklXK+StPZ2qw==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 4,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEOgLc5nnmhkXlEBP4ofBrYbwPlzCpBtdHLbD7JpsVUvvEn8yzfkA/6GokoFpwhUXAA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 5,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECq6RvSJCYxhle4dNgG8gIJyQDbO7dkWaoAV3Tn51n60wS9gOkY1w3lnaHEEHk7CBg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 10,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAECzxIQs1FH4OifGHz+IXNoAhwTuVxA0Y0IrV8O8Dzu86KLyPEYaLmFW90p7pDbZ3kg==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 11,
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEE8JQ1xlHDACihjFOTQqaJQwkpovzqGhLYPbmauiJ84DK95zrm7Ny95GLBlSpxxBmQ==");
        }
    }
}
