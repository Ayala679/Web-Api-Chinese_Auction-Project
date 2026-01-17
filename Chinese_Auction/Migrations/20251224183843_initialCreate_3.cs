using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinese_Auction.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate_3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Winner");

            migrationBuilder.RenameColumn(
                name: "UniquePackageId",
                table: "Basket",
                newName: "Unique_Package_Id");

            migrationBuilder.AddColumn<bool>(
                name: "Is_Won",
                table: "Purchases",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Is_Won",
                table: "Purchases");

            migrationBuilder.RenameColumn(
                name: "Unique_Package_Id",
                table: "Basket",
                newName: "UniquePackageId");

            migrationBuilder.CreateTable(
                name: "Winner",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gift_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Winner", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Winner_Gifts_Gift_Id",
                        column: x => x.Gift_Id,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Winner_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Winner_Gift_Id",
                table: "Winner",
                column: "Gift_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Winner_User_Id",
                table: "Winner",
                column: "User_Id");
        }
    }
}
