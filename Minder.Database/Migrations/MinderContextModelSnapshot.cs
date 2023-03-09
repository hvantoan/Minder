﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minder.Database;

#nullable disable

namespace Minder.Database.Migrations
{
    [DbContext(typeof(MinderContext))]
    partial class MinderContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Minder.Database.Models.Conversation", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ChannelId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<long>("CreateAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Conversation", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.File", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ItemId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("ItemType")
                        .HasMaxLength(20)
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(2147483647)
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Type")
                        .HasMaxLength(20)
                        .HasColumnType("int");

                    b.Property<long>("UploadDate")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("Type");

                    b.ToTable("File", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.GameSetting", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("GameTime")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("GameTypes")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<double>("Radius")
                        .HasColumnType("float");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("GameSetting", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Invitation", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<long>("CreateAt")
                        .HasColumnType("bigint");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TeamId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("Invitation", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Member", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Regency")
                        .HasColumnType("int");

                    b.Property<string>("TeamId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.HasIndex("UserId");

                    b.ToTable("Member", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Message", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Content")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ConversationId")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.Property<long>("CreateAt")
                        .HasColumnType("bigint");

                    b.Property<int>("MessageType")
                        .HasColumnType("int");

                    b.Property<string>("SenderId")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("SenderId");

                    b.ToTable("Message", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Participant", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ConversationId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<long>("JoinAt")
                        .HasColumnType("bigint");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("ConversationId");

                    b.HasIndex("UserId");

                    b.ToTable("Participant", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Permission", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("ClaimName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("Default")
                        .HasColumnType("bit");

                    b.Property<string>("DisplayName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("ParentId")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Permission", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "ec0f270b424249438540a16e9157c0c8",
                            ClaimName = "SEO",
                            Default = true,
                            DisplayName = "Quản lý",
                            IsActive = true,
                            ParentId = "",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "dc1c2ce584d74428b4e5241a5502787d",
                            ClaimName = "SEO.Setting",
                            Default = false,
                            DisplayName = "Cài đặt",
                            IsActive = true,
                            ParentId = "ec0f270b424249438540a16e9157c0c8",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "296285809bac481890a454ea8aed6af4",
                            ClaimName = "SEO.Setting.User",
                            Default = false,
                            DisplayName = "Người dùng",
                            IsActive = true,
                            ParentId = "dc1c2ce584d74428b4e5241a5502787d",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "74e2235cc48d47529e080b62dc699b02",
                            ClaimName = "SEO.Setting.User.Save",
                            Default = false,
                            DisplayName = "Thêm mới và sửa",
                            IsActive = true,
                            ParentId = "296285809bac481890a454ea8aed6af4",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "98873832ebcb4d9fb12e9b21a187f12c",
                            ClaimName = "SEO.Setting.User.Reset",
                            Default = false,
                            DisplayName = "Đặt lại mật khẩu",
                            IsActive = true,
                            ParentId = "296285809bac481890a454ea8aed6af4",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "a8845d8773f345d9b572ef4ee04136cf",
                            ClaimName = "SEO.Project",
                            Default = true,
                            DisplayName = "Project",
                            IsActive = true,
                            ParentId = "296285809bac481890a454ea8aed6af4",
                            Type = "Web"
                        },
                        new
                        {
                            Id = "d6ee70dc6c7c468f8f35206085b1880f",
                            ClaimName = "SEO.Project.Save",
                            Default = false,
                            DisplayName = "Thêm mới và sửa",
                            IsActive = true,
                            ParentId = "a8845d8773f345d9b572ef4ee04136cf",
                            Type = "Web"
                        });
                });

            modelBuilder.Entity("Minder.Database.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.HasKey("Id");

                    b.ToTable("Role", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "469b14225a79448c93e4e780aa08f0cc",
                            Code = "admin",
                            IsDelete = false,
                            Name = "Quản trị viên"
                        },
                        new
                        {
                            Id = "6ffa9fa20755486d9e317d447b652bd8",
                            Code = "user",
                            IsDelete = false,
                            Name = "Người dùng"
                        });
                });

            modelBuilder.Entity("Minder.Database.Models.RolePermission", b =>
                {
                    b.Property<string>("RoleId")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("PermissionId")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<bool>("IsEnable")
                        .HasColumnType("bit");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermission", (string)null);

                    b.HasData(
                        new
                        {
                            RoleId = "469b14225a79448c93e4e780aa08f0cc",
                            PermissionId = "ec0f270b424249438540a16e9157c0c8",
                            IsEnable = true
                        },
                        new
                        {
                            RoleId = "6ffa9fa20755486d9e317d447b652bd8",
                            PermissionId = "dc1c2ce584d74428b4e5241a5502787d",
                            IsEnable = true
                        });
                });

            modelBuilder.Entity("Minder.Database.Models.Stadium", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Address")
                        .HasMaxLength(1000)
                        .HasColumnType("nvarchar(1000)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Commune")
                        .HasMaxLength(5)
                        .HasColumnType("nvarchar(5)");

                    b.Property<long>("CreateAt")
                        .HasColumnType("bigint");

                    b.Property<string>("District")
                        .HasMaxLength(3)
                        .HasColumnType("nvarchar(3)");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("Province")
                        .HasMaxLength(2)
                        .HasColumnType("nvarchar(2)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(32)");

                    b.HasKey("Id");

                    b.HasIndex("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("Id", "Code");

                    b.ToTable("Stadium", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.Team", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<long>("CreateAt")
                        .HasColumnType("bigint");

                    b.Property<string>("GameTimes")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("GameTypes")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<decimal>("Latitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<decimal>("Longitude")
                        .HasColumnType("decimal(18,15)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Point")
                        .HasColumnType("int");

                    b.Property<double>("Radius")
                        .HasColumnType("float");

                    b.Property<int>("Rank")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Team", (string)null);
                });

            modelBuilder.Entity("Minder.Database.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDelete")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<int>("Sex")
                        .HasColumnType("int");

                    b.Property<string>("TeamId")
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.HasIndex("TeamId");

                    b.ToTable("User", (string)null);

                    b.HasData(
                        new
                        {
                            Id = "92dcba9b0bdd4f32a6170a1322472ead",
                            Age = 0,
                            IsAdmin = true,
                            IsDelete = false,
                            Name = "Hồ Văn Toàn",
                            Password = "CcW16ZwR+2SFn8AnpaN+dNakxXvQTI3btbcwpiugge2xYM4H2NfaAD0ZAnOcC4k8HnQLQBGLCpgCtggVfyopgg==",
                            Phone = "0336516906",
                            RoleId = "469b14225a79448c93e4e780aa08f0cc",
                            Sex = 0,
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("Minder.Database.Models.GameSetting", b =>
                {
                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithOne("GameSetting")
                        .HasForeignKey("Minder.Database.Models.GameSetting", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.Invitation", b =>
                {
                    b.HasOne("Minder.Database.Models.Team", "Team")
                        .WithMany("Inviteds")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithMany("Inviteds")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.Member", b =>
                {
                    b.HasOne("Minder.Database.Models.Team", "Team")
                        .WithMany("Members")
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithMany("Members")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.Message", b =>
                {
                    b.HasOne("Minder.Database.Models.Conversation", "Conversation")
                        .WithMany("Messages")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithMany("Messages")
                        .HasForeignKey("SenderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.Participant", b =>
                {
                    b.HasOne("Minder.Database.Models.Conversation", "Conversation")
                        .WithMany("Participants")
                        .HasForeignKey("ConversationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithMany("Participants")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Conversation");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.RolePermission", b =>
                {
                    b.HasOne("Minder.Database.Models.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Minder.Database.Models.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("Minder.Database.Models.Stadium", b =>
                {
                    b.HasOne("Minder.Database.Models.User", "User")
                        .WithMany("Stadiums")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("Minder.Database.Models.User", b =>
                {
                    b.HasOne("Minder.Database.Models.Role", "Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId");

                    b.HasOne("Minder.Database.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Role");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("Minder.Database.Models.Conversation", b =>
                {
                    b.Navigation("Messages");

                    b.Navigation("Participants");
                });

            modelBuilder.Entity("Minder.Database.Models.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("Minder.Database.Models.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("Minder.Database.Models.Team", b =>
                {
                    b.Navigation("Inviteds");

                    b.Navigation("Members");
                });

            modelBuilder.Entity("Minder.Database.Models.User", b =>
                {
                    b.Navigation("GameSetting");

                    b.Navigation("Inviteds");

                    b.Navigation("Members");

                    b.Navigation("Messages");

                    b.Navigation("Participants");

                    b.Navigation("Stadiums");
                });
#pragma warning restore 612, 618
        }
    }
}
