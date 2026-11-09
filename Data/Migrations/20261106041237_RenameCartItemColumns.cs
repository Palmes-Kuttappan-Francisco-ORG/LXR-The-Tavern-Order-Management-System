using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LXR_The_Tavern_Order_Management_System.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameCartItemColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "CartItems",
                newName: "MenuItemName");

            migrationBuilder.RenameColumn(
                name: "ProductID",
                table: "CartItems",
                newName: "MenuItemID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MenuItemName",
                table: "CartItems",
                newName: "ProductName");

            migrationBuilder.RenameColumn(
                name: "MenuItemID",
                table: "CartItems",
                newName: "ProductID");

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Category = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ID);
                });
        }
    }
}
