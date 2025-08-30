using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class BookingSchedularInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PetOwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Clients_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Bookings_PetWalkers_PetWalkerId",
                        column: x => x.PetWalkerId,
                        principalTable: "PetWalkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PetWalkerSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    PetWalkerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetWalkerSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PetWalkerSchedules_PetWalkers_PetWalkerId",
                        column: x => x.PetWalkerId,
                        principalTable: "PetWalkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PetOwnerId",
                table: "Bookings",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_PetWalkerId",
                table: "Bookings",
                column: "PetWalkerId");

            migrationBuilder.CreateIndex(
                name: "IX_PetWalkerSchedules_PetWalkerId",
                table: "PetWalkerSchedules",
                column: "PetWalkerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "PetWalkerSchedules");
        }
    }
}
