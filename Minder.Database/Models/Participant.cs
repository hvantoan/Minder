using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class Participant {
        public string Id { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTimeOffset JoinAt { get; set; }
        public string? LastSendMessageId { get; set; }
        public virtual User? User { get; set; }
        public virtual Group? Group { get; set; }
    }

    public class ParticipantConfig : IEntityTypeConfiguration<Participant> {

        public void Configure(EntityTypeBuilder<Participant> builder) {
            builder.ToTable(nameof(Participant));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.GroupId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.LastSendMessageId).HasMaxLength(32);
            builder.Property(o => o.UserId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.JoinAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk

            builder.HasOne(o => o.User).WithMany(o => o.Participants).HasForeignKey(o => o.UserId);
            builder.HasOne(o => o.Group).WithMany(o => o.Participants).HasForeignKey(o => o.GroupId);
        }
    }
}