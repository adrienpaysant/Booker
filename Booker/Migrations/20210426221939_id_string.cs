using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class id_string : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_AspNetUsers_BookerUserId1",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_BookerUserId1",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "BookerUserId1",
                table: "Book",
                newName: "Author");

            migrationBuilder.AlterColumn<string>(
                name: "BookerUserId",
                table: "Book",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.CreateIndex(
                name: "IX_Book_BookerUserId",
                table: "Book",
                column: "BookerUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Book_AspNetUsers_BookerUserId",
                table: "Book",
                column: "BookerUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Book_AspNetUsers_BookerUserId",
                table: "Book");

            migrationBuilder.DropIndex(
                name: "IX_Book_BookerUserId",
                table: "Book");

            migrationBuilder.RenameColumn(
                name: "Author",
                table: "Book",
                newName: "BookerUserId1");

            migrationBuilder.AlterColumn<int>(
                name: "BookerUserId",
                table: "Book",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

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
    }
}
