using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTourAttributesFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TourAttributes_Tours_TourId",
                table: "TourAttributes");

            migrationBuilder.DropIndex(
                name: "IX_TourAttributes_TourId",
                table: "TourAttributes");

            migrationBuilder.DropColumn(
                name: "TourId",
                table: "TourAttributes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "TourId",
                table: "TourAttributes",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_TourAttributes_TourId",
                table: "TourAttributes",
                column: "TourId");

            migrationBuilder.AddForeignKey(
                name: "FK_TourAttributes_Tours_TourId",
                table: "TourAttributes",
                column: "TourId",
                principalTable: "Tours",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
