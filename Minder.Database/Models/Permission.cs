using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using ToolSeoViet.Database.Enums;

namespace Minder.Database.Models {

    public partial class Permission {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string ClaimName { get; set; }
        public string DisplayName { get; set; }
        public bool Default { get; set; }
        public bool IsActive { get; set; }
        public EPermission  Type { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }

    public class PermissionConfig : IEntityTypeConfiguration<Permission> {

        public void Configure(EntityTypeBuilder<Permission> builder) {
            builder.ToTable(nameof(Permission));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.ParentId).HasMaxLength(32);

            builder.Property(o => o.ClaimName).HasMaxLength(50).IsRequired();
            builder.Property(o => o.DisplayName).HasMaxLength(50).IsRequired();

            builder.Property(o => o.Type).HasConversion(o => o.ToString(), o => (EPermission)Enum.Parse(typeof(EPermission), o));

            // fk
            builder.HasMany(o => o.RolePermissions).WithOne(o => o.Permission).HasForeignKey(o => o.PermissionId);

            // seed data

            builder.HasData(new Permission
            {
                Id = "ec0f270b424249438540a16e9157c0c8",
                ParentId = "",
                ClaimName = "SEO",
                DisplayName = "Quản lý",
                Default = true,
                IsActive = true,
                Type = EPermission.Web,
            });

            builder.HasData(new Permission
            {
                Id = "dc1c2ce584d74428b4e5241a5502787d",
                ParentId = "ec0f270b424249438540a16e9157c0c8",
                ClaimName = "SEO.Setting",
                DisplayName = "Cài đặt",
                Default = false,
                IsActive = true,
                Type = EPermission.Web,
            });

            builder.HasData(new Permission
            {
                Id = "296285809bac481890a454ea8aed6af4",
                ParentId = "dc1c2ce584d74428b4e5241a5502787d",
                ClaimName = "SEO.Setting.User",
                DisplayName = "Người dùng",
                Default = false,
                IsActive = true,
                Type = EPermission.Web,
            }, new Permission
            {
                Id = "74e2235cc48d47529e080b62dc699b02",
                ParentId = "296285809bac481890a454ea8aed6af4",
                ClaimName = "SEO.Setting.User.Save",
                DisplayName = "Thêm mới và sửa",
                Default = false,
                IsActive = true,
                Type = EPermission.Web,
            }, new Permission
            {
                Id = "98873832ebcb4d9fb12e9b21a187f12c",
                ParentId = "296285809bac481890a454ea8aed6af4",
                ClaimName = "SEO.Setting.User.Reset",
                DisplayName = "Đặt lại mật khẩu",
                Default = false,
                IsActive = true,
                Type = EPermission.Web,
            }, new Permission
            {
                Id = "a8845d8773f345d9b572ef4ee04136cf",
                ParentId = "296285809bac481890a454ea8aed6af4",
                ClaimName = "SEO.Project",
                DisplayName = "Project",
                Default = true,
                IsActive = true,
                Type = EPermission.Web,
            }, new Permission
            {
                Id = "d6ee70dc6c7c468f8f35206085b1880f",
                ParentId = "a8845d8773f345d9b572ef4ee04136cf",
                ClaimName = "SEO.Project.Save",
                DisplayName = "Thêm mới và sửa",
                Default = false,
                IsActive = true,
                Type = EPermission.Web,
            });
        }
    }
}