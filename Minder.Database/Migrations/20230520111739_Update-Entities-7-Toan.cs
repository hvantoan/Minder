using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities7Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsDelete",
                table: "Stadium");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684581458949L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683818821934L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoLocation",
                table: "Team",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAutoTime",
                table: "Team",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Team",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Stadium",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Participant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Member",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683818821933L);

            migrationBuilder.AlterColumn<string>(
                name: "ChannelId",
                table: "Group",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Group",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UpdateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "MatchSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    StadiumId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    SelectedDayOfWeek = table.Column<int>(type: "int", nullable: true),
                    From = table.Column<int>(type: "int", nullable: true),
                    To = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchSetting_Stadium_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadium",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchSetting_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeamRejected",
                columns: table => new
                {
                    TeamId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamRejected", x => new { x.TeamId, x.ItemId });
                    table.ForeignKey(
                        name: "FK_TeamRejected_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    HostTeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    OppsingTeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SelectedDayOfWeek = table.Column<int>(type: "int", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    From = table.Column<int>(type: "int", nullable: true),
                    To = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Match_MatchSetting_HostTeamId",
                        column: x => x.HostTeamId,
                        principalTable: "MatchSetting",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Match_MatchSetting_OppsingTeamId",
                        column: x => x.OppsingTeamId,
                        principalTable: "MatchSetting",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MatchParticipant",
                columns: table => new
                {
                    MemberId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    MatchSettingId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchParticipant", x => new { x.MemberId, x.MatchSettingId });
                    table.ForeignKey(
                        name: "FK_MatchParticipant_MatchSetting_MatchSettingId",
                        column: x => x.MatchSettingId,
                        principalTable: "MatchSetting",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MatchParticipant_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1684581458949L);

            migrationBuilder.CreateIndex(
                name: "IX_Group_ChannelId_Type",
                table: "Group",
                columns: new[] { "ChannelId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Group_Id",
                table: "Group",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Match_HostTeamId",
                table: "Match",
                column: "HostTeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Match_OppsingTeamId",
                table: "Match",
                column: "OppsingTeamId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MatchParticipant_MatchSettingId",
                table: "MatchParticipant",
                column: "MatchSettingId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSetting_StadiumId",
                table: "MatchSetting",
                column: "StadiumId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchSetting_TeamId",
                table: "MatchSetting",
                column: "TeamId");

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group");

            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "MatchParticipant");

            migrationBuilder.DropTable(
                name: "TeamRejected");

            migrationBuilder.DropTable(
                name: "MatchSetting");

            migrationBuilder.DropIndex(
                name: "IX_Group_ChannelId_Type",
                table: "Group");

            migrationBuilder.DropIndex(
                name: "IX_Group_Id",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsAutoLocation",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IsAutoTime",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Stadium");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Stadium");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Participant");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Member");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "UpdateAt",
                table: "Group");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683818821934L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684581458949L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Stadium",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1683818821933L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "ChannelId",
                table: "Group",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                columns: new[] { "DayOfBirth", "IsDelete" },
                values: new object[] { -62135596800000L, false });

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
