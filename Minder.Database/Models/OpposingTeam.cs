using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class OpposingTeam {
        public string Id { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public string MatchId { get; set; } = null!;
        public string? StadiumId { get; set; }

        public virtual Match? Match { get; set; }
        public virtual Stadium? Stadium { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<OpposingParticipant>? OpposingParticipants { get; set; }
    }

    public class OpposingTeamSettingConfig : IEntityTypeConfiguration<OpposingTeam> {

        public void Configure(EntityTypeBuilder<OpposingTeam> builder) {
            builder.ToTable(nameof(OpposingTeam));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.Team).WithMany(o => o.OpposingTeam).HasForeignKey(o => o.MatchId);
            builder.HasMany(o => o.OpposingParticipants).WithOne(o => o.OpposingTeam).HasForeignKey(o => o.OpposingTeamId);
        }
    }
}
