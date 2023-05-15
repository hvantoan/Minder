using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public partial class Team {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;

        public bool IsDeleted { get; set; }
        public DateTimeOffset CreateAt { get; set; }
        public DateTimeOffset UpdateAt { get; set; }

        public virtual GameSetting? GameSetting { get; set; }
        public virtual ICollection<Member>? Members { get; set; }
        public virtual ICollection<Invitation>? Inviteds { get; set; }
        public virtual ICollection<Group>? Groups { get; set; }
        public virtual ICollection<MatchSetting>? MatchSettings { get; set; }
        public virtual ICollection<TeamRejected>? TeamRejecteds { get; set; }
    }

    public class TeamConfig : IEntityTypeConfiguration<Team> {

        public void Configure(EntityTypeBuilder<Team> builder) {
            builder.ToTable(nameof(Team));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Code).IsRequired();
            builder.Property(o => o.Name).IsRequired();

            builder.Property(o => o.IsDeleted).HasDefaultValue(false);
            builder.Property(o => o.CreateAt).HasDefaultValue(DateTimeOffset.UtcNow)
                .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();
            builder.Property(o => o.UpdateAt).HasDefaultValue(DateTimeOffset.UtcNow)
                .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //filter

            builder.HasQueryFilter(o => !o.IsDeleted);

            // fk
            builder.HasMany(o => o.Members).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
            builder.HasMany(o => o.Inviteds).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
            builder.HasMany(o => o.MatchSettings).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
            builder.HasMany(o => o.TeamRejecteds).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
        }
    }
}