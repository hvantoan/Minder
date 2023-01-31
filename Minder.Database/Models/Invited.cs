using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Invited {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public string? Description { get; set; }
        public EInviteType Type { get; set; }
        public DateTimeOffset CreateAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Team? Team { get; set; }
    }

    public class InvitedConfig : IEntityTypeConfiguration<Invited> {

        public void Configure(EntityTypeBuilder<Invited> builder) {
            builder.ToTable(nameof(Invited));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.TeamId).HasMaxLength(32).IsRequired();

            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            // fk
            builder.HasOne(o => o.Team).WithMany(o => o.Inviteds).HasForeignKey(o => o.TeamId);
            builder.HasOne(o => o.User).WithMany(o => o.Inviteds).HasForeignKey(o => o.UserId);
        }
    }
}