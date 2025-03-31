using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EMS.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddRatingsReviewsToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "EventReviews");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "EventReviews");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "Events",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "ReviewsCount",
                table: "Events");

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "EventReviews",
                type: "double",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ReviewsCount",
                table: "EventReviews",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
