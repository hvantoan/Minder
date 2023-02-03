using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationshipFileUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_User_UploadBy",
                table: "File");

            migrationBuilder.DropIndex(
                name: "IX_File_UploadBy",
                table: "File");

            migrationBuilder.DropColumn(
                name: "GameTime",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "GameType",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "UploadBy",
                table: "File");

            migrationBuilder.AddColumn<string>(
                name: "GameTimes",
                table: "Team",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GameTypes",
                table: "Team",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "File",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_File_UserId",
                table: "File",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_File_User_UserId",
                table: "File",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_File_User_UserId",
                table: "File");

            migrationBuilder.DropIndex(
                name: "IX_File_UserId",
                table: "File");

            migrationBuilder.DropColumn(
                name: "GameTimes",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "GameTypes",
                table: "Team");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "File");

            migrationBuilder.AddColumn<string>(
                name: "GameTime",
                table: "Team",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GameType",
                table: "Team",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UploadBy",
                table: "File",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_File_UploadBy",
                table: "File",
                column: "UploadBy");

            migrationBuilder.AddForeignKey(
                name: "FK_File_User_UploadBy",
                table: "File",
                column: "UploadBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
