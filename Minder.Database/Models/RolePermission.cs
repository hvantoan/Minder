using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public partial class RolePermission {
        public string RoleId { get; set; } = null!;
        public string PermissionId { get; set; } = null!;
        public bool IsEnable { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Permission? Permission { get; set; }
    }

    public class RolePermissionConfig : IEntityTypeConfiguration<RolePermission> {

        public void Configure(EntityTypeBuilder<RolePermission> builder) {
            builder.ToTable(nameof(RolePermission));

            builder.HasKey(o => new { o.RoleId, o.PermissionId });
            builder.Property(o => o.RoleId).HasMaxLength(32);
            builder.Property(o => o.PermissionId).HasMaxLength(32);

            // fk
            builder.HasOne(o => o.Role).WithMany(o => o.RolePermissions).HasForeignKey(o => o.RoleId);
            builder.HasOne(o => o.Permission).WithMany(o => o.RolePermissions).HasForeignKey(o => o.PermissionId);
            builder.HasData(new RolePermission() {
                RoleId = "469b14225a79448c93e4e780aa08f0cc",
                PermissionId = "ec0f270b424249438540a16e9157c0c8",
                IsEnable = true
            }, new RolePermission() {
                RoleId = "6ffa9fa20755486d9e317d447b652bd8",
                PermissionId = "dc1c2ce584d74428b4e5241a5502787d",
                IsEnable = true
            });
        }
    }
}