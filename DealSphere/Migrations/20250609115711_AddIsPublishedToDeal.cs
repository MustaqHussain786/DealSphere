using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealSphere.Migrations
{
    /// <inheritdoc />
    public partial class AddIsPublishedToDeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPublished",
                table: "Deals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "Deals");
        }
    }
}
