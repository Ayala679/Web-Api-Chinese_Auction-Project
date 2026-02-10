using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinese_Auction.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate_10 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "picture",
                table: "Gifts",
                newName: "Picture");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Picture",
                table: "Gifts",
                newName: "picture");
        }
    }
}
