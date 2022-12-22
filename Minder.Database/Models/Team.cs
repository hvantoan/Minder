using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Minder.Database.Models {

    public partial class Team {
        public string Id { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string RankId { get; set; } = null!;
        public long ReputationScore { get; set; }
        public DateTimeOffset CreateAt { get; set; }

        public virtual Rank? Rank { get; set; }
        public virtual ICollection<User>? Users { get; set; }
    }

    public class TeamConfig : IEntityTypeConfiguration<Team> {

        public void Configure(EntityTypeBuilder<Team> builder) {
            builder.ToTable(nameof(Team));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Code).IsRequired();
            builder.Property(o => o.Name).IsRequired();
            builder.Property(o => o.RankId).HasMaxLength(32).IsRequired();
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            // fk
            builder.HasMany(o => o.Users).WithOne(o => o.Team).HasForeignKey(o => o.TeamId);
        }
    }
}