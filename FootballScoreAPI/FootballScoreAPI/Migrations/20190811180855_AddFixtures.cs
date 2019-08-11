using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FootballScoreAPI.Migrations
{
    public partial class AddFixtures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AwayTeam",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Competition",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "HomeTeam",
                table: "Goals");

            migrationBuilder.AddColumn<int>(
                name: "FixtureId",
                table: "Goals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Fixtures",
                columns: table => new
                {
                    FixtureId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Date = table.Column<DateTime>(nullable: false),
                    Competition = table.Column<string>(nullable: true),
                    HomeTeam = table.Column<string>(nullable: true),
                    AwayTeam = table.Column<string>(nullable: true),
                    HomeScore = table.Column<int>(nullable: false),
                    AwayScore = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fixtures", x => x.FixtureId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_FixtureId",
                table: "Goals",
                column: "FixtureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Fixtures_FixtureId",
                table: "Goals",
                column: "FixtureId",
                principalTable: "Fixtures",
                principalColumn: "FixtureId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Fixtures_FixtureId",
                table: "Goals");

            migrationBuilder.DropTable(
                name: "Fixtures");

            migrationBuilder.DropIndex(
                name: "IX_Goals_FixtureId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "FixtureId",
                table: "Goals");

            migrationBuilder.AddColumn<string>(
                name: "AwayTeam",
                table: "Goals",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Competition",
                table: "Goals",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "Goals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "HomeTeam",
                table: "Goals",
                nullable: true);
        }
    }
}
