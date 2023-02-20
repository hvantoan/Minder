using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class Participant {
        public string Id { get; set; } = null!;
        public string ConversationId { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public DateTimeOffset JoinAt { get; set; }

        public virtual User? User { get; set; }
        public virtual Conversation? Conversation { get; set; }
    }

    public class ParticipantConfig : IEntityTypeConfiguration<Participant> {

        public void Configure(EntityTypeBuilder<Participant> builder) {
            builder.ToTable(nameof(Participant));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.ConversationId).HasMaxLength(32);
            builder.Property(o => o.UserId).HasMaxLength(32);

            builder.Property(o => o.JoinAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk

            builder.HasOne(o => o.User).WithMany(o => o.Participants).HasForeignKey(o => o.UserId);
            builder.HasOne(o => o.Conversation).WithMany(o => o.Participants).HasForeignKey(o => o.ConversationId);
        }
    }
}