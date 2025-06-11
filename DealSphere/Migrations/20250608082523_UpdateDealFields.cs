using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DealSphere.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDealFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountPrice",
                table: "Deals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OtherStoreName",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "OtherStorePrice",
                table: "Deals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "OtherStoreProductUrl",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductUrl",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Rating",
                table: "Deals",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Store",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Subcategory",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "DiscountPrice",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OtherStoreName",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OtherStorePrice",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "OtherStoreProductUrl",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "ProductUrl",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Rating",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Store",
                table: "Deals");

            migrationBuilder.DropColumn(
                name: "Subcategory",
                table: "Deals");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Deals",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
