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
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024720L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "TeamRejected",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066088L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024719L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024718L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066087L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024718L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066085L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024716L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684290066084L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684118024716L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1684290066088L);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024720L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "TeamRejected",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Team",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Stadium",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066088L);

            migrationBuilder.AlterColumn<long>(
                name: "JoinAt",
                table: "Participant",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024719L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024718L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Message",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024718L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066087L);

            migrationBuilder.AlterColumn<long>(
                name: "UpdateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024716L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066085L);

            migrationBuilder.AlterColumn<long>(
                name: "CreateAt",
                table: "Group",
                type: "bigint",
                nullable: false,
                defaultValue: 1684118024716L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1684290066084L);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);
        }
    }
}
