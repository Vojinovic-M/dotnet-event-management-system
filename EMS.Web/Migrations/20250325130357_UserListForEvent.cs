using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class UserListForEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersInEvent",
                table: "Events",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsersInEvent",
                table: "Events");
        }
    }
}
