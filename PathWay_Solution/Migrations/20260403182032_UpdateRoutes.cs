using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoutes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PricePerSeat",
                table: "Routes",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PricePerSeat",
                table: "Routes");
        }
    }
}
