﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookfiy_WepApp.Data.Migrations
{
    public partial class addImagePublicIdtoBooktable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePublicId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePublicId",
                table: "Books");
        }
    }
}
