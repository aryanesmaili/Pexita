using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pexita.Migrations
{
    /// <inheritdoc />
    public partial class InitialMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Price = table.Column<double>(type: "float", nullable: true),
                    BrandName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsPurchasedBefore = table.Column<bool>(type: "bit", nullable: false),
                    DatePurchased = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Rate = table.Column<double>(type: "float", nullable: true),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductPicURL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateAdded = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
