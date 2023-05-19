using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities9Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867570L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "TeamRejected",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867570L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867569L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867569L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

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

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867569L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867569L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867568L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867567L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867567L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867562L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066085L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684496867562L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066084L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1684496867570L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAutoLocation",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "IsAutoTime",
                table: "Team");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867570L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "TeamRejected",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867570L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867569L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867569L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867569L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867569L);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867568L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867567L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867567L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066085L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867562L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066084L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684496867562L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
