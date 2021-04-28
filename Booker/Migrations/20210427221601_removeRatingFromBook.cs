using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class removeRatingFromBook : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOfRater",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "RatingSum",
                table: "Book");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
