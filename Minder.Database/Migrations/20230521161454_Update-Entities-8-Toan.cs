using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities8Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684685693679L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684581458949L);

            migrationBuilder.AddColumn<long>(
                name: "Date",
                table: "MatchSetting",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1684685693679L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Date",
                table: "MatchSetting");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684581458949L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684685693679L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
