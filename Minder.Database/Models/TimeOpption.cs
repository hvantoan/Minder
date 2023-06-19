using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minder.Database.Enums;
using Newtonsoft.Json;

namespace Minder.Database.Models {

    public class TimeOpption {
        public string Id { get; set; } = null!;
        public string MatchId { get; set; } = null!;

        [JsonIgnore]
        public EDayOfWeek DayOfWeek { get; set; }

        public DateTimeOffset Date { get; set; }

        [JsonIgnore]
        public virtual Match? Match { get; set; }

        public virtual ICollection<TimeItem>? TimeItems { get; set; }
    }

    public class TimeOpptionConfig : IEntityTypeConfiguration<TimeOpption> {

        public void Configure(EntityTypeBuilder<TimeOpption> builder) {
            builder.ToTable(nameof(TimeOpption));
            builder.Property(o => o.Date)
              .HasConversion(o => o.ToUnixTimeMilliseconds(), o => DateTimeOffset.FromUnixTimeMilliseconds(o)).IsRequired();

            //Fk

            builder.HasMany(o => o.TimeItems).WithOne(o => o.TimeOpption).HasForeignKey(o => o.TimeOpptionId);
            builder.HasOne(o => o.Match).WithMany(o => o.TimeOpptions).HasForeignKey(o => o.MatchId);
        }
    }
}