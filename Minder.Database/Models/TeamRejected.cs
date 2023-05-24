using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class TeamRejected {
        public string TeamId { get; set; } = null!;
        public string ItemId { get; set; } = null!;
        public ETeamRejected Type { get; set; }
        public DateTimeOffset CreateAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual Team? Team { get; set; }
    }

    public class TeamRejectedConfig : IEntityTypeConfiguration<TeamRejected> {

        public void Configure(EntityTypeBuilder<TeamRejected> builder) {
            builder.ToTable(nameof(TeamRejected));
            builder.HasKey(o => new { o.TeamId, o.ItemId });
            builder.Property(x => x.TeamId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.ItemId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.CreateAt)
              .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            builder.HasOne(o => o.Team).WithMany(o => o.TeamRejecteds).HasForeignKey(o => o.TeamId);
        }
    }
}