using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSpeciesStringFieldFromPets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Clients_OwnerId",
                table: "Pets");

            migrationBuilder.DropColumn(
                name: "Species",
                table: "Pets");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Pets",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Clients_OwnerId",
                table: "Pets",
                column: "OwnerId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pets_Clients_OwnerId",
                table: "Pets");

            migrationBuilder.AlterColumn<string>(
                name: "Color",
                table: "Pets",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "Species",
                table: "Pets",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Pets_Clients_OwnerId",
                table: "Pets",
                column: "OwnerId",
                principalTable: "Clients",
                principalColumn: "Id");
        }
    }
}
