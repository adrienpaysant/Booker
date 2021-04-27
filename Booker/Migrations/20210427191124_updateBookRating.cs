using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class updateBookRating : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Book");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "Rating",
                table: "Book",
                type: "REAL",
                nullable: true);
        }
    }
}
