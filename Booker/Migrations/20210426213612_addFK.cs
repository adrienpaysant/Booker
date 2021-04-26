using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class addFK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Book",
                newName: "BookerUserId1");

            migrationBuilder.AddColumn<int>(
                name: "BookerUserId",
                table: "Book",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Book_BookerUserId1",
                table: "Book",
                column: "BookerUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_AspNetUsers_BookerUserId1",
                table: "Book",
                column: "BookerUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_AspNetUsers_BookerUserId1",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_BookerUserId1",
                table: "Book");

            migrationBuilder.DropColumn(
                name: "BookerUserId",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "BookerUserId1",
                table: "Book",
                newName: "Author");
        }
    }
}
