using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FurryFriends.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveServiceAreaRelationshioptoCountryRegion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_Countries_CountryID",
                table: "ServiceAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_Localities_LocalityID",
                table: "ServiceAreas");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_Regions_RegionID",
                table: "ServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_CountryID",
                table: "ServiceAreas");

            migrationBuilder.DropIndex(
                name: "IX_ServiceAreas_RegionID",
                table: "ServiceAreas");

            migrationBuilder.DropColumn(
                name: "CountryID",
                table: "ServiceAreas");

            migrationBuilder.DropColumn(
                name: "RegionID",
                table: "ServiceAreas");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_Localities_LocalityID",
                table: "ServiceAreas",
                column: "LocalityID",
                principalTable: "Localities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServiceAreas_Localities_LocalityID",
                table: "ServiceAreas");

            migrationBuilder.AddColumn<Guid>(
                name: "CountryID",
                table: "ServiceAreas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "RegionID",
                table: "ServiceAreas",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_CountryID",
                table: "ServiceAreas",
                column: "CountryID");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAreas_RegionID",
                table: "ServiceAreas",
                column: "RegionID");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_Countries_CountryID",
                table: "ServiceAreas",
                column: "CountryID",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_Localities_LocalityID",
                table: "ServiceAreas",
                column: "LocalityID",
                principalTable: "Localities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceAreas_Regions_RegionID",
                table: "ServiceAreas",
                column: "RegionID",
                principalTable: "Regions",
                principalColumn: "Id");
        }
    }
}
