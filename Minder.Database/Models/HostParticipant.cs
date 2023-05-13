using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class HostParticipant {
        public string Id { get; set; } = null!;
        public string HostTeamId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public EPosition Position { get; set; }

        public HostTeam? HostTeam { get; set; }
    }

    public class HostParticipantConfig : IEntityTypeConfiguration<HostParticipant> {

        public void Configure(EntityTypeBuilder<HostParticipant> builder) {
            builder.ToTable(nameof(HostParticipant));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.HostTeam).WithMany(o => o.HostParticipants).HasForeignKey(o => o.HostTeamId);
        }
    }
}