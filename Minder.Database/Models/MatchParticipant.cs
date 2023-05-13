using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class MatchParticipant {
        public string Id { get; set; } = null!;
        public string MatchSettingId { get; set; } = null!;
        public string MemberId { get; set; } = null!;
        public EPosition Position { get; set; }

        public MatchSetting? MatchSetting { get; set; }
        public Member? Member { get; set; }
    }

    public class MatchParticipantConfig : IEntityTypeConfiguration<MatchParticipant> {

        public void Configure(EntityTypeBuilder<MatchParticipant> builder) {
            builder.ToTable(nameof(MatchParticipant));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.MatchSetting).WithMany(o => o.MatchParticipants).HasForeignKey(o => o.MatchSettingId);
            builder.HasOne(o => o.Member).WithMany(o => o.).HasForeignKey(o => o.MatchSettingId);
        }
    }
}