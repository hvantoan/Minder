using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Match {
        public string Id { get; set; } = null!;
        public EMatch Status { get; set; }
        public DayOfWeek? SelectedDate { get; set; }

        public virtual MatchSetting? HostTeam { get; set; }
        public virtual MatchSetting? OpposingTeam { get; set; }
    }

    public class MatchConfig : IEntityTypeConfiguration<Match> {

        public void Configure(EntityTypeBuilder<Match> builder) {
            builder.ToTable(nameof(Match));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.HostTeam).WithOne(o => o.Match).HasForeignKey<MatchSetting>(o => o.MatchId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.OpposingTeam).WithOne(o => o.Match).HasForeignKey<MatchSetting>(o => o.MatchId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}