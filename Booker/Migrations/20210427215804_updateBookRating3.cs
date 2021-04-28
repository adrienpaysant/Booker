using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class updateBookRating3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rating_Book_BookId",
                table: "Rating");

            migrationBuilder.DropIndex(
                name: "IX_Rating_BookId",
                table: "Rating");

            migrationBuilder.AddColumn<int>(
                name: "NumberOfRater",
                table: "Book",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RatingSum",
                table: "Book",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRater",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "RatingSum",
                table: "Book");

            migrationBuilder.CreateIndex(
                name: "IX_Rating_BookId",
                table: "Rating",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rating_Book_BookId",
                table: "Rating",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
