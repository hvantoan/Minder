using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public partial class Team {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public DateTimeOffset CreateAt { get; set; }

        public virtual GameSetting? GameSetting { get; set; }
        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Invitation>? Inviteds { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }
    }

    public class TeamConfig : IEntityTypeConfiguration<Team> {

        public void Configure(EntityTypeBuilder<Team> builder) {
            builder.ToTable(nameof(Team));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Code).IsRequired();
            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            // fk
            builder.HasMany(o => o.Members).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
            builder.HasMany(o => o.Inviteds).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
            builder.HasMany(o => o.Groups).WithOne(o => o.Team).HasForeignKey(o => o.TeamId).OnDelete(DeleteBehavior.Cascade);

        }
    }
}