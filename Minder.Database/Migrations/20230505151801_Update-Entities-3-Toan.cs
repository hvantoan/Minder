using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities3Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683299881316L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683295913748L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1683299881315L,
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

            migrationBuilder.AddColumn<string>(
                name: "TeamId",
                table: "Group",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1683299881316L);

            migrationBuilder.CreateIndex(
                name: "IX_Group_TeamId",
                table: "Group",
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

            migrationBuilder.DropIndex(
                name: "IX_Group_TeamId",
                table: "Group");

            migrationBuilder.DropColumn(
                name: "TeamId",
                table: "Group");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683295913748L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683299881316L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683299881315L);

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

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
