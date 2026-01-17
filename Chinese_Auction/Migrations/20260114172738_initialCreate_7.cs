using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chinese_Auction.Migrations
{
    /// <inheritdoc />
    public partial class initialCreate_7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Basket");

            migrationBuilder.AddColumn<int>(
                name: "Value",
                table: "Gifts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "Gifts");

            migrationBuilder.CreateTable(
                name: "Basket",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Gift_Id = table.Column<int>(type: "int", nullable: false),
                    Package_Id = table.Column<int>(type: "int", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Cards_quantity = table.Column<int>(type: "int", nullable: false),
                    Unique_Package_Id = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Basket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Basket_Gifts_Gift_Id",
                        column: x => x.Gift_Id,
                        principalTable: "Gifts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Basket_Package_Package_Id",
                        column: x => x.Package_Id,
                        principalTable: "Package",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Basket_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Gift_Id",
                table: "Basket",
                column: "Gift_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_Package_Id",
                table: "Basket",
                column: "Package_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Basket_User_Id",
                table: "Basket",
                column: "User_Id");
        }
    }
}
