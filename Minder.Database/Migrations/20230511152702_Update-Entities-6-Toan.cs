using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities6Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683818821934L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683304127818L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1683818821933L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683304127817L);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Group",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                columns: new[] { "DayOfBirth", "Password" },
                values: new object[] { 1683818821934L, "wYoNiOClwsx0kyK9EIRqmaEqg72hAj+U12g16P2YoVo5rYlhxSpU/sHoBXvgPICRHMM1ESCAs/tBHcxov6xjgQ==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Group");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683304127818L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683818821934L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1683304127817L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683818821933L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                columns: new[] { "DayOfBirth", "Password" },
                values: new object[] { -62135596800000L, "CcW16ZwR+2SFn8AnpaN+dNakxXvQTI3btbcwpiugge2xYM4H2NfaAD0ZAnOcC4k8HnQLQBGLCpgCtggVfyopgg==" });

            migrationBuilder.AddForeignKey(
                name: "FK_Group_Team_TeamId",
                table: "Group",
                column: "TeamId",
                principalTable: "Team",
                principalColumn: "Id");
        }
    }
}
