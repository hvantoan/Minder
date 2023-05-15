﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;

namespace Minder.Database.Models {

    public class Match {
        public string Id { get; set; } = null!;
        public string HostTeamId { get; set; } = null!;
        public string OppsingTeamId { get; set; } = null!;
        public EMatch Status { get; set; }
        public DayOfWeek? SelectedDate { get; set; }
        public string? Address { get; set; }
        public ETime? From { get; set; }
        public ETime? To { get; set; }

        public virtual MatchSetting? HostTeam { get; set; }
        public virtual MatchSetting? OpposingTeam { get; set; }
    }

    public class MatchConfig : IEntityTypeConfiguration<Match> {

        public void Configure(EntityTypeBuilder<Match> builder) {
            builder.ToTable(nameof(Match));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.HostTeam).WithOne(o => o.HostMatch).HasForeignKey<Match>(o => o.HostTeamId).OnDelete(DeleteBehavior.NoAction);
            builder.HasOne(o => o.OpposingTeam).WithOne(o => o.OpposingMatch).HasForeignKey<Match>(o => o.OppsingTeamId).OnDelete(DeleteBehavior.NoAction);
        }
    }
}