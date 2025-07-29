using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraFlow.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ISBN", "IsCheckedOut" },
                values: new object[] { "0-8479-9529-1", true });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ISBN", "IsCheckedOut" },
                values: new object[] { "0-1500-2863-6", true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ISBN", "IsCheckedOut" },
                values: new object[] { "", false });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "ISBN", "IsCheckedOut" },
                values: new object[] { "", false });
        }
    }
}
