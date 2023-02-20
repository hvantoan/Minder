using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Minder.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Conversation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ChannelId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "File",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Type = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    ItemType = table.Column<int>(type: "int", maxLength: 20, nullable: false),
                    ItemId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Path = table.Column<string>(type: "nvarchar(max)", maxLength: 2147483647, nullable: false),
                    UploadDate = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_File", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ParentId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ClaimName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DisplayName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Default = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Team",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GameTypes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GameTimes = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Radius = table.Column<double>(type: "float", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Team", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermission",
                columns: table => new
                {
                    RoleId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PermissionId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    IsEnable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermission", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermission_Permission_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permission",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermission_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: true),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sex = table.Column<int>(type: "int", nullable: false),
                    Age = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsAdmin = table.Column<bool>(type: "bit", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Role_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Role",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_User_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameSetting",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    GameTypes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GameTimes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Radius = table.Column<double>(type: "float", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Point = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSetting", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSetting_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Invitation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invitation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invitation_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Invitation_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Member",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    TeamId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Regency = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Member", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Member_Team_TeamId",
                        column: x => x.TeamId,
                        principalTable: "Team",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Member_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    SenderId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    MessageType = table.Column<int>(type: "int", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Message", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Message_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Message_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Participant",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ConversationId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    JoinAt = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participant", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participant_Conversation_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "Conversation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participant_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Stadium",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Longitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(18,15)", nullable: false),
                    Province = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: true),
                    District = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: true),
                    Commune = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreateAt = table.Column<long>(type: "bigint", nullable: false),
                    IsDelete = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stadium", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stadium_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "ClaimName", "Default", "DisplayName", "IsActive", "ParentId", "Type" },
                values: new object[,]
                {
                    { "296285809bac481890a454ea8aed6af4", "SEO.Setting.User", false, "Người dùng", true, "dc1c2ce584d74428b4e5241a5502787d", "Web" },
                    { "74e2235cc48d47529e080b62dc699b02", "SEO.Setting.User.Save", false, "Thêm mới và sửa", true, "296285809bac481890a454ea8aed6af4", "Web" },
                    { "98873832ebcb4d9fb12e9b21a187f12c", "SEO.Setting.User.Reset", false, "Đặt lại mật khẩu", true, "296285809bac481890a454ea8aed6af4", "Web" },
                    { "a8845d8773f345d9b572ef4ee04136cf", "SEO.Project", true, "Project", true, "296285809bac481890a454ea8aed6af4", "Web" },
                    { "d6ee70dc6c7c468f8f35206085b1880f", "SEO.Project.Save", false, "Thêm mới và sửa", true, "a8845d8773f345d9b572ef4ee04136cf", "Web" },
                    { "dc1c2ce584d74428b4e5241a5502787d", "SEO.Setting", false, "Cài đặt", true, "ec0f270b424249438540a16e9157c0c8", "Web" },
                    { "ec0f270b424249438540a16e9157c0c8", "SEO", true, "Quản lý", true, "", "Web" }
                });

            migrationBuilder.InsertData(
                table: "Role",
                columns: new[] { "Id", "Code", "IsDelete", "Name" },
                values: new object[,]
                {
                    { "469b14225a79448c93e4e780aa08f0cc", "admin", false, "Quản trị viên" },
                    { "6ffa9fa20755486d9e317d447b652bd8", "user", false, "Người dùng" }
                });

            migrationBuilder.InsertData(
                table: "RolePermission",
                columns: new[] { "PermissionId", "RoleId", "IsEnable" },
                values: new object[,]
                {
                    { "ec0f270b424249438540a16e9157c0c8", "469b14225a79448c93e4e780aa08f0cc", true },
                    { "dc1c2ce584d74428b4e5241a5502787d", "6ffa9fa20755486d9e317d447b652bd8", true }
                });

            migrationBuilder.InsertData(
                table: "User",
                columns: new[] { "Id", "Age", "Description", "IsAdmin", "IsDelete", "Name", "Password", "Phone", "RoleId", "Sex", "TeamId", "Username" },
                values: new object[] { "92dcba9b0bdd4f32a6170a1322472ead", 0, null, true, false, "Hồ Văn Toàn", "CcW16ZwR+2SFn8AnpaN+dNakxXvQTI3btbcwpiugge2xYM4H2NfaAD0ZAnOcC4k8HnQLQBGLCpgCtggVfyopgg==", "0336516906", "469b14225a79448c93e4e780aa08f0cc", 0, null, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_File_Type",
                table: "File",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_GameSetting_UserId",
                table: "GameSetting",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_TeamId",
                table: "Invitation",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Invitation_UserId",
                table: "Invitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_TeamId",
                table: "Member",
                column: "TeamId");

            migrationBuilder.CreateIndex(
                name: "IX_Member_UserId",
                table: "Member",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_ConversationId",
                table: "Message",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_SenderId",
                table: "Message",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_ConversationId",
                table: "Participant",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Participant_UserId",
                table: "Participant",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermission_PermissionId",
                table: "RolePermission",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Stadium_Id",
                table: "Stadium",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Stadium_Id_Code",
                table: "Stadium",
                columns: new[] { "Id", "Code" });

            migrationBuilder.CreateIndex(
                name: "IX_Stadium_UserId",
                table: "Stadium",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_User_RoleId",
                table: "User",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_User_TeamId",
                table: "User",
                column: "TeamId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "File");

            migrationBuilder.DropTable(
                name: "GameSetting");

            migrationBuilder.DropTable(
                name: "Invitation");

            migrationBuilder.DropTable(
                name: "Member");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "Participant");

            migrationBuilder.DropTable(
                name: "RolePermission");

            migrationBuilder.DropTable(
                name: "Stadium");

            migrationBuilder.DropTable(
                name: "Conversation");

            migrationBuilder.DropTable(
                name: "Permission");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropTable(
                name: "Role");

            migrationBuilder.DropTable(
                name: "Team");
        }
    }
}
