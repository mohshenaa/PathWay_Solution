using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PathWay_Solution.Migrations
{
    /// <inheritdoc />
    public partial class Town : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CounterStaff_Counters_CounterId",
                table: "CounterStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_CounterStaff_Employee_EmployeeId",
                table: "CounterStaff");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Location_LocationId",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_Routes_Location_LocationId1",
                table: "Routes");

            migrationBuilder.DropForeignKey(
                name: "FK_TripStop_Location_LocationId",
                table: "TripStop");

            migrationBuilder.DropForeignKey(
                name: "FK_TripStop_Location_LocationId1",
                table: "TripStop");

            migrationBuilder.DropIndex(
                name: "IX_TripStop_LocationId1",
                table: "TripStop");

            migrationBuilder.DropIndex(
                name: "IX_Routes_LocationId",
                table: "Routes");

            migrationBuilder.DropIndex(
                name: "IX_Routes_LocationId1",
                table: "Routes");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CounterStaff",
                table: "CounterStaff");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "TripStop");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "Routes");

            migrationBuilder.DropColumn(
                name: "LocationId1",
                table: "Routes");

            migrationBuilder.RenameTable(
                name: "CounterStaff",
                newName: "AdCounterStaffdress");

            migrationBuilder.RenameIndex(
                name: "IX_CounterStaff_EmployeeId",
                table: "AdCounterStaffdress",
                newName: "IX_AdCounterStaffdress_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_CounterStaff_CounterId",
                table: "AdCounterStaffdress",
                newName: "IX_AdCounterStaffdress_CounterId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AdCounterStaffdress",
                table: "AdCounterStaffdress",
                column: "CounterStaffId");

            migrationBuilder.CreateTable(
                name: "Expense",
                columns: table => new
                {
                    ExpenseId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExpenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: true),
                    TripId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expense", x => x.ExpenseId);
                    table.ForeignKey(
                        name: "FK_Expense_Trip_TripId",
                        column: x => x.TripId,
                        principalTable: "Trip",
                        principalColumn: "TripId");
                    table.ForeignKey(
                        name: "FK_Expense_Vehicle_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicle",
                        principalColumn: "VehicleId");
                });

            migrationBuilder.CreateTable(
                name: "ReportAnalytics",
                columns: table => new
                {
                    ReportAnalyticsId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReportType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportAnalytics", x => x.ReportAnalyticsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expense_TripId",
                table: "Expense",
                column: "TripId");

            migrationBuilder.CreateIndex(
                name: "IX_Expense_VehicleId",
                table: "Expense",
                column: "VehicleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdCounterStaffdress_Counters_CounterId",
                table: "AdCounterStaffdress",
                column: "CounterId",
                principalTable: "Counters",
                principalColumn: "CounterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AdCounterStaffdress_Employee_EmployeeId",
                table: "AdCounterStaffdress",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TripStop_Location_LocationId",
                table: "TripStop",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdCounterStaffdress_Counters_CounterId",
                table: "AdCounterStaffdress");

            migrationBuilder.DropForeignKey(
                name: "FK_AdCounterStaffdress_Employee_EmployeeId",
                table: "AdCounterStaffdress");

            migrationBuilder.DropForeignKey(
                name: "FK_TripStop_Location_LocationId",
                table: "TripStop");

            migrationBuilder.DropTable(
                name: "Expense");

            migrationBuilder.DropTable(
                name: "ReportAnalytics");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AdCounterStaffdress",
                table: "AdCounterStaffdress");

            migrationBuilder.RenameTable(
                name: "AdCounterStaffdress",
                newName: "CounterStaff");

            migrationBuilder.RenameIndex(
                name: "IX_AdCounterStaffdress_EmployeeId",
                table: "CounterStaff",
                newName: "IX_CounterStaff_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_AdCounterStaffdress_CounterId",
                table: "CounterStaff",
                newName: "IX_CounterStaff_CounterId");

            migrationBuilder.AddColumn<int>(
                name: "LocationId1",
                table: "TripStop",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "Routes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LocationId1",
                table: "Routes",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CounterStaff",
                table: "CounterStaff",
                column: "CounterStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TripStop_LocationId1",
                table: "TripStop",
                column: "LocationId1");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LocationId",
                table: "Routes",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Routes_LocationId1",
                table: "Routes",
                column: "LocationId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CounterStaff_Counters_CounterId",
                table: "CounterStaff",
                column: "CounterId",
                principalTable: "Counters",
                principalColumn: "CounterId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CounterStaff_Employee_EmployeeId",
                table: "CounterStaff",
                column: "EmployeeId",
                principalTable: "Employee",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Location_LocationId",
                table: "Routes",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routes_Location_LocationId1",
                table: "Routes",
                column: "LocationId1",
                principalTable: "Location",
                principalColumn: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripStop_Location_LocationId",
                table: "TripStop",
                column: "LocationId",
                principalTable: "Location",
                principalColumn: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_TripStop_Location_LocationId1",
                table: "TripStop",
                column: "LocationId1",
                principalTable: "Location",
                principalColumn: "LocationId");
        }
    }
}
