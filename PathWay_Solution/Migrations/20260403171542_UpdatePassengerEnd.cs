using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePassengerEnd : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "TripSeat",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PriceAtBooking",
                table: "BookingSeat",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "TripSeat");

            migrationBuilder.DropColumn(
                name: "PriceAtBooking",
                table: "BookingSeat");
        }
    }
}
