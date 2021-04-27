using Microsoft.EntityFrameworkCore.Migrations;

namespace Booker.Migrations
{
    public partial class updateRatings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Rating",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BookerUserId = table.Column<string>(type: "TEXT", nullable: true),
                    Value = table.Column<int>(type: "INTEGER", nullable: false),
                    BookId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rating", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rating_Book_BookId",
                        column: x => x.BookId,
                        principalTable: "Book",
                        principalColumn: "ISBN",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Rating_BookId",
                table: "Rating",
                column: "BookId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Rating");
        }
    }
}
