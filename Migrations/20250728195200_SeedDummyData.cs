using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LibraFlow.Migrations
{
    /// <inheritdoc />
    public partial class SeedDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Books",
                columns: new[] { "Id", "Author", "ISBN", "IsCheckedOut", "Title" },
                values: new object[,]
                {
                    { 1, "Andrew Hunt", "", false, "The Pragmatic Programmer" },
                    { 2, "Robert C. Martin", "", false, "Clean Code" }
                });

            migrationBuilder.InsertData(
                table: "Members",
                columns: new[] { "Id", "Email", "Name" },
                values: new object[,]
                {
                    { 1, "", "Alice Smith" },
                    { 2, "", "Bob Johnson" }
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "BookId", "CheckedOutDate", "MemberId", "ReturnDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2024, 7, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, null },
                    { 2, 2, new DateTime(2024, 7, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, new DateTime(2024, 7, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_BookId",
                table: "Loans",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Loans_MemberId",
                table: "Loans",
                column: "MemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Members_MemberId",
                table: "Loans",
                column: "MemberId",
                principalTable: "Members",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Books_BookId",
                table: "Loans");

            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Members_MemberId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_BookId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_MemberId",
                table: "Loans");

            migrationBuilder.DeleteData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Loans",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Members",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
