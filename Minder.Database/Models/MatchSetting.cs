using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class MatchSetting {
        public string Id { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public string MatchId { get; set; } = null!;
        public string? StadiumId { get; set; }

        public virtual Match? Match { get; set; }
        public virtual Stadium? Stadium { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<MatchParticipant>? MatchParticipants { get; set; }
    }

    public class MatchSettingSettingConfig : IEntityTypeConfiguration<MatchSetting> {

        public void Configure(EntityTypeBuilder<MatchSetting> builder) {
            builder.ToTable(nameof(MatchSetting));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.Match).WithOne(o => o.OpposingTeam).HasForeignKey<Match>(o => o.Id);
            builder.HasOne(o => o.Match).WithOne(o => o.HostTeam).HasForeignKey<Match>(o => o.Id);
            builder.HasOne(o => o.Team).WithMany(o => o.MatchSettings).HasForeignKey(o => o.MatchId);
            builder.HasMany(o => o.MatchParticipants).WithOne(o => o.MatchSetting).HasForeignKey(o => o.MatchSettingId);
        }
    }
}