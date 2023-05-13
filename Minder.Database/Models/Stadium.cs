﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class Stadium {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string Code { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Phone { get; set; } = string.Empty;

        public decimal Longitude { get; set; }
        public decimal Latitude { get; set; }

        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Commune { get; set; }
        public string? Address { get; set; }
        public bool IsDeleted { get; set; }
        public DateTimeOffset UpdateAt { get; set; }
        public DateTimeOffset CreateAt { get; set; }

        public virtual User? User { get; set; }
        public virtual ICollection<HostTeam>? HostTeams { get; set; }
        public virtual ICollection<OpposingTeam>? OpposingTeams { get; set; }
    }

    public class StadiumConfig : IEntityTypeConfiguration<Stadium> {

        public void Configure(EntityTypeBuilder<Stadium> builder) {
            builder.ToTable(nameof(Stadium));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.Property(o => o.Code).HasMaxLength(255).IsRequired();
            builder.Property(o => o.Name).HasMaxLength(255).IsRequired();
            builder.Property(o => o.Phone).HasMaxLength(11);

            builder.Property(o => o.Longitude).HasColumnType("decimal(18,15)");
            builder.Property(o => o.Latitude).HasColumnType("decimal(18,15)");

            builder.Property(o => o.Province).HasMaxLength(2);
            builder.Property(o => o.District).HasMaxLength(3);
            builder.Property(o => o.Commune).HasMaxLength(5);
            builder.Property(o => o.Address).HasMaxLength(1000);

            builder.Property(o => o.IsDeleted).HasDefaultValue(false);

            builder.Property(o => o.CreateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o))
                .HasDefaultValue(DateTimeOffset.UtcNow).IsRequired();
            builder.Property(o => o.UpdateAt).HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o))
                .HasDefaultValue(DateTimeOffset.UtcNow).IsRequired();

            //filter

            builder.HasQueryFilter(o => !o.IsDeleted);

            //fk

            builder.HasOne(o => o.User).WithMany(o => o.Stadiums).HasForeignKey(o => o.UserId);
            builder.HasMany(o => o.HostTeams).WithOne(o => o.Stadium).HasForeignKey(o => o.StadiumId);
            builder.HasMany(o => o.OpposingTeams).WithOne(o => o.Stadium).HasForeignKey(o => o.StadiumId);

            // index

            builder.HasIndex(o => o.Id);
            builder.HasIndex(o => new { o.Id, o.Code });
        }
    }
}