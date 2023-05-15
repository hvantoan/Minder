using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class MatchParticipant {
        public string MemberId { get; set; } = null!;
        public string MatchSettingId { get; set; } = null!;
        public EPosition Position { get; set; }

        public virtual MatchSetting? MatchSetting { get; set; }
        public virtual Member? Member { get; set; }
    }

    public class HostParticipantConfig : IEntityTypeConfiguration<MatchParticipant> {

        public void Configure(EntityTypeBuilder<MatchParticipant> builder) {
            builder.ToTable(nameof(MatchParticipant));

            builder.HasKey(o => new { o.MemberId, o.MatchSettingId });
            builder.Property(o => o.MemberId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.MatchSettingId).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.MatchSetting).WithMany(o => o.MatchParticipants).HasForeignKey(o => o.MatchSettingId);
            builder.HasOne(o => o.Member).WithMany(o => o.MatchParticipants).HasForeignKey(o => o.MatchSettingId);
        }
    }
}