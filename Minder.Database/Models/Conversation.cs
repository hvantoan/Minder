using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class Conversation {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string ChannelId { get; set; } = null!;
        public DateTimeOffset CreateAt { get; set; }

        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Participant>? Participants { get; set; }
    }

    public class ConversationConfig : IEntityTypeConfiguration<Conversation> {

        public void Configure(EntityTypeBuilder<Conversation> builder) {
            builder.ToTable(nameof(Conversation));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.ChannelId).HasMaxLength(32);
            builder.Property(o => o.Title).HasMaxLength(255);
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk

            builder.HasMany(o => o.Messages).WithOne(o => o.Conversation).HasForeignKey(o => o.ConversationId);
            builder.HasMany(o => o.Participants).WithOne(o => o.Conversation).HasForeignKey(o => o.ConversationId);
        }
    }
}