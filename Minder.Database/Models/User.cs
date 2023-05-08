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
        public DateTimeOffset DayOfBirth { get; set; }
        public string? Description { get; set; }

        //Rank

        public bool IsAdmin { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Team? Team { get; set; }
        public virtual GameSetting? GameSetting { get; set; }
        public virtual ICollection<Stadium>? Stadiums { get; set; }
        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Invitation>? Inviteds { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Participant>? Participants { get; set; }
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
            builder.Property(o => o.DayOfBirth).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).HasDefaultValue(DateTimeOffset.Now);

            // fk
            builder.HasOne(o => o.Role).WithMany(o => o.Users).HasForeignKey(o => o.RoleId);
            builder.HasMany(o => o.Members).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.Inviteds).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.Stadiums).WithOne(o => o.User).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.Messages).WithOne(o => o.User).HasForeignKey(o => o.SenderId);
            builder.HasMany(o => o.Participants).WithOne(o => o.User).HasForeignKey(o => o.UserId);

            builder.HasData(new User() {
                Id = "92dcba9b0bdd4f32a6170a1322472ead",
                IsAdmin = true,
                Name = "Hồ Văn Toàn",
                Password = "wYoNiOClwsx0kyK9EIRqmaEqg72hAj+U12g16P2YoVo5rYlhxSpU/sHoBXvgPICRHMM1ESCAs/tBHcxov6xjgQ==",
                RoleId = "469b14225a79448c93e4e780aa08f0cc",
                Username = "admin@gmail.com",
                Phone = "0336516906",
                IsActive = true
            });
        }
    }
}