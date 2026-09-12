using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.src.FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPetWalkerAvailabilitySettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DailyPetWalkLimit",
                table: "PetWalkers",
                type: "int",
                nullable: false,
                defaultValue: 8,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "InterBookingGapMinutes",
                table: "PetWalkers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SlotDurationMinutes",
                table: "PetWalkers",
                type: "int",
                nullable: false,
                defaultValue: 60);

            migrationBuilder.CreateTable(
                name: "PetWalkerDateExceptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    IsUnavailable = table.Column<bool>(type: "bit", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetWalkerDateExceptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetWalkerDateExceptions_PetWalkers_PetWalkerId",
                        column: x => x.PetWalkerId,
                        principalTable: "PetWalkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetWalkerDateExceptions_PetWalkerId_Date",
                table: "PetWalkerDateExceptions",
                columns: new[] { "PetWalkerId", "Date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PetWalkerDateExceptions");

            migrationBuilder.DropColumn(
                name: "InterBookingGapMinutes",
                table: "PetWalkers");

            migrationBuilder.DropColumn(
                name: "SlotDurationMinutes",
                table: "PetWalkers");

            migrationBuilder.AlterColumn<int>(
                name: "DailyPetWalkLimit",
                table: "PetWalkers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 8);
        }
    }
}
