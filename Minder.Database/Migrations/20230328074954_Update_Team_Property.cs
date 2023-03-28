using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameTimes",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "GameTypes",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Point",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Radius",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Team");

            migrationBuilder.AddColumn<string>(
                name: "GameSettingId",
                table: "Team",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_GameSettingId",
                table: "Team",
                column: "GameSettingId");

            migrationBuilder.AddForeignKey(
                name: "FK_Team_GameSetting_GameSettingId",
                table: "Team",
                column: "GameSettingId",
                principalTable: "GameSetting",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Team_GameSetting_GameSettingId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_GameSettingId",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "GameSettingId",
                table: "Team");

            migrationBuilder.AddColumn<string>(
                name: "GameTimes",
                table: "Team",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameTypes",
                table: "Team",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "Team",
                type: "decimal(18,15)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "Team",
                type: "decimal(18,15)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Point",
                table: "Team",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Radius",
                table: "Team",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Team",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
