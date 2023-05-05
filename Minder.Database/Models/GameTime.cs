using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Minder.Database.Models {

    public class GameTime {
        public string Id { get; set; } = null!;
        public string Monday { get; set; } = string.Empty;
        public string Tuesday { get; set; } = string.Empty;
        public string Wednesday { get; set; } = string.Empty;
        public string Thursday { get; set; } = string.Empty;
        public string Friday { get; set; } = string.Empty;
        public string Saturday { get; set; } = string.Empty;
        public string Sunday { get; set; } = string.Empty;

        // Fk
        public string GameSettingId { get; set; } = null!;
        public virtual GameSetting? GameSetting { get; set; }
    }

    public class GameTimeConfig : IEntityTypeConfiguration<GameTime> {

        public void Configure(EntityTypeBuilder<GameTime> builder) {
            builder.ToTable(nameof(GameTime));

            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasMaxLength(32).IsRequired();

            builder.HasOne(o => o.GameSetting).WithOne(o => o.GameTime).HasForeignKey<GameTime>(o => o.GameSettingId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}