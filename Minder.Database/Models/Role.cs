using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Minder.Database.Models {

    public partial class Role {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public virtual ICollection<User>? Users { get; set; }
        public virtual ICollection<RolePermission>? RolePermissions { get; set; }
    }

    public class RoleConfig : IEntityTypeConfiguration<Role> {

        public void Configure(EntityTypeBuilder<Role> builder) {
            builder.ToTable(nameof(Role));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);

            builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
            builder.Property(o => o.Name).HasMaxLength(255).IsRequired();

            // fk
            builder.HasMany(o => o.Users).WithOne(o => o.Role).HasForeignKey(o => o.RoleId);
            builder.HasMany(o => o.RolePermissions).WithOne(o => o.Role).HasForeignKey(o => o.RoleId);

            builder.HasData(new Role() {
                Id = "469b14225a79448c93e4e780aa08f0cc",
                Name = "Quản trị viên",
                Code = "admin"
            }, new Role() {
                Id = "6ffa9fa20755486d9e317d447b652bd8",
                Code = "user",
                Name = "Người dùng"
            });
        }
    }
}