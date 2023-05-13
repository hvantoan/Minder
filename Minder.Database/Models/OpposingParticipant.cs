using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class OpposingParticipant {
        public string Id { get; set; } = null!;
        public string OpposingTeamId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public EPosition Position { get; set; }

        public virtual OpposingTeam? OpposingTeam { get; set; }
    }

    public class OpposingParticipantConfig : IEntityTypeConfiguration<OpposingParticipant> {

        public void Configure(EntityTypeBuilder<OpposingParticipant> builder) {
            builder.ToTable(nameof(OpposingParticipant));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.OpposingTeam).WithMany(o => o.OpposingParticipants).HasForeignKey(o => o.OpposingTeamId);
        }
    }
}