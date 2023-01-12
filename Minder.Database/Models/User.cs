using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public partial class User {
        public string Id { get; set; } = null!;
        public string? RoleId { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public ESex Sex { get; set; }
        public int Age { get; set; }
        public string? Description { get; set; }

        // Game setting
        public string GameType { get; set; } = string.Empty;

        public string GameTime { get; set; } = string.Empty;
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public double Radius { get; set; }

        //Xếp hạng
        public ERank Rank { get; set; }

        public int Point { get; set; }

        public bool IsAdmin { get; set; }
        public bool IsDelete { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<File>? Files { get; set; }
        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Invited>? Inviteds { get; set; }
    }

    public class UserConfig : IEntityTypeConfiguration<User> {

        public void Configure(EntityTypeBuilder<User> builder) {
            builder.ToTable(nameof(User));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.RoleId).HasMaxLength(32);

            builder.Property(o => o.Name).HasMaxLength(32);
            builder.Property(o => o.Username).IsRequired();
            builder.Property(o => o.Password).IsRequired();
            builder.Property(o => o.Phone).IsRequired();

            // fk
            builder.HasOne(o => o.Role).WithMany(o => o.Users).HasForeignKey(o => o.RoleId);
            builder.HasMany(o => o.Members).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.Inviteds).WithOne(o => o.User).HasForeignKey(o => o.UserId);

            builder.HasData(new User() {
                Id = "92dcba9b0bdd4f32a6170a1322472ead",
                IsAdmin = true,
                Name = "Hồ Văn Toàn",
                Password = "CcW16ZwR+2SFn8AnpaN+dNakxXvQTI3btbcwpiugge2xYM4H2NfaAD0ZAnOcC4k8HnQLQBGLCpgCtggVfyopgg==",
                RoleId = "469b14225a79448c93e4e780aa08f0cc",
                Username = "admin",
                Phone = "0336516906"
            });
        }
    }
}