using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class updateUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "User");

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthor",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAuthor",
                table: "User");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
