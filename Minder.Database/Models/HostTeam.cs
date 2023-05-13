using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class HostTeam {
        public string Id { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public string MatchId { get; set; } = null!;
        public string? StadiumId { get; set; }

        public virtual Match? Match { get; set; }
        public virtual Stadium? Stadium { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<HostParticipant>? HostParticipants { get; set; }
    }

    public class HostTeamSettingConfig : IEntityTypeConfiguration<HostTeam> {

        public void Configure(EntityTypeBuilder<HostTeam> builder) {
            builder.ToTable(nameof(HostTeam));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.Team).WithMany(o => o.HostTeams).HasForeignKey(o => o.MatchId);
            builder.HasMany(o => o.HostParticipants).WithOne(o => o.HostTeam).HasForeignKey(o => o.HostTeamId);
        }
    }
}