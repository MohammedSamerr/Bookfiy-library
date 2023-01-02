using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookfiy_WepApp.Data.Migrations
{
    public partial class EditSubscribtion_tabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Subscriptions");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Subscriptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Subscriptions");

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Subscriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
