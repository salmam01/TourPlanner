using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TourPlanner.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tours",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    TransportType = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tours", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TourAttributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Popularity = table.Column<double>(type: "double precision", nullable: false),
                    ChildFriendliness = table.Column<bool>(type: "boolean", nullable: false),
                    SearchAlgorithmRanking = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourAttributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourAttributes_Tours_Id",
                        column: x => x.Id,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourDetails",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RouteInformation = table.Column<string>(type: "text", nullable: true),
                    Distance = table.Column<double>(type: "double precision", nullable: false),
                    EstimatedTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourDetails_Tours_Id",
                        column: x => x.Id,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TourLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Difficulty = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<double>(type: "double precision", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    TotalDistance = table.Column<double>(type: "double precision", nullable: false),
                    TotalTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    TourId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TourLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TourLogs_Tours_TourId",
                        column: x => x.TourId,
                        principalTable: "Tours",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TourLogs_TourId",
                table: "TourLogs",
                column: "TourId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TourAttributes");

            migrationBuilder.DropTable(
                name: "TourDetails");

            migrationBuilder.DropTable(
                name: "TourLogs");

            migrationBuilder.DropTable(
                name: "Tours");
        }
    }
}
