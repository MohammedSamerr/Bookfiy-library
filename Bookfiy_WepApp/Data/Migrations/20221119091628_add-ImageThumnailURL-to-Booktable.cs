using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookfiy_WepApp.Data.Migrations
{
    public partial class addImageThumnailURLtoBooktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageThumnailURL",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageThumnailURL",
                table: "Books");
        }
    }
}
