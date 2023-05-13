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
                defaultValue: 1684002834500L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683818821934L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684002834500L,
                oldClrType: typeof(long),
                oldType: "bigint");

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
                defaultValue: 1684002834500L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684002834499L,
                oldClrType: typeof(long),
                oldType: "bigint");

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
                defaultValue: 1684002834499L);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                defaultValue: 1684002834499L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Participant",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684002834499L,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684002834499L);

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
                defaultValue: 1684002834497L,
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
                defaultValue: 1684002834497L);

            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SelectedDate = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HostTeam",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    StadiumId = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostTeam_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HostTeam_Stadium_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadium",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HostTeam_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpposingTeam",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    MatchId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    StadiumId = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpposingTeam", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpposingTeam_Match_MatchId",
                        column: x => x.MatchId,
                        principalTable: "Match",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OpposingTeam_Stadium_StadiumId",
                        column: x => x.StadiumId,
                        principalTable: "Stadium",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpposingTeam_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HostParticipant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    HostTeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HostParticipant_HostTeam_HostTeamId",
                        column: x => x.HostTeamId,
                        principalTable: "HostTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HostParticipant_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "OpposingParticipant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    OpposingTeamId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    MemberId = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpposingParticipant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OpposingParticipant_Member_MemberId",
                        column: x => x.MemberId,
                        principalTable: "Member",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OpposingParticipant_OpposingTeam_OpposingTeamId",
                        column: x => x.OpposingTeamId,
                        principalTable: "OpposingTeam",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1684002834500L);

            migrationBuilder.CreateIndex(
                name: "IX_Group_ChannelId_Type",
                table: "Group",
                columns: new[] { "ChannelId", "Type" });

            migrationBuilder.CreateIndex(
                name: "IX_Group_Id",
                table: "Group",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HostParticipant_HostTeamId",
                table: "HostParticipant",
                column: "HostTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_HostParticipant_MemberId",
                table: "HostParticipant",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_HostTeam_MatchId",
                table: "HostTeam",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostTeam_StadiumId",
                table: "HostTeam",
                column: "StadiumId");

            migrationBuilder.CreateIndex(
                name: "IX_HostTeam_TeamId",
                table: "HostTeam",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OpposingParticipant_MemberId",
                table: "OpposingParticipant",
                column: "MemberId");

            migrationBuilder.CreateIndex(
                name: "IX_OpposingParticipant_OpposingTeamId",
                table: "OpposingParticipant",
                column: "OpposingTeamId");

            migrationBuilder.CreateIndex(
                name: "IX_OpposingTeam_MatchId",
                table: "OpposingTeam",
                column: "MatchId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OpposingTeam_StadiumId",
                table: "OpposingTeam",
                column: "StadiumId");

            migrationBuilder.CreateIndex(
                name: "IX_OpposingTeam_TeamId",
                table: "OpposingTeam",
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
                name: "HostParticipant");

            migrationBuilder.DropTable(
                name: "OpposingParticipant");

            migrationBuilder.DropTable(
                name: "HostTeam");

            migrationBuilder.DropTable(
                name: "OpposingTeam");

            migrationBuilder.DropTable(
                name: "Match");

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
                oldDefaultValue: 1684002834500L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684002834500L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684002834499L);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelete",
                table: "Stadium",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684002834499L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684002834499L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1683818821933L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684002834497L);

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
