using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealSphere.Migrations
{
    /// <inheritdoc />
    public partial class AddActualPriceToDeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Deals");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "Deals",
                newName: "ActualPrice");

            migrationBuilder.AlterColumn<float>(
                name: "Rating",
                table: "Deals",
                type: "real",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActualPrice",
                table: "Deals",
                newName: "Price");

            migrationBuilder.AlterColumn<double>(
                name: "Rating",
                table: "Deals",
                type: "float",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
