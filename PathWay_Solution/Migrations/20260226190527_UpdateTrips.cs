using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTrips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Trip");

            migrationBuilder.AlterColumn<int>(
                name: "TripType",
                table: "Trip",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "TripStatus",
                table: "Trip",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TripStatus",
                table: "Trip");

            migrationBuilder.AlterColumn<string>(
                name: "TripType",
                table: "Trip",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Trip",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
