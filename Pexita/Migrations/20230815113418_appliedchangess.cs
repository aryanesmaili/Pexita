using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pexita.Migrations
{
    /// <inheritdoc />
    public partial class appliedchangess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Address_Users_UserID",
                table: "Address");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Address",
                table: "Address");

            migrationBuilder.RenameTable(
                name: "Address",
                newName: "Addresses");

            migrationBuilder.RenameColumn(
                name: "DateAdded",
                table: "Products",
                newName: "DateCreated");

            migrationBuilder.RenameColumn(
                name: "TimeCreated",
                table: "Comments",
                newName: "DateCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Address_UserID",
                table: "Addresses",
                newName: "IX_Addresses_UserID");

            migrationBuilder.AddColumn<string>(
                name: "PhoneNumber",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_Users_UserID",
                table: "Addresses",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_Users_UserID",
                table: "Addresses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Addresses",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "Addresses");

            migrationBuilder.RenameTable(
                name: "Addresses",
                newName: "Address");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Products",
                newName: "DateAdded");

            migrationBuilder.RenameColumn(
                name: "DateCreated",
                table: "Comments",
                newName: "TimeCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Addresses_UserID",
                table: "Address",
                newName: "IX_Address_UserID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Address",
                table: "Address",
                column: "ID");

            migrationBuilder.AddForeignKey(
                name: "FK_Address_Users_UserID",
                table: "Address",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "ID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
