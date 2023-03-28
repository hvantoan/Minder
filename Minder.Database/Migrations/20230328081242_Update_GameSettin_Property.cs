using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateGameSettinProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSetting_User_UserId",
                table: "GameSetting");

            migrationBuilder.DropForeignKey(
                name: "FK_Team_GameSetting_GameSettingId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_Team_GameSettingId",
                table: "Team");

            migrationBuilder.DropIndex(
                name: "IX_GameSetting_UserId",
                table: "GameSetting");

            migrationBuilder.DropColumn(
                name: "GameSettingId",
                table: "Team");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "GameSetting",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "GameSetting",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameSetting_TeamId",
                table: "GameSetting",
                column: "TeamId",
                unique: true,
                filter: "[TeamId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_GameSetting_UserId",
                table: "GameSetting",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSetting_Team_TeamId",
                table: "GameSetting",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameSetting_User_UserId",
                table: "GameSetting",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameSetting_Team_TeamId",
                table: "GameSetting");

            migrationBuilder.DropForeignKey(
                name: "FK_GameSetting_User_UserId",
                table: "GameSetting");

            migrationBuilder.DropIndex(
                name: "IX_GameSetting_TeamId",
                table: "GameSetting");

            migrationBuilder.DropIndex(
                name: "IX_GameSetting_UserId",
                table: "GameSetting");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "GameSetting");

            migrationBuilder.AddColumn<string>(
                name: "GameSettingId",
                table: "Team",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "GameSetting",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Team_GameSettingId",
                table: "Team",
                column: "GameSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSetting_UserId",
                table: "GameSetting",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_GameSetting_User_UserId",
                table: "GameSetting",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Team_GameSetting_GameSettingId",
                table: "Team",
                column: "GameSettingId",
                principalTable: "GameSetting",
                principalColumn: "Id");
        }
    }
}
