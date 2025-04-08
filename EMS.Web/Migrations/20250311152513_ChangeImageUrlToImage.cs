using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class ChangeImageUrlToImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename ImageUrl to Image
            migrationBuilder.Sql("ALTER TABLE `Events` CHANGE `ImageUrl` `Image` VARCHAR(255);");

            // Change Time column type
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "Time",
                table: "Events",
                type: "time",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time(6)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Rename Image back to ImageUrl
            migrationBuilder.Sql("ALTER TABLE `Events` CHANGE `Image` `ImageUrl` VARCHAR(255);");

            // Revert Time column type
            migrationBuilder.AlterColumn<TimeOnly>(
                name: "Time",
                table: "Events",
                type: "time(6)",
                nullable: false,
                oldClrType: typeof(TimeOnly),
                oldType: "time");
        }
    }
}
