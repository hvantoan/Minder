using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Minder.Database.Models {

    public partial class Rank {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string FileId { get; set; } = null!;
        public DateTimeOffset CreateAt { get; set; }
        public virtual ICollection<Team>? Teams { get; set; }
    }

    public class RankConfig : IEntityTypeConfiguration<Rank> {

        public void Configure(EntityTypeBuilder<Rank> builder) {
            builder.ToTable(nameof(Rank));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            // fk
            builder.HasMany(o => o.Teams).WithOne(o => o.Rank).HasForeignKey(o => o.RankId);
        }
    }
}