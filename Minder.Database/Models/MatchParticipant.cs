using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class MatchParticipant {
        public string Id { get; set; } = null!;
        public string MatchId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public EPosition Position { get; set; }
        public bool IsAccept { get; set; }
        public virtual User? User { get; set; }
        public virtual Match? Match { get; set; }
    }

    public class HostParticipantConfig : IEntityTypeConfiguration<MatchParticipant> {

        public void Configure(EntityTypeBuilder<MatchParticipant> builder) {
            builder.ToTable(nameof(MatchParticipant));

            builder.Property(o => o.MatchId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.User).WithMany(o => o.MatchParticipants).HasForeignKey(o => o.UserId);
            builder.HasOne(o => o.Match).WithMany(o => o.Participants).HasForeignKey(o => o.MatchId);
        }
    }
}