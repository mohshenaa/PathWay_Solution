using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class Chair : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripSchedule_Routes_RoutesRouteId",
                table: "TripSchedule");

            migrationBuilder.DropIndex(
                name: "IX_TripSchedule_RoutesRouteId",
                table: "TripSchedule");

            migrationBuilder.DropColumn(
                name: "RoutesRouteId",
                table: "TripSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_TripSchedule_RouteId",
                table: "TripSchedule",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripSchedule_Routes_RouteId",
                table: "TripSchedule",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "RouteId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TripSchedule_Routes_RouteId",
                table: "TripSchedule");

            migrationBuilder.DropIndex(
                name: "IX_TripSchedule_RouteId",
                table: "TripSchedule");

            migrationBuilder.AddColumn<int>(
                name: "RoutesRouteId",
                table: "TripSchedule",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TripSchedule_RoutesRouteId",
                table: "TripSchedule",
                column: "RoutesRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripSchedule_Routes_RoutesRouteId",
                table: "TripSchedule",
                column: "RoutesRouteId",
                principalTable: "Routes",
                principalColumn: "RouteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
