using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Newtonsoft.Json;

namespace Minder.Database.Models {

    public class TimeItem {
        public string Id { get; set; } = null!;

        public string TimeOpptionId { get; set; } = null!;

        public int? From { get; set; }
        public int? To { get; set; }
        public int MemberCount { get; set; }

        [JsonIgnore]
        public virtual TimeOpption? TimeOpption { get; set; }
    }

    public class TimeItemConfig : IEntityTypeConfiguration<TimeItem> {

        public void Configure(EntityTypeBuilder<TimeItem> builder) {
            builder.ToTable(nameof(TimeItem));

            builder.HasOne(o => o.TimeOpption).WithMany(o => o.TimeItems).HasForeignKey(o => o.TimeOpptionId);
        }
    }
}