using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIsActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "User",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "IsActive",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "User");
        }
    }
}
