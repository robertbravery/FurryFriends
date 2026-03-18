using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Timeslots : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClientAddress",
                table: "Bookings",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TimeslotId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CustomTimeRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestedDate = table.Column<DateOnly>(type: "date", nullable: false),
                    PreferredStartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    PreferredEndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    PreferredDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    ClientAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CounterOfferedDate = table.Column<DateOnly>(type: "date", nullable: true),
                    CounterOfferedTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    ResponseReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomTimeRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Timeslots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    DurationInMinutes = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeslots", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TravelBuffers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PreviousBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OriginAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DestinationAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    BufferDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TravelBuffers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WorkingHours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkingHours", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomTimeRequests_ClientId",
                table: "CustomTimeRequests",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomTimeRequests_PetWalkerId",
                table: "CustomTimeRequests",
                column: "PetWalkerId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomTimeRequests_RequestedDate",
                table: "CustomTimeRequests",
                column: "RequestedDate");

            migrationBuilder.CreateIndex(
                name: "IX_CustomTimeRequests_Status",
                table: "CustomTimeRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_Date",
                table: "Timeslots",
                column: "Date");

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_PetWalkerId",
                table: "Timeslots",
                column: "PetWalkerId");

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_PetWalkerId_Date",
                table: "Timeslots",
                columns: new[] { "PetWalkerId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_Status",
                table: "Timeslots",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TravelBuffers_BookingId",
                table: "TravelBuffers",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_TravelBuffers_PreviousBookingId",
                table: "TravelBuffers",
                column: "PreviousBookingId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_DayOfWeek",
                table: "WorkingHours",
                column: "DayOfWeek");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_PetWalkerId",
                table: "WorkingHours",
                column: "PetWalkerId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkingHours_PetWalkerId_DayOfWeek",
                table: "WorkingHours",
                columns: new[] { "PetWalkerId", "DayOfWeek" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CustomTimeRequests");

            migrationBuilder.DropTable(
                name: "Timeslots");

            migrationBuilder.DropTable(
                name: "TravelBuffers");

            migrationBuilder.DropTable(
                name: "WorkingHours");

            migrationBuilder.DropColumn(
                name: "ClientAddress",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "TimeslotId",
                table: "Bookings");
        }
    }
}
