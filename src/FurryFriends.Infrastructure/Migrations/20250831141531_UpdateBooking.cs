using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations;

/// <inheritdoc />
public partial class UpdateBooking : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.RenameColumn(
        name: "Start",
        table: "Bookings",
        newName: "StartTime");

    migrationBuilder.RenameColumn(
        name: "End",
        table: "Bookings",
        newName: "EndTime");

    migrationBuilder.AddColumn<string>(
        name: "Notes",
        table: "Bookings",
        type: "nvarchar(max)",
        nullable: true);

    migrationBuilder.AddColumn<decimal>(
        name: "Price",
        table: "Bookings",
        type: "decimal(18,2)",
        nullable: false,
        defaultValue: 0.0m);

    migrationBuilder.AddColumn<int>(
        name: "Status",
        table: "Bookings",
        type: "int",
        nullable: false,
        defaultValue: 0);

    migrationBuilder.AddColumn<DateTime>(
        name: "CreatedAt",
        table: "Bookings",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

    migrationBuilder.AddColumn<DateTime>(
        name: "UpdatedAt",
        table: "Bookings",
        type: "datetime2",
        nullable: false,
        defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropColumn(
        name: "CreatedAt",
        table: "Bookings");

    migrationBuilder.DropColumn(
        name: "UpdatedAt",
        table: "Bookings");

    migrationBuilder.DropColumn(
        name: "Notes",
        table: "Bookings");

    migrationBuilder.DropColumn(
        name: "Price",
        table: "Bookings");

    migrationBuilder.DropColumn(
        name: "Status",
        table: "Bookings");

    migrationBuilder.RenameColumn(
        name: "StartTime",
        table: "Bookings",
        newName: "Start");

    migrationBuilder.RenameColumn(
        name: "EndTime",
        table: "Bookings",
        newName: "End");
  }
}
