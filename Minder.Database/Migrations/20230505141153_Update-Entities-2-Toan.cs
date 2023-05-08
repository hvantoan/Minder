using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntities2Toan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Conversation_ConversationId",
                table: "Participant");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "Participant",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_ConversationId",
                table: "Participant",
                newName: "IX_Participant_GroupId");

            migrationBuilder.RenameColumn(
                name: "ConversationId",
                table: "Message",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_ConversationId",
                table: "Message",
                newName: "IX_Message_GroupId");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683295913748L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683036774060L);

            migrationBuilder.CreateTable(
                name: "Group",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: 1683295913748L);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Group_GroupId",
                table: "Message",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Group_GroupId",
                table: "Participant",
                column: "GroupId",
                principalTable: "Group",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_Group_GroupId",
                table: "Message");

            migrationBuilder.DropForeignKey(
                name: "FK_Participant_Group_GroupId",
                table: "Participant");

            migrationBuilder.DropTable(
                name: "Group");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Participant",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Participant_GroupId",
                table: "Participant",
                newName: "IX_Participant_ConversationId");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "Message",
                newName: "ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_Message_GroupId",
                table: "Message",
                newName: "IX_Message_ConversationId");

            migrationBuilder.AlterColumn<long>(
                name: "DayOfBirth",
                table: "User",
                type: "bigint",
                nullable: false,
                defaultValue: 1683036774060L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldDefaultValue: 1683295913748L);

            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "User",
                keyColumn: "Id",
                keyValue: "92dcba9b0bdd4f32a6170a1322472ead",
                column: "DayOfBirth",
                value: -62135596800000L);

            migrationBuilder.AddForeignKey(
                name: "FK_Message_Conversation_ConversationId",
                table: "Message",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Participant_Conversation_ConversationId",
                table: "Participant",
                column: "ConversationId",
                principalTable: "Conversation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
