using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pexita.Migrations
{
    /// <inheritdoc />
    public partial class userprpofilepic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Successfull",
                table: "Payments",
                newName: "Successful");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicURL",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicURL",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Successful",
                table: "Payments",
                newName: "Successfull");
        }
    }
}
