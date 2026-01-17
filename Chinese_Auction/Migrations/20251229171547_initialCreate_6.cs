using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinese_Auction.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate_6 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Unique_Package_Id",
                table: "Package");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Unique_Package_Id",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
