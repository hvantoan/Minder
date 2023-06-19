﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;
using Newtonsoft.Json;

namespace Minder.Database.Models {

    public class MatchSetting {
        public string Id { get; set; } = null!;
        public string TeamId { get; set; } = null!;
        public string? StadiumId { get; set; }
        public EDayOfWeek? SelectedDayOfWeek { get; set; }
        public ETime? From { get; set; }
        public ETime? To { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool HasConfirm { get; set; }

        [JsonIgnore]
        public virtual Match? HostMatch { get; set; }

        [JsonIgnore]
        public virtual Match? OpposingMatch { get; set; }

        public virtual Stadium? Stadium { get; set; }
        public virtual Team? Team { get; set; }
    }

    public class MatchSettingSettingConfig : IEntityTypeConfiguration<MatchSetting> {

        public void Configure(EntityTypeBuilder<MatchSetting> builder) {
            builder.ToTable(nameof(MatchSetting));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();
            builder.Property(o => o.StadiumId).HasMaxLength(32);
            builder.Property(o => o.TeamId).HasMaxLength(32);
            builder.Property(o => o.Date)
              .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            builder.HasOne(o => o.Team).WithMany(o => o.MatchSettings).HasForeignKey(o => o.TeamId);
            builder.HasOne(o => o.Stadium).WithMany(o => o.MatchSettings).HasForeignKey(o => o.StadiumId);
        }
    }
}