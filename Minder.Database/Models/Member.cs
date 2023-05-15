using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Member {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public ERegency Regency { get; set; }
        public bool IsDeleted { get; set; }

        public virtual User? User { get; set; }
        public virtual Team? Team { get; set; }
        public virtual ICollection<MatchParticipant>? MatchParticipants { get; set; }
    }

    public class MemberConfig : IEntityTypeConfiguration<Member> {

        public void Configure(EntityTypeBuilder<Member> builder) {
            builder.ToTable(nameof(Member));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.TeamId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.IsDeleted).HasDefaultValue(false);

            // query

            builder.HasQueryFilter(o => !o.IsDeleted);

            // fk
            builder.HasOne(o => o.Team).WithMany(o => o.Members).HasForeignKey(o => o.TeamId);
            builder.HasOne(o => o.User).WithMany(o => o.Members).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.MatchParticipants).WithOne(o => o.Member).HasForeignKey(o => o.MemberId);
        }
    }
}