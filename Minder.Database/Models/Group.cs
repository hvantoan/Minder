using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class Group {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = string.Empty;
        public string? TeamId { get; set; }
        public string? ChannelId { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public EGroup Type { get; set; }
        public Team? Team { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<Participant>? Participants { get; set; }
    }

    public class ConversationConfig : IEntityTypeConfiguration<Group> {

        public void Configure(EntityTypeBuilder<Group> builder) {
            builder.ToTable(nameof(Group));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32);
            builder.Property(o => o.TeamId).HasMaxLength(32);
            builder.Property(o => o.ChannelId).HasMaxLength(32);
            builder.Property(o => o.Title).HasMaxLength(255);
            builder.Property(o => o.CreateAt).HasDefaultValue(DateTimeOffset.Now)
                .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //fk

            builder.HasMany(o => o.Messages).WithOne(o => o.Group).HasForeignKey(o => o.GroupId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(o => o.Participants).WithOne(o => o.Group).HasForeignKey(o => o.GroupId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(o => o.Team).WithMany(o => o.Groups).HasForeignKey(o => o.TeamId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}