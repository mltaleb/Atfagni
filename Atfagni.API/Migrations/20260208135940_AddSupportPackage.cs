using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Atfagni.API.Migrations
{
    /// <inheritdoc />
    public partial class AddSupportPackage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptsPackages",
                table: "Rides",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "AcceptsPassengers",
                table: "Rides",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PackageDescription",
                table: "Rides",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerPackage",
                table: "Rides",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "AgreedPrice",
                table: "Bookings",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "PackageDetails",
                table: "Bookings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptsPackages",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "AcceptsPassengers",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PackageDescription",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "PricePerPackage",
                table: "Rides");

            migrationBuilder.DropColumn(
                name: "AgreedPrice",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PackageDetails",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Bookings");
        }
    }
}
