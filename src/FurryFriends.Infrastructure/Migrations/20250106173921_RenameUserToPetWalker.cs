using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserToPetWalker : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_Users_UserID",
                table: "ServiceAreas");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.CreateTable(
                name: "PetWalkers",
                columns: table => new
                {
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Biography = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false),
                    HasFirstAidCertificatation = table.Column<bool>(type: "bit", nullable: false),
                    DailyPetWalkLimit = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StateProvinceRegion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetWalkers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PetWalkers_Email",
                table: "PetWalkers",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_PetWalkers_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "PetWalkers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_PetWalkers_UserID",
                table: "ServiceAreas",
                column: "UserID",
                principalTable: "PetWalkers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Photos_PetWalkers_UserId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_PetWalkers_UserID",
                table: "ServiceAreas");

            migrationBuilder.DropTable(
                name: "PetWalkers");

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Biography = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DailyPetWalkLimit = table.Column<int>(type: "int", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HasFirstAidCertificatation = table.Column<bool>(type: "bit", nullable: false),
                    HasInsurance = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: false),
                    City = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    StateProvinceRegion = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Street = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ZipCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Currency = table.Column<string>(type: "char(3)", fixedLength: true, maxLength: 3, nullable: false),
                    HourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    Gender = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    PhoneCountryCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_Users_UserID",
                table: "ServiceAreas",
                column: "UserID",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
