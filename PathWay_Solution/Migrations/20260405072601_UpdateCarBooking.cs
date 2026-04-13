using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCarBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Routes_FromLocationId",
                table: "Routes");

            migrationBuilder.AddColumn<DateTime>(
                name: "DropTime",
                table: "VehicleBookings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_VehicleBookings_AppUserId",
                table: "VehicleBookings",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromLocationId_ToLocationId",
                table: "Routes",
                columns: new[] { "FromLocationId", "ToLocationId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_VehicleBookings_AspNetUsers_AppUserId",
                table: "VehicleBookings",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VehicleBookings_AspNetUsers_AppUserId",
                table: "VehicleBookings");

            migrationBuilder.DropIndex(
                name: "IX_VehicleBookings_AppUserId",
                table: "VehicleBookings");

            migrationBuilder.DropIndex(
                name: "IX_Routes_FromLocationId_ToLocationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "DropTime",
                table: "VehicleBookings");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_FromLocationId",
                table: "Routes",
                column: "FromLocationId");
        }
    }
}
