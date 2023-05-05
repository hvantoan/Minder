using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddGamtimeDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GameTime",
                table: "GameSetting");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683034479101L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1682701007170L);

            migrationBuilder.CreateTable(
                name: "GameTime",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Monday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tuesday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wednesday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Thursday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Friday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Saturday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sunday = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameSettingId = table.Column<string>(type: "nvarchar(32)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTime", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameTime_GameSetting_GameSettingId",
                        column: x => x.GameSettingId,
                        principalTable: "GameSetting",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1683034479101L);

            migrationBuilder.CreateIndex(
                name: "IX_GameTime_GameSettingId",
                table: "GameTime",
                column: "GameSettingId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameTime");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1682701007170L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683034479101L);

            migrationBuilder.AddColumn<string>(
                name: "GameTime",
                table: "GameSetting",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
