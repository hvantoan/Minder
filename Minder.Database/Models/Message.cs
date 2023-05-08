using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Message {
        public string Id { get; set; } = null!;
        public string GroupId { get; set; } = null!;
        public string SenderId { get; set; } = null!;
        public EMessageType MessageType { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTimeOffset CreateAt { get; set; }

        public virtual Group? Group { get; set; }
        public virtual User? User { get; set; }
    }

    public class MessageConfig : IEntityTypeConfiguration<Message> {

        public void Configure(EntityTypeBuilder<Message> builder) {
            builder.ToTable(nameof(Message));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk

            builder.HasOne(o => o.Group).WithMany(o => o.Messages).HasForeignKey(o => o.GroupId);
            builder.HasOne(o => o.User).WithMany(o => o.Messages).HasForeignKey(o => o.SenderId);
        }
    }
}