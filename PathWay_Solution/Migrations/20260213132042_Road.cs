using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class Road : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counters_Routes_RoutesRouteId",
                table: "Counters");

            migrationBuilder.DropIndex(
                name: "IX_Counters_RoutesRouteId",
                table: "Counters");

            migrationBuilder.DropColumn(
                name: "RoutesRouteId",
                table: "Counters");

            migrationBuilder.CreateIndex(
                name: "IX_Employee_AppUserId",
                table: "Employee",
                column: "AppUserId",
                unique: true,
                filter: "[AppUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Counters_RouteId",
                table: "Counters",
                column: "RouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counters_Routes_RouteId",
                table: "Counters",
                column: "RouteId",
                principalTable: "Routes",
                principalColumn: "RouteId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Employee_AspNetUsers_AppUserId",
                table: "Employee",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Counters_Routes_RouteId",
                table: "Counters");

            migrationBuilder.DropForeignKey(
                name: "FK_Employee_AspNetUsers_AppUserId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Employee_AppUserId",
                table: "Employee");

            migrationBuilder.DropIndex(
                name: "IX_Counters_RouteId",
                table: "Counters");

            migrationBuilder.AddColumn<int>(
                name: "RoutesRouteId",
                table: "Counters",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Counters_RoutesRouteId",
                table: "Counters",
                column: "RoutesRouteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Counters_Routes_RoutesRouteId",
                table: "Counters",
                column: "RoutesRouteId",
                principalTable: "Routes",
                principalColumn: "RouteId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
