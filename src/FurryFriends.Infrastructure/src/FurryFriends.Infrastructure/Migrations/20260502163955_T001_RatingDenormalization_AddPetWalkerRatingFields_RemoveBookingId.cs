using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.src.FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class T001_RatingDenormalization_AddPetWalkerRatingFields_RemoveBookingId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_BookingId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_CreatedDate",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "BookingId",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                table: "Ratings");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Ratings",
                newName: "UpdatedAt");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Ratings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Ratings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "AverageRating",
                table: "PetWalkers",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TotalRatingsCount",
                table: "PetWalkers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ClientId",
                table: "Ratings",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_PetWalkerId_Status",
                table: "Ratings",
                columns: new[] { "PetWalkerId", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Ratings_ClientId",
                table: "Ratings");

            migrationBuilder.DropIndex(
                name: "IX_Ratings_PetWalkerId_Status",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "AverageRating",
                table: "PetWalkers");

            migrationBuilder.DropColumn(
                name: "TotalRatingsCount",
                table: "PetWalkers");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Ratings",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<Guid>(
                name: "BookingId",
                table: "Ratings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                table: "Ratings",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_BookingId",
                table: "Ratings",
                column: "BookingId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_CreatedDate",
                table: "Ratings",
                column: "CreatedDate");
        }
    }
}
