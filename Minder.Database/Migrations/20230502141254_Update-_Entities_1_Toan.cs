using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities1Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683036774060L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683034479101L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1683036774060L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683034479101L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683036774060L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
