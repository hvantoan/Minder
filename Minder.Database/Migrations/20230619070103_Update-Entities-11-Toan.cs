using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities11Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipant_MatchSetting_MatchSettingId",
                table: "MatchParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipant_Member_MemberId",
                table: "MatchParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchParticipant",
                table: "MatchParticipant");

            migrationBuilder.RenameColumn(
                name: "MatchSettingId",
                table: "MatchParticipant",
                newName: "MatchId");

            migrationBuilder.RenameColumn(
                name: "MemberId",
                table: "MatchParticipant",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_MatchParticipant_MatchSettingId",
                table: "MatchParticipant",
                newName: "IX_MatchParticipant_MatchId");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1687158063204L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1687157253098L);

            migrationBuilder.AddColumn<bool>(
                name: "IsAccept",
                table: "MatchParticipant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "MatchParticipant",
                type: "nvarchar(32)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchParticipant",
                table: "MatchParticipant",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1687158063204L);

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipant_UserId",
                table: "MatchParticipant",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipant_Match_MatchId",
                table: "MatchParticipant",
                column: "MatchId",
                principalTable: "Match",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipant_User_UserId",
                table: "MatchParticipant",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipant_Match_MatchId",
                table: "MatchParticipant");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchParticipant_User_UserId",
                table: "MatchParticipant");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MatchParticipant",
                table: "MatchParticipant");

            migrationBuilder.DropIndex(
                name: "IX_MatchParticipant_UserId",
                table: "MatchParticipant");

            migrationBuilder.DropColumn(
                name: "IsAccept",
                table: "MatchParticipant");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "MatchParticipant");

            migrationBuilder.RenameColumn(
                name: "MatchId",
                table: "MatchParticipant",
                newName: "MatchSettingId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "MatchParticipant",
                newName: "MemberId");

            migrationBuilder.RenameIndex(
                name: "IX_MatchParticipant_MatchId",
                table: "MatchParticipant",
                newName: "IX_MatchParticipant_MatchSettingId");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1687157253098L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1687158063204L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatchParticipant",
                table: "MatchParticipant",
                columns: new[] { "MemberId", "MatchSettingId" });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipant_MatchSetting_MatchSettingId",
                table: "MatchParticipant",
                column: "MatchSettingId",
                principalTable: "MatchSetting",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchParticipant_Member_MemberId",
                table: "MatchParticipant",
                column: "MemberId",
                principalTable: "Member",
                principalColumn: "Id");
        }
    }
}
