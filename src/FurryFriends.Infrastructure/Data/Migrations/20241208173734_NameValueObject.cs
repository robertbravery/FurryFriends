using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Data.Migrations;

    /// <inheritdoc />
    public partial class NameValueObject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Contributors");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Contributors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Contributors",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name_FullName",
                table: "Contributors",
                type: "TEXT",
                nullable: false,
                computedColumnSql: "FirstName + ' ' + LastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name_FullName",
                table: "Contributors");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Contributors");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Contributors");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Contributors",
                type: "TEXT",
                maxLength: 100,
                nullable: false,
                defaultValue: "");
        }
    }

