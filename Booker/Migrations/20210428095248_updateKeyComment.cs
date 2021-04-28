using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class updateKeyComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Book_BookId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_BookId",
                table: "Comments");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Comments_BookId",
                table: "Comments",
                column: "BookId");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Book_BookId",
                table: "Comments",
                column: "BookId",
                principalTable: "Book",
                principalColumn: "ISBN",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
