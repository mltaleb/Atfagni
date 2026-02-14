using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atfagni.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFirstNameAndLastNameToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CarLicensePlate",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarModel",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DefaultSeats",
                table: "Users",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CarLicensePlate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CarModel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DefaultSeats",
                table: "Users");
        }
    }
}
