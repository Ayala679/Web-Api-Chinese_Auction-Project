using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinese_Auction.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate_11 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "Gifts");

            migrationBuilder.RenameColumn(
                name: "isLottery",
                table: "Gifts",
                newName: "IsLottery");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLottery",
                table: "Gifts",
                newName: "isLottery");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "Gifts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
